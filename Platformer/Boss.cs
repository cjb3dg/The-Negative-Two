using Microsoft.Xna.Framework.Content;
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

        public bool active;

        public int state = 1;

        private int targetX;
        private int targetY;

        private double delay = .5;
        private double remDelay = .5;

        private double projDelay = 2;
        private double remPDelay = 2;

        private int cooldown = 50;

        public Boss(int x, int y, int width, int height, Texture2D image, Texture2D image_i, int totalFrames, int maxHP, List<MovementPattern> mPList, Player player1, bool inv, bool neutral, bool active) //Moving Constructor
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.image = image;
            this.image_i = image_i;
            this.totalFrames = totalFrames;
            this.maxHP = maxHP;
            this.curHP = maxHP;
            this.mPList = mPList;
            this.remDelay = .5;
            this.alive = true;

            this.IsInverted = inv;
            this.IsNeutral = neutral;
            this.active = active;

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

        public void Update(Microsoft.Xna.Framework.GameTime gameTime, Player player1, List<Enemy> eList, ContentManager content)
        {
            UpdateTarget(player1);
            UpdateState();
            this.cooldown--;
            this.remDelay -= gameTime.ElapsedGameTime.TotalSeconds;
            this.remPDelay -= gameTime.ElapsedGameTime.TotalSeconds;

            if (remDelay <= 0)
            {
                if (this.mIter + 1 > this.curMPattern.xVList.Count)
                {
                    //spawnSpider(eList, content);

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

        public void spawnSpider(List<Enemy> eList, ContentManager content)
        {
            List<double> xVList = new List<double>();
            List<double> yVList = new List<double>();
            xVList.Add(-2);
            xVList.Add(-2);
            yVList.Add(1);
            yVList.Add(-1);

            MovementPattern spiderPattern = new MovementPattern(xVList, yVList);
            Enemy spider = new Enemy(this.getX(), this.getY(), 24, 22, content.Load<Texture2D>("smallSpider"), content.Load<Texture2D>("smallSpider_i"), 1, 1, spiderPattern);
            eList.Add(spider);
        }

        public bool Shoot(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (cooldown == 0)
            {
                cooldown = 50;
                return true;
            }
            return false;
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
