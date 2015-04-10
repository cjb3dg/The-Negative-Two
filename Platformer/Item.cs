using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Negative_One
{
    class Item : Invertible
    {
        private int type;
        private bool alive;

        public Item(int x, int y, int width, int height, int type, Texture2D normal, Texture2D inverted)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.image = normal;
            this.image_i = inverted;

            this.type = type;
            this.alive = true;

            this.IsInverted = false;
        }

        public int getX()
        {
            return spriteX;
        }
        public int getY()
        {
            return spriteY;
        }
        public void setX(int x)
        {
            spriteX = x;
        }
        public void setY(int y)
        {
            spriteY = y;
        }
        public int getWidth()
        {
            return spriteWidth;
        }
        public int getHeight()
        {
            return spriteHeight;
        }

        public int getType()
        {
            return type;
        }

        public bool isAlive()
        {
            return alive;
        }

        public void remove()
        {
            alive = false;
        }

        public void Update()
        {

        }

    }
}
