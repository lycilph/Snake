using LyCilph.Elements;
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
        private KeyboardState new_state, old_state;

        private int text_area_width = 400;

        private MainMenuState main_menu_state;
        private GameState game_state;
        private GameElement current_state;
        private GameElement debug;

        public Settings Settings { get; private set; }
        public int ScreenWidth { get { return graphics.PreferredBackBufferWidth; } }
        public int ScreenHeight { get { return graphics.PreferredBackBufferHeight; } }
        public int TextAreaStart { get { return Settings.Size * Settings.CellSize + Margin * 2; } }
        public int Margin { get; private set; } = 10;

        public SnakeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            Settings = Settings.Load();

            graphics.PreferredBackBufferWidth = Settings.Size * Settings.CellSize + Margin * 2 + text_area_width;
            graphics.PreferredBackBufferHeight = Settings.Size * Settings.CellSize + Margin * 2;
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
            main_menu_state = new MainMenuState(this);
            game_state = new GameState(this);
            current_state = main_menu_state;

            debug = new Debug(this);

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

            main_menu_state.LoadContent();
            game_state.LoadContent();
            debug.LoadContent();
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

            current_state.UpdateInput(new_state, old_state);
            debug.UpdateInput(new_state, old_state);

            // Save current keyboard state for next cycle
            old_state = new_state;

            // Update game logic
            current_state.UpdateLogic(game_time);
            debug.UpdateLogic(game_time);

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
            debug.Draw(sprite_batch);

            base.Draw(gameTime);

            sprite_batch.End();

        }

        public void TransitionToGameState()
        {
            current_state = game_state;
        }
    }
}
