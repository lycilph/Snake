using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LyCilph.Elements
{
    public class Board : GameElement
    {
        private Texture2D point;

        public Board(SnakeGame game) : base(game) { }

        public override void LoadContent()
        {
            point = new Texture2D(game.GraphicsDevice, 1, 1);
            point.SetData(new Color[] { Color.White });
        }

        public override void Draw(SpriteBatch sprite_batch)
        {
            var size = game.Settings.Size * game.Settings.CellSize;
            var margin = game.Margin;

            // Top border
            sprite_batch.Draw(point, new Rectangle(margin, margin, size, 1), Color.Blue);
            // Bottom border
            sprite_batch.Draw(point, new Rectangle(margin, size+margin, size, 1), Color.Blue);
            // Left border
            sprite_batch.Draw(point, new Rectangle(margin, margin, 1, size), Color.Blue);
            // Right border
            sprite_batch.Draw(point, new Rectangle(size+margin, margin, 1, size), Color.Blue);
        }
    }
}
