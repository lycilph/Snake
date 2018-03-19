using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Snake
{
    public class SnakeGame : Game
    {
        private enum GameStates { Menu, Playing, AIPlaying };

        private GraphicsDeviceManager graphics;
        private SpriteBatch sprite_batch;
        private KeyboardState new_state, old_state;
        private SpriteFont header_font;
        private SpriteFont text_font;

        private GameStates game_state;
        private SnakePlayer snake_player;
        private SnakeNeuralNetwork snake_ai;
        private Food food;
        private int update_delta;
        private int update_delta_threshold;
        public SnakeGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferWidth = 800; // 40 cells wide
            graphics.PreferredBackBufferHeight = 600; // 30 cells heigh

            //IsFixedTimeStep = true;
            //TargetElapsedTime = TimeSpan.FromMilliseconds(80); // 20 milliseconds == 50 FPS, 80 msec == 12,5 FPS
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            update_delta = 0;
            update_delta_threshold = 100;

            game_state = GameStates.Menu;

            snake_player = new SnakePlayer();
            snake_ai = new SnakeNeuralNetwork();
            food = new Food();

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

            header_font = Content.Load<SpriteFont>("header_font");
            text_font = Content.Load<SpriteFont>("text_font");

            snake_player.LoadContent(Content);
            snake_ai.LoadContent(Content);
            food.LoadContent(Content);
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

            switch (game_state)
            {
                case GameStates.Menu:
                    UpdateInputMenu();
                    break;
                case GameStates.Playing:
                    UpdateInputPlaying();
                    break;
                case GameStates.AIPlaying:
                    UpdateInputAIPlaying();
                    break;
            }

            // Save current keyboard state for next cycle
            old_state = new_state;

            switch (game_state)
            {
                case GameStates.Playing:
                    UpdateLogicPlaying(game_time);
                    break;
                case GameStates.AIPlaying:
                    UpdateLogicAIPlaying(game_time);
                    break;
            }

            base.Update(game_time);
        }

        private void UpdateInputMenu()
        {
            if (old_state.IsKeyUp(Keys.N) && new_state.IsKeyDown(Keys.N))
            {
                snake_player.Reset();
                food.Random();
                update_delta_threshold = 100;

                game_state = GameStates.Playing;
            }

            if (old_state.IsKeyUp(Keys.A) && new_state.IsKeyDown(Keys.A))
            {
                snake_ai.Reset();
                food.Random();
                update_delta_threshold = 100;

                game_state = GameStates.AIPlaying;
            }
        }

        private void UpdateInputPlaying()
        {
            snake_player.UpdateInput(new_state, old_state);
        }

        private void UpdateInputAIPlaying()
        {
            if (old_state.IsKeyUp(Keys.Add) && new_state.IsKeyDown(Keys.Add))
            {
                update_delta_threshold += 10;
            }

            if (old_state.IsKeyUp(Keys.Subtract) && new_state.IsKeyDown(Keys.Subtract))
            {
                update_delta_threshold -= 10;
                if (update_delta_threshold < 10)
                    update_delta_threshold = 10;
            }
        }

        private void UpdateLogicPlaying(GameTime game_time)
        {
            update_delta += game_time.ElapsedGameTime.Milliseconds;
            Debug.WriteLine($"Update delta: {update_delta} - {game_time.TotalGameTime.Seconds}");

            // This means that the snake will move once every 100 ms
            if (update_delta >= update_delta_threshold)
            {
                update_delta = 0;
                snake_player.Move();

                // Was the snake out of bounds?
                if (snake_player.Head.X < 0 || snake_player.Head.X >= 40 || snake_player.Head.Y < 0 || snake_player.Head.Y >= 30)
                {
                    game_state = GameStates.Menu;
                }

                // Did the snake hit itself?
                if (snake_player.HitBody())
                {
                    game_state = GameStates.Menu;
                }

                // Did the snake eat the food?
                if (snake_player.Head.X == food.X && snake_player.Head.Y == food.Y)
                {
                    snake_player.HasEaten = true;
                    food.Random();
                }
            }
        }

        private void UpdateLogicAIPlaying(GameTime game_time)
        {
            update_delta += game_time.ElapsedGameTime.Milliseconds;
            Debug.WriteLine($"Update delta: {update_delta} - {game_time.TotalGameTime.Seconds}");

            // This means that the snake will move once every 100 ms
            if (update_delta >= update_delta_threshold)
            {
                update_delta = 0;

                // Get input for ai
                snake_ai.UpdateInput(food);
                // Run neural network
                snake_ai.Run();
                // Get output from ai and move
                snake_ai.SetDirection();
                snake_ai.Move();
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="game_time">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime game_time)
        {
            GraphicsDevice.Clear(Color.White);

            sprite_batch.Begin();

            switch (game_state)
            {
                case GameStates.Menu:
                    DrawMenu();
                    break;
                case GameStates.Playing:
                    DrawPlaying();
                    break;
                case GameStates.AIPlaying:
                    DrawAIPlaying();
                    break;
            }

            sprite_batch.End();

            base.Draw(game_time);
        }

        private void DrawMenu()
        {
            var header = "Snake";
            var header_size = header_font.MeasureString(header);
            var text_x = (graphics.PreferredBackBufferWidth - header_size.X) / 2;

            sprite_batch.DrawString(header_font, header, new Vector2(text_x, 220), Color.Black);
            sprite_batch.DrawString(text_font, "(N)ew Game", new Vector2(text_x, 250), Color.Black);
            sprite_batch.DrawString(text_font, "(A)i Game", new Vector2(text_x, 270), Color.Black);
            sprite_batch.DrawString(text_font, "(Esc) Quit", new Vector2(text_x, 290), Color.Black);
        }

        private void DrawPlaying()
        {
            food.Draw(sprite_batch);
            snake_player.Draw(sprite_batch);
            sprite_batch.DrawString(text_font, $"Score {snake_player.Score}", new Vector2(5, 5), Color.Black);
        }

        private void DrawAIPlaying()
        {
            food.Draw(sprite_batch);
            snake_ai.Draw(sprite_batch);
            sprite_batch.DrawString(text_font, $"Score {snake_player.Score}", new Vector2(5, 5), Color.Black);
            sprite_batch.DrawString(text_font, $"Update {update_delta_threshold}", new Vector2(5, 25), Color.Black);
        }
    }
}
