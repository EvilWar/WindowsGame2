using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
//using Microsoft.Xna.Framework.Net;
//using Microsoft.Xna.Framework.Storage;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace WindowsGame2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
     
    internal static class NativeMethods
    {

        [DllImport("user32.dll")]
        internal static extern Int32 SetWindowLong(IntPtr hWnd, int nIndex, int nNewIndex);

        [DllImport("user32.dll")]
        internal static extern bool GetClientRect(IntPtr hWnd, ref RECT rect);

        [DllImport("user32.dll")]
        internal static extern void MoveWindow(IntPtr hwnd, int x, int y, int width, int height, bool repaint);

        [DllImport("user32.dll")]
        internal static extern void SetParent(IntPtr childHwnd, IntPtr parentHwnd);

        [DllImport("user32.DLL", EntryPoint = "IsWindowVisible")]
        internal static extern bool IsWindowVisible(IntPtr hWnd);

        internal struct RECT
        {
            public int left,
                            top,
                            right,
                            bottom;
        }
    }
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector3 LightDirection;
        Point old_mouse;

        ContentManager content;

        private const int GWL_STYLE = -16;
        private const int WS_CHILD = 1073741824;
        
        private IntPtr _parentHwnd = new IntPtr(0);
        private bool _previewMode = false;
        private float modelSize;

        public Game1()
        {
            
            graphics = new GraphicsDeviceManager(this);
            content = new ResourceContentManager(Services, Resource1.ResourceManager);
            //Content.RootDirectory = "Content";
            //StalTrans.Resource1.ResourceManager.
            //_previewMode = true;

        }

        public Game1(IntPtr parentHwnd)
        {
            graphics = new GraphicsDeviceManager(this);
            content = new ResourceContentManager(Services, Resource1.ResourceManager);
            //Content.RootDirectory = "Content";
            _previewMode = true;
            _parentHwnd = parentHwnd;
            
        }

        protected override void Initialize()
        {
            if (_previewMode == false)
            {
                graphics.PreferMultiSampling = true;
                graphics.IsFullScreen = true;
                //graphics.PreferredBackBufferHeight =
                //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                //graphics.PreferredBackBufferWidth =
                //GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                graphics.PreferredBackBufferFormat = graphics.GraphicsDevice.DisplayMode.Format;
                graphics.PreferredBackBufferWidth = graphics.GraphicsDevice.DisplayMode.Width;
                graphics.PreferredBackBufferHeight = graphics.GraphicsDevice.DisplayMode.Height;
                graphics.IsFullScreen = true;
                IsMouseVisible = false;
                graphics.ApplyChanges();
            }
            else
            {
                graphics.IsFullScreen = false;
                IsMouseVisible = true;
                //IMPORTANT NOTE: Do not execute the apply changes after the below code or the preview
                // will not display correctly.
                graphics.ApplyChanges();

                if (NativeMethods.IsWindowVisible(_parentHwnd) == true)
                {
                    NativeMethods.RECT wndRect = new NativeMethods.RECT();
                    NativeMethods.GetClientRect(_parentHwnd, ref wndRect);
                    NativeMethods.SetParent(Window.Handle, _parentHwnd);
                    NativeMethods.SetWindowLong(Window.Handle, GWL_STYLE, WS_CHILD);
                    NativeMethods.MoveWindow(Window.Handle, wndRect.left, wndRect.top, wndRect.right, wndRect.bottom, true);
                }
                

            }

            modelSize = 50.0f * (graphics.PreferredBackBufferWidth / 1366.0f);
            base.Initialize();
        }

        // Set the 3D model to draw.
        Model myModel;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            try
            {
                spriteBatch = new SpriteBatch(GraphicsDevice);
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show("SpriteBatch: "+ex.FileName);
            }

            try
            {
                myModel = content.Load<Model>("ST_hi");
            }
            catch (System.IO.FileNotFoundException ex)
            {
                MessageBox.Show("Content.Load: "+ex.FileName);
            }


            LightDirection = new Vector3(2, -2, -2);
            old_mouse.X = Mouse.GetState().X;
            old_mouse.Y = Mouse.GetState().Y;

            aspectRatio = (float)graphics.PreferredBackBufferWidth /
            (float)graphics.PreferredBackBufferHeight;
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
                Exit();
            
            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Escape))
                Exit();

            if (Math.Abs(old_mouse.X - Mouse.GetState().X) > 5 || Math.Abs(old_mouse.Y - Mouse.GetState().Y) > 5)
                if (_previewMode == false)
                    Exit();

            if (_previewMode == true)
            {
                if (NativeMethods.IsWindowVisible(_parentHwnd) == false)
                    Exit();
            }

            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.05f);

            old_mouse.X = Mouse.GetState().X;
            old_mouse.Y = Mouse.GetState().Y;

            base.Update(gameTime);
        }


        // Set the position of the model in world space, and set the rotation.
        Vector3 modelPosition = new Vector3 (0.0f, 1.0f, 0.0f);
        float modelRotation = 0.0f;

        // Set the position of the camera in world space, for our view matrix.
        Vector3 cameraPosition = new Vector3(0.0f, 0.0f, 10.0f);

        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.White);

            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[myModel.Bones.Count];
            myModel.CopyAbsoluteBoneTransformsTo(transforms);

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in myModel.Meshes)
            {
                // This is where the mesh orientation is set, as well as our camera and projection.
                if (mesh.Name == "Object001")
                foreach (BasicEffect effect in mesh.Effects)
                {
                    //effect.EnableDefaultLighting();
                        effect.AmbientLightColor = new Vector3(0.95f, 0.54f, 0.09f);
                        //�������� �������� ������������� ����� �0
                        effect.DirectionalLight0.Enabled = true;
                        //��������� ���������
                        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 0.92549f,0.643137f);
                        //effect.DirectionalLight0.SpecularColor = new Vector3(1, 0.92549f, 0.643137f);
                        //����������� ����� - � ������ Game1 �� ������ �����������
                        //�� ������������ ��������
                        effect.DirectionalLight0.Direction = Vector3.Normalize(LightDirection);
                        //�������� ���������
                        effect.LightingEnabled = true;

                    effect.World = Matrix.CreateRotationY(modelRotation) * transforms[mesh.ParentBone.Index] 
                        * Matrix.CreateTranslation(modelPosition) * Matrix.CreateScale(modelSize * aspectRatio);
                 
                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                        //effect.Projection = Matrix.CreateOrthographic((float)graphics.GraphicsDevice.Viewport.Width,
                        //(float)graphics.GraphicsDevice.Viewport.Height,
                        //-1000.0f, 1000.0f);
                        effect.Projection = Matrix.CreateOrthographic(graphics.GraphicsDevice.DisplayMode.Width, graphics.GraphicsDevice.DisplayMode.Height, -1000, 1000);
                }
                else
                foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateTranslation(modelPosition) * Matrix.CreateScale(modelSize * aspectRatio);

                        effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                        //effect.Projection = Matrix.CreateOrthographic((float)graphics.GraphicsDevice.Viewport.Width,
                        //(float)graphics.GraphicsDevice.Viewport.Height,
                        //-1000.0f, 1000.0f);
                        effect.Projection = Matrix.CreateOrthographic(graphics.GraphicsDevice.DisplayMode.Width, graphics.GraphicsDevice.DisplayMode.Height, -1000, 1000);
                    }

                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}
