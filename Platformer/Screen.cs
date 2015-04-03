using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    abstract class Screen
    {
        public string Type;
        protected ContentManager content;
        public virtual void LoadContent(ContentManager contentManager) { }
        public virtual void Update(PlatformerMain game) { }
        public virtual void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice) { }
    }
}
