using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace The_Negative_One
{
    /* All non-neutral objects */
    abstract class Invertible : Sprite
    {
        public bool IsInverted = false; /* inverted = of inverse world */

        public bool IsNeutral = false; /* TODO */

        protected bool allowInversion = true; /* should be false for objects without dual-states */

        public override void Draw(SpriteBatch sb, int cameraX)
        {
            DrawAnimation(sb, IsInverted, spriteX - cameraX, spriteY, false);
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
            if (IsInverted != im.IsWorldInverted || IsNeutral)
            {
                return true;
            }
            return false;
        }
    }
}
