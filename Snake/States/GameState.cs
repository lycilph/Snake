using LyCilph.Controllers;
using LyCilph.Elements;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Linq;

namespace LyCilph.States
{
    public class GameState : State
    {
        private Texture2D point;
        private Texture2D circle;
        private SpriteFont font;
        private Song chomp;

        private UpdateManager update_manager;

        private Cell food;
        private Snake snake;
        private Controller controller;

        public GameState(StateManager state_manager, GraphicsDevice graphics_device) : base(state_manager)
        {
            point = new Texture2D(graphics_device, 1, 1);
            point.SetData(new Color[] { Color.White });

            update_manager = new UpdateManager();

            food = new Cell();
            snake = new Snake();
            SetPlayerController();
        }

        public void SetPlayerController()
        {
            controller = new PlayerController(food, snake);
        }

        public void SetSimpleController()
        {
            controller = new SimpleAIController(food, snake);
        }

        public void SetNeuralNetworkController(Chromosome chromosome)
        {
            controller = new NeuralNetworkController(food, snake, chromosome);
        }

        public override void Reset()
        {
            food.Random();
            snake.Reset();
            update_manager.Reset();
        }

        public override void LoadContent(ContentManager content)
        {
            circle = content.Load<Texture2D>("circle");
            font = content.Load<SpriteFont>("font");
            chomp = content.Load<Song>("chomp");
        }

        public override void HandleInput(InputManager input)
        {
            if (input.IsPressed(Keys.Back))
                state_manager.TransitionToMainMenuState();

            if (input.IsPressed(Keys.Add))
                update_manager.Inc();

            if (input.IsPressed(Keys.Subtract))
                update_manager.Dec();

            if (input.IsPressed(Keys.M))
                update_manager.Max();

            if (input.IsPressed(Keys.R))
                update_manager.Toggle();

            if (input.IsPressed(Keys.S))
                update_manager.Step();

            if (input.IsPressed(Keys.Escape))
                state_manager.TransitionToMainMenuState();

            controller.HandleInput(input);
        }

        public override void Update(GameTime game_time)
        {
            if (!update_manager.Update(game_time))
                return;

            snake.Move();

            // Did the snake eat the food?
            if (food.Hit(snake.Head))
            {
                MediaPlayer.Play(chomp);
                snake.HasEaten = true;
                CreateFood();
            }

            // Was the snake out of bounds?
            if (snake.Head.X < 0 || snake.Head.X >= Settings.board_size || snake.Head.Y < 0 || snake.Head.Y >= Settings.board_size)
            {
                snake.CauseOfDeath = "Hit border";
                state_manager.TransitionToGameOverState(snake);
            }

            // Did the snake hit itself?
            if (snake.HitSelf())
            {
                snake.CauseOfDeath = "Hit self";
                state_manager.TransitionToGameOverState(snake);
            }

            // Did the snake die of hunger?
            if (snake.Energy <= 0)
            {
                snake.CauseOfDeath = "Died of hunger";
                state_manager.TransitionToGameOverState(snake);
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            var board_size = Settings.board_width;
            var cell_size = Settings.cell_size;

            // Top border
            sprite_batch.Draw(point, new Rectangle(0, 0, board_size, 1), Color.Blue);
            // Bottom border
            sprite_batch.Draw(point, new Rectangle(0, board_size - 1, board_size, 1), Color.Blue);
            // Left border
            sprite_batch.Draw(point, new Rectangle(0, 0, 1, board_size), Color.Blue);
            // Right border
            sprite_batch.Draw(point, new Rectangle(board_size - 1, 0, 1, board_size), Color.Blue);

            // Food
            sprite_batch.Draw(circle, new Rectangle(food.X * cell_size, food.Y * cell_size, cell_size, cell_size), Color.Red);

            // Snake head
            sprite_batch.Draw(circle, new Rectangle(snake.Head.X * cell_size, snake.Head.Y * cell_size, cell_size, cell_size), Color.Black);
            // Draw body
            foreach (var body in snake.Body.Skip(1))
                sprite_batch.Draw(circle, new Rectangle(body.X * cell_size, body.Y * cell_size, cell_size, cell_size), Color.Gray);

            // Snake information
            sprite_batch.DrawString(font, $"Score: {snake.Score}", new Vector2(board_size + 5, 0), Color.Black);
            sprite_batch.DrawString(font, $"Age: {snake.Age}", new Vector2(board_size + 5, 20), Color.Black);
            sprite_batch.DrawString(font, $"Energy: {snake.Energy}", new Vector2(board_size + 5, 40), Color.Black);

            // Neural network information
            if (controller is NeuralNetworkController nn)
            {
                sprite_batch.DrawString(font, "Distance to edge", new Vector2(board_size + 20, 100), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[7]:N3} {nn.Input[0]:N3} {nn.Input[1]:N3}", new Vector2(board_size + 20, 120), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[6]:N3}   *   {nn.Input[2]:N3}", new Vector2(board_size + 20, 140), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[5]:N3} {nn.Input[4]:N3} {nn.Input[3]:N3}", new Vector2(board_size + 20, 160), Color.Black);

                sprite_batch.DrawString(font, "Distance to body", new Vector2(board_size + 20, 200), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[15]:N3} {nn.Input[8]:N3} {nn.Input[9]:N3}", new Vector2(board_size + 20, 220), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[14]:N3}   *   {nn.Input[10]:N3}", new Vector2(board_size + 20, 240), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[13]:N3} {nn.Input[12]:N3} {nn.Input[11]:N3}", new Vector2(board_size + 20, 260), Color.Black);

                sprite_batch.DrawString(font, "Found food", new Vector2(board_size + 20, 300), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[23]} {nn.Input[16]} {nn.Input[17]}", new Vector2(board_size + 20, 320), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[22]} * {nn.Input[18]}", new Vector2(board_size + 20, 340), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Input[21]} {nn.Input[20]} {nn.Input[19]}", new Vector2(board_size + 20, 360), Color.Black);

                sprite_batch.DrawString(font, "Direction", new Vector2(board_size + 20, 400), Color.Black);
                sprite_batch.DrawString(font, $"      {nn.Output[0]:N2}", new Vector2(board_size + 20, 420), Color.Black);
                sprite_batch.DrawString(font, $"{nn.Output[3]:N2} * {nn.Output[1]:N2}", new Vector2(board_size + 20, 440), Color.Black);
                sprite_batch.DrawString(font, $"      {nn.Output[2]:N2}", new Vector2(board_size + 20, 460), Color.Black);
            }

            // Speed information
            sprite_batch.DrawString(font, $"(R)un/pause", new Vector2(board_size + 5, board_size - 65), Color.Black);
            sprite_batch.DrawString(font, $"(S)tep", new Vector2(board_size + 5, board_size - 45), Color.Black);
            sprite_batch.DrawString(font, $"(+/-) Update: {update_manager.Interval:N0} - (M)ax speed", new Vector2(board_size + 5, board_size - 25), Color.Black);
        }

        private void CreateFood()
        {
            var retry = true;
            while (retry)
            {
                food.Random();
                retry = snake.Hit(food);
            }
        }
    }
}
