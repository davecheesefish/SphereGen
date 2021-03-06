﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SphereGen
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class SphereGen : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont hudFont;

        KeyboardState keyboardState;
        KeyboardState pastKeyboardState;

        Camera camera;
        Icosphere sphere;

        int refineCount = 0;

        public SphereGen()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // Set graphics resolution.
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            this.IsFixedTimeStep = false;

            keyboardState = Keyboard.GetState();
            pastKeyboardState = keyboardState;

            sphere = new Icosphere();
            camera = new Camera(new Vector3(0, 0, -2), Vector3.Zero, MathHelper.PiOver4, (float)graphics.PreferredBackBufferWidth / (float)graphics.PreferredBackBufferHeight);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            hudFont = Content.Load<SpriteFont>("fonts/hudfont");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Update the keyboard state.
            pastKeyboardState = keyboardState;
            keyboardState = Keyboard.GetState();

            // Exit if escape is pressed.
            if (keyboardState.IsKeyDown(Keys.Escape))
                Exit();

            // Refine the sphere if space is pressed.
            if (keyboardState.IsKeyUp(Keys.Space) && pastKeyboardState.IsKeyDown(Keys.Space))
            {
                sphere.Refine();
                ++refineCount;
            }

            camera.Position = new Vector3((float)(3.0 * Math.Sin(gameTime.TotalGameTime.TotalSeconds)), 0.5f, (float)(3.0 * Math.Cos(gameTime.TotalGameTime.TotalSeconds)));

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            sphere.Draw(GraphicsDevice, camera);

            spriteBatch.Begin();
            double fps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;
            spriteBatch.DrawString(hudFont, "Refinements: " + refineCount.ToString() + "  Faces: " + sphere.FaceCount.ToString() + "  FPS: " + Math.Floor(fps).ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(hudFont, "David Prior 2016 - davecheesefish.com - davidprior.media", new Vector2(10, GraphicsDevice.Viewport.Height - 25), Color.DarkGray);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
