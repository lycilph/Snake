using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace LyCilph.States
{
    public class GameState : StateEntity
    {
        private Texture2D point;
        private Texture2D circle;
        private SpriteFont font;

        private Cell food;
        private Snake snake;

        public GameState(StateManager state_manager, GraphicsDevice graphics_device) : base(state_manager)
        {
            point = new Texture2D(graphics_device, 1, 1);
            point.SetData(new Color[] { Color.White });

            food = new Cell();
            snake = new Snake();
        }

        public override void LoadContent(ContentManager content)
        {
            circle = content.Load<Texture2D>("circle");
            font = content.Load<SpriteFont>("font");
        }

        public override void Reset()
        {
            food.Random();
            snake.Reset();
        }

        public override void Update(GameTime game_time)
        {
            
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
        }
    }
}
