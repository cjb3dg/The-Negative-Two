﻿using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace The_Negative_One
{
    class Projectile : Invertible
    {
        private double x_vel;
        private double y_vel;
        private bool alive;
        private bool friendly;

        public Projectile(int x, int y, int width, int height, Texture2D normal, Texture2D inverted, double x_vel, double y_vel, bool friendly, int totalFrames, bool isInverted)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.image = normal;
            this.image_i = inverted;
            this.x_vel = x_vel;
            this.y_vel = y_vel;
            this.alive = true;
            this.friendly = friendly;
            this.totalFrames = totalFrames;
            this.IsInverted = isInverted;
        }

        public void Update(List<Obstacle> oList, ref List<Enemy> eList, ref List<Boss> bList, int cameraX, Player player, GameTime gametime)
        {
            spriteX += (int)x_vel;
            spriteY += (int)y_vel;
            checkObstacleCollisions(oList);
            //checkPlayerCollisions(player);
            checkEnemyCollisions(ref eList);
            checkBossCollisions(ref bList);
            checkBounds(cameraX);
            UpdateAnimation(gametime, true, true);
        }

        public override void Draw(SpriteBatch sb, int cameraX)
        {
            DrawAnimation(sb, IsInverted, spriteX - cameraX, spriteY, x_vel <= 0);
        }

        public void setX(int x)
        {
            spriteX = x;
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

        public bool IsFriendly()
        {
            return friendly;
        }

        public void checkBounds(int cameraX)
        {
            if (spriteX > 1280 + cameraX || spriteY > 800 || spriteX < cameraX || spriteY < -50)
            {
                alive = false;
            }
        }

        public bool isAlive()
        {
            return alive;
        }

        private void checkEnemyCollisions(ref List<Enemy> eList)
        {
            if (friendly == false)
            {
                return;
            }
            foreach (Enemy e in eList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    e.curHP--;
                    if (e.curHP == 0)
                    {
                        e.kill();
                    }
                    alive = false;
                }
            }
        }

        private void checkBossCollisions(ref List<Boss> bList)
        {
            if (friendly == false)
            {
                return;
            }
            foreach (Boss e in bList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    alive = false;
                    e.damage(1);
                    if (e.curHP <= 0)
                    {
                        e.kill();
                    }
                }
            }
        }

        private void checkPlayerCollisions(Player e)
        {
            if (friendly == true)
            {
                return;
            }
            else if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
            {
                alive = false;
                e.curHP-= 100;
                if (e.curHP <= 0)
                {
                    e.CheckDeath();
                }
            }
        }

        private void checkObstacleCollisions(List<Obstacle> oList)
        {
            foreach (Obstacle o in oList)
            {
                if (!(spriteX + spriteWidth < o.getX() + 2 || spriteX > o.getX() + o.getWidth() - 2 || spriteY + spriteHeight < o.getY() + 2 || spriteY > o.getY() + o.getHeight() - 2))
                {
                    alive = false;
                }
            }
        }

    }
}