using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;
namespace The_Negative_One
{
	abstract class Sprite
	{
		protected int spriteX, spriteY;
		protected int spriteWidth, spriteHeight;
		protected Texture2D image;

		public Sprite()
		{
		}

        public virtual void Draw(SpriteBatch sb, int cameraX)
        {
            sb.Draw(image, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
        }
	}
}

