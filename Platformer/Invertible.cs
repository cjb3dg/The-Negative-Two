using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Platformer
{
    /* All non-neutral objects */
    abstract class Invertible : Sprite
    {
        public bool IsInverted = false; /* inverted = of inverse world */

        public bool IsNeutral = false; /* TODO */

        protected bool allowInversion = true; /* should be false for objects without dual-states */

        protected Texture2D image_i;

        public override void Draw(SpriteBatch sb)
        {
            if (!IsInverted)
            {
                sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
            }
            else
            {
                sb.Draw(image_i, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
            }
        }

        public void invert()
        {
            if (allowInversion && !IsNeutral)
            {
                IsInverted = !IsInverted;
            }
        }

        public void setNeutral()
        {
            IsNeutral = true;
        }

        /* prevents inversion */
        public void lockInversion()
        {
            allowInversion = false;
        }

        public void unlockInversion()
        {
            allowInversion = true;
        }

        public bool IsActive(InversionManager im)
        {
            if (IsInverted != im.IsWorldInverted)
            {
                return true;
            }
            return false;
        }
    }
}
