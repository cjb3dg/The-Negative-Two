﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    class Enemy : Invertible
    {
        public int curHP;
        public MovementPattern mPattern;
        public double xVel;
        public double yVel;
        public int mIter;
        private bool alive;

        public const double delay = .5;
        public double remainingDelay;

        public Enemy(int x, int y, int width, int height, Texture2D normal, Texture2D inverted, int curHP, MovementPattern mPattern) //Moving Constructor
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.image = normal;
            this.image_i = inverted;
            this.curHP = curHP;
            this.mPattern = mPattern;
            this.remainingDelay = .5;
            this.alive = true;
        }

        public Enemy(Enemy e) //Copy Constructor
        {
            this.spriteX = e.spriteX;
            this.spriteY = e.spriteY;
            this.spriteWidth = e.spriteWidth;
            this.spriteHeight = e.spriteHeight;
            this.image = e.image;
            this.image_i = e.image_i;
            this.curHP = e.curHP;
            this.mPattern = e.mPattern;
            this.alive = e.alive;
        }


        public void Update(Microsoft.Xna.Framework.GameTime gameTime, List<Obstacle> oList)
        {
            this.remainingDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (remainingDelay <= 0)
            {

                this.mIter = (this.mIter + 1) % this.mPattern.xVList.Count;
                this.xVel = this.mPattern.xVList.ToArray()[this.mIter];
                this.yVel = this.mPattern.yVList.ToArray()[this.mIter];

                remainingDelay = delay;
            }
            this.spriteX += Convert.ToInt32(xVel);
            this.spriteY += Convert.ToInt32(yVel);
        }

        public int getX()
        {
            return spriteX;
        }

        public int getY()
        {
            return spriteY;
        }

        public int getWidth()
        {
            return spriteWidth;
        }

        public int getHeight()
        {
            return spriteHeight;
        }

        public void setX(int x)
        {
            spriteX = x;
        }

        public void setY(int y)
        {
            spriteY = y;
        }

        public void setWidth(int w)
        {
            spriteWidth = w;
        }

        public void setHeight(int h)
        {
            spriteHeight = h;
        }

        public void kill()
        {
            alive = false;
        }

        public bool isAlive()
        {
            return alive;
        }
    }
}