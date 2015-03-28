using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;


namespace Platformer
{
    class Projectile : Invertible
    {
        private double x_vel;
        private double y_vel;
        private bool alive;

        public Projectile(int x, int y, int width, int height, Texture2D normal, Texture2D inverted, double x_vel, double y_vel)
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
        }

        public void Update(List<Obstacle> oList, ref List<Enemy> eList)
        {
            spriteX += (int)x_vel;
            spriteY += (int)y_vel;
            checkObstacleCollisions(oList);
            checkEnemyCollisions(ref eList);
            checkBounds();
        }

        public void checkBounds()
        {
            if (spriteX > 800 || spriteY > 500)
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
            foreach (Enemy e in eList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    alive = false;
                    e.kill();
                }
            }
        }

        private void checkObstacleCollisions(List<Obstacle> oList)
        {
            foreach (Obstacle o in oList)
            {
                if (!(spriteX + spriteWidth < o.getX() || spriteX > o.getX() + o.getWidth() || spriteY + spriteHeight < o.getY() || spriteY > o.getY() + o.getHeight()))
                {
                    alive = false;
                }
            }
        }

    }
}