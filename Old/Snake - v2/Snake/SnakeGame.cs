using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Snake
{
    public class SnakeGame : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch sprite_batch;

        private KeyboardState new_state, old_state;

        private MenuGameState menu_state;
        private PlayingGameState playing_state;
        private GameOverState game_over_state;
        private GameElement current_state;

        private FPSComponent fps;

        public SnakeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 600;

            graphics.SynchronizeWithVerticalRetrace = false;
            graphics.ApplyChanges();

            TargetElapsedTime = TimeSpan.FromSeconds(1.0 / 200.0);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            menu_state = new MenuGameState(this, graphics.PreferredBackBufferWidth);
            playing_state = new PlayingGameState(this);
            game_over_state = new GameOverState(this, graphics.PreferredBackBufferWidth, playing_state.Snake);

            current_state = menu_state;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            sprite_batch = new SpriteBatch(GraphicsDevice);

            menu_state.LoadContent();
            playing_state.LoadContent();
            game_over_state.LoadContent();

            fps = new FPSComponent(this, sprite_batch, Content.Load<SpriteFont>("font"));
            Components.Add(fps);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="game_time">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime game_time)
        {
            // Poll for current keyboard state
            new_state = Keyboard.GetState();

            // If they hit esc, exit
            if (new_state.IsKeyDown(Keys.Escape))
                Exit();

            // Turn on/off debug information
            if (old_state.IsKeyUp(Keys.F3) && new_state.IsKeyDown(Keys.F3))
            {
                fps.Enabled = !fps.Enabled;
                fps.Visible = fps.Enabled;
            }

            current_state.UpdateInput(new_state, old_state);

            // Save current keyboard state for next cycle
            old_state = new_state;

            // Update game logic
            current_state.UpdateLogic(game_time);

            base.Update(game_time);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.White);

            sprite_batch.Begin();

            current_state.Draw(sprite_batch);
            base.Draw(gameTime);

            sprite_batch.End();
        }

        public void TransitionToGameState()
        {
            playing_state.Reset();
            current_state = playing_state;
        }

        public void TransitionToMenuState()
        {
            current_state = menu_state;
        }

        public void TransitionToGameOverState()
        {
            current_state = game_over_state;
        }
    }
}
