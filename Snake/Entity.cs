using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LyCilph
{
    public abstract class Entity
    {
        public virtual void Update(GameTime game_time) { }
        public virtual void Draw(SpriteBatch sprite_batch) { }
    }
}
