using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class PlayingGameState : GameElement
    {
        private SpriteFont font;

        private Board board;
        private Food food;
        private Brain brain;
        private NeuralNetworkBrain nn_brain;

        private double update_delta;
        private double update_delta_threshold = 100;
        private double update_delta_step_size = 5;

        private bool running = true;
        private bool step = false;

        public Snake Snake { get; private set; }

        public PlayingGameState(SnakeGame game) : base(game)
        {
            board = new Board(game, 10, 60);
            food = new Food(game, board);
            Snake = new Snake(game, board);
            brain = new SimpleBrain(board, food, Snake);
            nn_brain = new NeuralNetworkBrain(board, food, Snake);
        }

        public override void LoadContent()
        {
            font = game.Content.Load<SpriteFont>("font");

            board.LoadContent();
            food.LoadContent();
            Snake.LoadContent();
        }

        public override void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            board.UpdateInput(new_state, old_state);

            if (old_state.IsKeyUp(Keys.Add) && new_state.IsKeyDown(Keys.Add))
            {
                if (update_delta_threshold > update_delta_step_size)
                    update_delta_threshold = Math.Max(update_delta_step_size, update_delta_threshold - update_delta_step_size);
                else
                    update_delta_threshold = Math.Max(1, update_delta_threshold - 1);
            }

            if (old_state.IsKeyUp(Keys.Subtract) && new_state.IsKeyDown(Keys.Subtract))
            {
                if (update_delta_threshold < update_delta_step_size)
                    update_delta_threshold = update_delta_step_size;
                else
                    update_delta_threshold += update_delta_step_size;
            }

            if (old_state.IsKeyUp(Keys.R) && new_state.IsKeyDown(Keys.R))
                running = !running;

            if (old_state.IsKeyUp(Keys.S) && new_state.IsKeyDown(Keys.S))
            {
                step = true;
                update_delta += update_delta_threshold; // This is to make sure an update is actually performed
            }

            brain.React(new_state, old_state);
            nn_brain.React(new_state, old_state);
        }

        public override void UpdateLogic(GameTime game_time)
        {
            if (!running && !step)
                return;
            step = false;

            Console.WriteLine($"Delta: {update_delta}, Ticks: {game_time.ElapsedGameTime.Ticks}");

            update_delta += game_time.ElapsedGameTime.TotalMilliseconds;
            if (update_delta < update_delta_threshold)
                return;
            update_delta = 0;

            Snake.Move();

            // Was the snake out of bounds?
            if (Snake.Head.X < 0 || Snake.Head.X >= board.BoardSizeInCells || Snake.Head.Y < 0 || Snake.Head.Y >= board.BoardSizeInCells)
            {
                Snake.CauseOfDeath = "Hit border";
                game.TransitionToGameOverState();
            }

            // Did the snake hit itself?
            if (Snake.HitSelf())
            {
                Snake.CauseOfDeath = "Hit self";
                game.TransitionToGameOverState();
            }

            // Did the snake eat the food?
            if (food.Hit(Snake.Head))
            {
                Snake.HasEaten = true;
                food.Random();
            }

            // Did the snake die of hunger?
            if (Snake.Energy <= 0)
            {
                Snake.CauseOfDeath = "Died of hunger";
                game.TransitionToGameOverState();
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            board.Draw(sprite_batch);
            food.Draw(sprite_batch);
            Snake.Draw(sprite_batch);

            sprite_batch.DrawString(font, $"Score: {Snake.Score}", new Vector2(board.BoardSizeInPixels + 5, 5), Color.Black);
            sprite_batch.DrawString(font, $"Age: {Snake.Age}", new Vector2(board.BoardSizeInPixels + 5, 25), Color.Black);
            sprite_batch.DrawString(font, $"Energy: {Snake.Energy}", new Vector2(board.BoardSizeInPixels + 5, 45), Color.Black);

            sprite_batch.DrawString(font, "Distance to edge", new Vector2(board.BoardSizeInPixels + 20, 100), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[7]:N3} {nn_brain.Input[0]:N3} {nn_brain.Input[1]:N3}", new Vector2(board.BoardSizeInPixels + 20, 120), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[6]:N3}   *   {nn_brain.Input[2]:N3}", new Vector2(board.BoardSizeInPixels + 20, 140), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[5]:N3} {nn_brain.Input[4]:N3} {nn_brain.Input[3]:N3}", new Vector2(board.BoardSizeInPixels + 20, 160), Color.Black);

            sprite_batch.DrawString(font, "Distance to body", new Vector2(board.BoardSizeInPixels + 20, 200), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[15]:N3} {nn_brain.Input[8]:N3} {nn_brain.Input[9]:N3}", new Vector2(board.BoardSizeInPixels + 20, 220), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[14]:N3}   *   {nn_brain.Input[10]:N3}", new Vector2(board.BoardSizeInPixels + 20, 240), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[13]:N3} {nn_brain.Input[12]:N3} {nn_brain.Input[11]:N3}", new Vector2(board.BoardSizeInPixels + 20, 260), Color.Black);

            sprite_batch.DrawString(font, "Found food", new Vector2(board.BoardSizeInPixels + 20, 300), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[23]} {nn_brain.Input[16]} {nn_brain.Input[17]}", new Vector2(board.BoardSizeInPixels + 20, 320), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[22]} * {nn_brain.Input[18]}", new Vector2(board.BoardSizeInPixels + 20, 340), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Input[21]} {nn_brain.Input[20]} {nn_brain.Input[19]}", new Vector2(board.BoardSizeInPixels + 20, 360), Color.Black);

            sprite_batch.DrawString(font, "Direction", new Vector2(board.BoardSizeInPixels + 20, 400), Color.Black);
            sprite_batch.DrawString(font, $"      {nn_brain.Output[0]:N2}", new Vector2(board.BoardSizeInPixels + 20, 420), Color.Black);
            sprite_batch.DrawString(font, $"{nn_brain.Output[3]:N2} * {nn_brain.Output[1]:N2}", new Vector2(board.BoardSizeInPixels + 20, 440), Color.Black);
            sprite_batch.DrawString(font, $"      {nn_brain.Output[2]:N2}", new Vector2(board.BoardSizeInPixels + 20, 460), Color.Black);

            sprite_batch.DrawString(font, $"(R)un/pause", new Vector2(board.BoardSizeInPixels + 5, board.BoardSizeInPixels - 65), Color.Black);
            sprite_batch.DrawString(font, $"(S)tep", new Vector2(board.BoardSizeInPixels + 5, board.BoardSizeInPixels - 45), Color.Black);
            sprite_batch.DrawString(font, $"(+/-) Update: {update_delta_threshold}", new Vector2(board.BoardSizeInPixels + 5, board.BoardSizeInPixels - 25), Color.Black);
        }

        public void Reset()
        {
            food.Random();
            Snake.Reset();
            brain.Reset();

            running = true;
            step = false;
        }
    }
}
