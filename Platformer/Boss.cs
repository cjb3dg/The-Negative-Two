using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Negative_One
{
    class Boss : Invertible
    {
        public List<MovementPattern> mPList; //A Boss has a list of possible movement patterns it can switch between on the fly
        public MovementPattern curMPattern;
        public int maxHP;
        public int curHP;
        public double xVel;
        public double yVel;
        public int mIter = 0; //Iterator for curMPattern
        public int mLIter = 0; //Iterator for mPList
        private bool alive;

        public int state = 1;

        private int targetX;
        private int targetY;

        private double delay = .5;
        private double remDelay = .5;

        private double projDelay = 2;
        private double remPDelay = 2;

        public Boss(int x, int y, int width, int height, Texture2D normal, Texture2D inverted, int maxHP, List<MovementPattern> mPList, Player player1) //Moving Constructor
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.image = normal;
            this.image_i = inverted;
            this.maxHP = maxHP;
            this.curHP = maxHP;
            this.mPList = mPList;
            this.remDelay = .5;
            this.alive = true;

            this.curMPattern = mPList[0];

            this.targetX = player1.getX() + player1.getWidth() / 2;
            this.targetY = player1.getY() + player1.getHeight() / 2;

        }

        public void damage(int damage)
        {
            this.curHP -= damage;
            this.curHP = Math.Max(curHP, 0);
        }

        public void UpdateTarget(Player player1)
        {
            this.targetX = player1.getX() + player1.getWidth() / 2;
            this.targetY = player1.getY() + player1.getHeight() / 2;
        }

        public void UpdateState()
        {
            if ((curHP < maxHP / 2) && (state != 2))
            {
                this.state = 2;
                this.delay /= 2;
                this.remDelay /= 2;
            }
        }

        public void Update(Microsoft.Xna.Framework.GameTime gameTime, Player player1)
        {
            UpdateTarget(player1);
            UpdateState();

            this.remDelay -= gameTime.ElapsedGameTime.TotalSeconds;
            this.remPDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (remDelay <= 0)
            {
                if (this.mIter + 1 > this.curMPattern.xVList.Count)
                {
                    this.mIter = 0;
                    this.mLIter = (this.mLIter + 1) % this.mPList.Count;
                    this.xVel = 0;
                    this.yVel = 0;
                    this.curMPattern = this.mPList[mLIter];

                }

                else
                {
                    this.xVel = this.curMPattern.xVList[this.mIter];
                    this.yVel = this.curMPattern.yVList[this.mIter];
                    this.mIter = this.mIter + 1;

                }

                remDelay = delay;
            }
            this.spriteX += state * Convert.ToInt32(xVel);
            this.spriteY += state * Convert.ToInt32(yVel);

            if (remPDelay <= 0)
            {
                //Create Projectile here
                remPDelay = projDelay;
            }
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

        public int getState()
        {
            return state;
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

        public void setState(int state)
        {
            this.state = state;
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
