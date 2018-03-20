using LyCilph.States;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace LyCilph
{
    public class SnakeGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch sprite_batch;
        private InputManager input_manager;
        private StateManager state_manager;

        public SnakeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = Settings.screen_width;
            graphics.PreferredBackBufferHeight = Settings.screen_height;
            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / Settings.TargetFPS);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            input_manager = new InputManager();
            state_manager = new StateManager(GraphicsDevice);
            
            state_manager.TransitionToMainMenuState();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            sprite_batch = new SpriteBatch(GraphicsDevice);

            state_manager.LoadContent(Content);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime game_time)
        {
            input_manager.Begin();

            // Handle input
            if (input_manager.IsPressed(Keys.Escape))
                Exit();
            state_manager.CurrentState.HandleInput(input_manager);

            input_manager.End();

            // Update logic
            state_manager.CurrentState.Update(game_time);
            base.Update(game_time);
        }

        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.White);

            sprite_batch.Begin();

            state_manager.CurrentState.Draw(sprite_batch);
            base.Draw(game_time);

            sprite_batch.End();
        }
    }
}
