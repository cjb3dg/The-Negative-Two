using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Platformer
{
    class Player : Invertible
    {
        private bool moving;
        private bool grounded;
        private int speed;
        private int x_accel;
        private double friction;
        public double x_vel;
        public double y_vel;
        public int movedX;
        private bool pushing;
        public double gravity = .5;
        public int maxFallSpeed = 10;
        private int jumpPoint = 0;
        public bool victory = false;
        private bool alive = true;
        private bool right = true;
        private int cooldown;
        public int maxHP;
        public int curHP;
        private int floorHeight = 480;

        public Player(int x, int y, int width, int height)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;

            this.maxHP = 1000;
            this.curHP = 1000;
            this.cooldown = 0;

            floorHeight -= this.spriteHeight;

            grounded = false;
            moving = false;
            pushing = false;

            // Movement
            speed = 5;
            friction = .15;
            x_accel = 0;
            x_vel = 0;
            y_vel = 0;
            movedX = 0;
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
        public void setWidth(int w)
        {
            spriteWidth = w;
        }
        public void setHeight(int h)
        {
            spriteHeight = h;
        }

        public int getHP()
        {
            return curHP;
        }

        public void setHP(int hp)
        {
            curHP = hp;
        }

        public int getCooldown()
        {
            return cooldown;
        }

        public void setCooldown(int cd)
        {
            cooldown = cd;
        }

        public bool isAlive()
        {
            return alive;
        }

        public bool facingRight()
        {
            return right;
        }

        public void LoadContent(ContentManager content)
        {
            {
                image = content.Load<Texture2D>("Zero.png");
                image_i = content.Load<Texture2D>("Zero_i.png");
            }

        }

        public void Update(Controls controls, GameTime gameTime, List<Obstacle> oList, List<Enemy> eList)
        {
            Move(controls, oList, eList);
            Jump(controls, gameTime);
            if (cooldown > 0)
            {
                cooldown--;
            }
            CheckDeath();
        }

        public void CheckDeath()
        {
            if (curHP <= 0 || spriteY > 450)
            {
                alive = false;
            }
        }
        public void Move(Controls controls, List<Obstacle> oList, List<Enemy> eList)
        {
            if (victory == true)
            {
                return;
            }

            // Sideways Acceleration
            if (controls.onPress(Keys.Right, Buttons.DPadRight))
            {
                x_accel += speed;
                right = true;
            }
            else if (controls.onRelease(Keys.Right, Buttons.DPadRight))
                x_accel -= speed;
            if (controls.onPress(Keys.Left, Buttons.DPadLeft))
            {
                x_accel -= speed;
                right = false;
            }
            else if (controls.onRelease(Keys.Left, Buttons.DPadLeft))
                x_accel += speed;

            double playerFriction = pushing ? (friction * 3) : friction;
            x_vel = x_vel * (1 - playerFriction) + x_accel * .10;
            movedX = Convert.ToInt32(x_vel);
            spriteX += movedX;

            // Gravity
            if (!grounded)
            {
                y_vel += gravity;
                if (y_vel > maxFallSpeed)
                    y_vel = maxFallSpeed;
                spriteY += Convert.ToInt32(y_vel);
            }
            else
            {
                y_vel = 0;
            }

            // Check up/down collisions, then left/right
            checkObstacleCollisions(oList);
            checkEnemyCollisions(eList);

        }

        private void checkEnemyCollisions(List<Enemy> eList)
        {
            foreach (Enemy e in eList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    curHP -= 10;
                }
            }
        }

        private void checkObstacleCollisions(List<Obstacle> oList)
        {
            foreach (Obstacle o in oList)
            {
                if (!(spriteX + spriteWidth < o.getX() || spriteX > o.getX() + o.getWidth() || spriteY + spriteHeight < o.getY() || spriteY > o.getY() + o.getHeight()))
                {
                    if (spriteY + spriteHeight <= o.getY() + y_vel && y_vel >= 0 && spriteX + spriteWidth > o.getX() && spriteX < o.getX() + o.getWidth())
                    {
                        spriteY = o.getY() - spriteHeight;
                        grounded = true;
                        return;
                    }
                    if (spriteY >= o.getY() + o.getHeight() + y_vel && y_vel < 0 && spriteX + spriteWidth > o.getX() && spriteX < o.getX() + o.getWidth())
                    {
                        spriteY = o.getY() + o.getHeight();
                        y_vel = 0;
                    }
                    if (spriteX + spriteWidth <= o.getX() + x_vel && x_vel > 0)
                    {
                        spriteX = o.getX() - spriteWidth;
                        x_vel = 0;
                    }
                    if (spriteX >= o.getX() + o.getWidth() + x_vel && x_vel < 0)
                    {
                        spriteX = o.getX() + o.getWidth();
                        x_vel = 0;
                    }
                }
            }
            if (spriteY >= floorHeight)
            {
                spriteY = floorHeight;
                grounded = true;
                return;
            }
            grounded = false;
        }

        public bool Shoot(Controls controls)
        {
            if (controls.onPress(Keys.Enter, Buttons.RightTrigger) && cooldown == 0)
            {
                cooldown = 20;
                return true;
            }
            return false;
        }

        private void Jump(Controls controls, GameTime gameTime)
        {
            // Jump on button press
            if (controls.onPress(Keys.Up, Buttons.DPadUp) && grounded)
            {
                y_vel = -13;
                jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                grounded = false;
            }
            // Cut jump short on button release
            else if (controls.onRelease(Keys.Up, Buttons.A) && y_vel < 0)
            {
                y_vel /= 2;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (right)
            {
                if (!IsInverted)
                {
                    spriteBatch.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
                }
                else
                {
                    spriteBatch.Draw(image_i, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White);
                }
            }
            else
            {
                if (!IsInverted)
                {
                    spriteBatch.Draw(image, null, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                }
                else
                {
                    spriteBatch.Draw(image_i, null, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                }
            }
        }
    }
}