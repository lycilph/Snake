using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Snake
{
    public class Board : GameElement
    {
        private Texture2D point;

        public bool ShowBorder { get; set; }
        public bool ShowGrid { get; set; }
        public int CellSizeInPixels { get; set; }
        public int BoardSizeInCells { get; set; }
        public int BoardSizeInPixels { get { return CellSizeInPixels * BoardSizeInCells; } }
        public int Center { get { return BoardSizeInCells / 2; } }

        public Board(SnakeGame game, int cell_size_in_pixels, int board_size_in_cells) : base(game)
        {
            CellSizeInPixels = cell_size_in_pixels;
            BoardSizeInCells = board_size_in_cells;
            ShowBorder = true;
            ShowGrid = false;
        }

        public override void LoadContent()
        {
            point = new Texture2D(game.GraphicsDevice, 1, 1);
            point.SetData<Color>(new Color[] { Color.White });
        }

        public override void UpdateInput(KeyboardState new_state, KeyboardState old_state)
        {
            if (old_state.IsKeyUp(Keys.B) && new_state.IsKeyDown(Keys.B))
            {
                ShowBorder = !ShowBorder;
                ShowGrid = (ShowBorder ? ShowGrid : false);
            }

            if (old_state.IsKeyUp(Keys.G) && new_state.IsKeyDown(Keys.G))
            {
                ShowGrid = !ShowGrid;
                ShowBorder = (ShowGrid ? true : ShowBorder);
            }
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            if (ShowBorder)
            {
                // Top border
                sprite_batch.Draw(point, new Rectangle(0, 0, BoardSizeInPixels, 1), Color.Blue);
                // Bottom border
                sprite_batch.Draw(point, new Rectangle(0, BoardSizeInPixels - 1, BoardSizeInPixels, 1), Color.Blue);
                // Left border
                sprite_batch.Draw(point, new Rectangle(0, 0, 1, BoardSizeInPixels), Color.Blue);
                // Right border
                sprite_batch.Draw(point, new Rectangle(BoardSizeInPixels - 1, 0, 1, BoardSizeInPixels), Color.Blue);
            }

            if (!ShowGrid)
                return;

            for (int i = 1; i < BoardSizeInCells; i++)
                sprite_batch.Draw(point, new Rectangle(0, i * CellSizeInPixels - 1, BoardSizeInPixels, 1), Color.Blue);

            for (int i = 1; i < BoardSizeInCells; i++)
                sprite_batch.Draw(point, new Rectangle(i * CellSizeInPixels - 1, 0, 1, BoardSizeInPixels), Color.Blue);
        }
    }
}
