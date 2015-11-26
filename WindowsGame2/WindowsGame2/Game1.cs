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

namespace WindowsGame2
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Vector3 LightDirection;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            
            graphics.PreferMultiSampling = true;
            graphics.IsFullScreen = true;
            graphics.PreferredBackBufferHeight =
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            graphics.PreferredBackBufferWidth =
            GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        // Set the 3D model to draw.
        Model myModel;

        // The aspect ratio determines how to scale 3d to 2d projection.
        float aspectRatio;

        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            myModel = Content.Load<Model>("ST\\ST_hi");
            aspectRatio = (float)graphics.GraphicsDevice.Viewport.Width /
            (float)graphics.GraphicsDevice.Viewport.Height;

            LightDirection = new Vector3(2, -2, -2);
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if (Keyboard.GetState(PlayerIndex.One).IsKeyDown(Keys.Escape))
                this.Exit();

            modelRotation += (float)gameTime.ElapsedGameTime.TotalMilliseconds * MathHelper.ToRadians(0.05f);

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
                        //Включить источник направленного света №0
                        effect.DirectionalLight0.Enabled = true;
                        //Настроить параметры
                        effect.DirectionalLight0.DiffuseColor = new Vector3(1, 0.92549f,0.643137f);
                        //effect.DirectionalLight0.SpecularColor = new Vector3(1, 0.92549f, 0.643137f);
                        //Направление света - в класса Game1 мы меняем направление
                        //по клавиатурным командам
                        effect.DirectionalLight0.Direction = Vector3.Normalize(LightDirection);
                        //Включить освещение
                        effect.LightingEnabled = true;

                    effect.World = Matrix.CreateRotationY(modelRotation) * transforms[mesh.ParentBone.Index] 
                        * Matrix.CreateTranslation(modelPosition) * Matrix.CreateScale(40.0f*aspectRatio);
                 
                    effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                    effect.Projection = Matrix.CreateOrthographic((float)graphics.GraphicsDevice.Viewport.Width,
                    (float)graphics.GraphicsDevice.Viewport.Height,
                    -1000.0f, 1000.0f);
                }
                else
                foreach (BasicEffect effect in mesh.Effects)
                    {
                        //effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateTranslation(modelPosition) * Matrix.CreateScale(40.0f * aspectRatio);

                        effect.View = Matrix.CreateLookAt(cameraPosition, Vector3.Zero, Vector3.Up);
                        effect.Projection = Matrix.CreateOrthographic((float)graphics.GraphicsDevice.Viewport.Width,
                        (float)graphics.GraphicsDevice.Viewport.Height,
                        -1000.0f, 1000.0f);
                    }

                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}
