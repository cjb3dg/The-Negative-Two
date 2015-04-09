using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace The_Negative_One
{
    class Player : Invertible
    {
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
        private int invulnerability;
        public int maxHP;
        public int curHP;
        public int maxEnergy;
        public int curEnergy;
        public int energyCost;
        public int energyRecover;
        private int floorHeight = 480;

        public Player(int x, int y, int width, int height)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;

            this.maxHP = 1000;
            this.curHP = 1000;
            this.maxEnergy = 300;
            this.curEnergy = 300;
            this.energyCost = 100;
            this.energyRecover = 1;
            this.cooldown = 0;
            this.invulnerability = 0;

            floorHeight -= this.spriteHeight;

            grounded = false;
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

        public int getEnergy()
        {
            return curEnergy;
        }

        public void setEnergy(int en)
        {
            curEnergy = en;
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

        public int Update(Controls controls, GameTime gameTime, List<Obstacle> oList, List<Enemy> eList, List<Boss> bList, List<Item> iList, InversionManager inv, Door door, bool cameraStill)
        {
            int retVal = Move(controls, oList, eList, bList, iList, door, cameraStill);
            Jump(controls, gameTime);
            Invert(controls, inv);
            if (cooldown > 0)
            {
                cooldown--;
            }
            CheckDeath();
            return retVal;
        }

        public void CheckDeath()
        {
            if (curHP <= 0)
            {
                alive = false;
            }
        }

        public void setAlive(bool a)
        {
            alive = a;
        }

        public void Invert(Controls controls, InversionManager inv)
        {
            if (controls.onPress(Keys.Space, Buttons.LeftTrigger) && curEnergy >= energyCost)
            {
                inv.invert();
                curEnergy -= energyCost;
            }
            if (curEnergy < maxEnergy)
            {
                curEnergy += energyRecover;
            }
            else
            {
                curEnergy = maxEnergy;
            }
        }

        public int Move(Controls controls, List<Obstacle> oList, List<Enemy> eList, List<Boss> bList, List<Item> iList, Door door, bool cameraStill)
        {
            int oldX = spriteX;

            // Sideways Acceleration
            bool rightPressed = controls.isPressed(Keys.Right, Buttons.LeftThumbstickRight);
            bool leftPressed = controls.isPressed(Keys.Left, Buttons.LeftThumbstickLeft);
            if ( rightPressed && leftPressed )
            {
                x_accel = 0;
            }
            else if (rightPressed)
            {
                x_accel = speed;
                right = true;
            }
            else if (leftPressed)
            {
                x_accel = -1 * speed;
                right = false;
            }
            else
                x_accel = 0;

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
            checkBossCollisions(bList);
            checkItemCollisions(iList);
            checkLevelSuccess(door);

            if (cameraStill)
            {
                return 0;
            }
            int diffX = spriteX - oldX;
            return diffX;
        }

        private void checkItemCollisions(List<Item> iList)
        {
            foreach (Item e in iList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    if (curHP < maxHP && e.getType() == 0)
                    {
                        curHP += 200;
                        if (curHP > maxHP)
                        {
                            curHP = maxHP;
                        }
                        e.remove();
                    }
                }
            }
        }

        private void checkEnemyCollisions(List<Enemy> eList)
        {
            foreach (Enemy e in eList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    if (invulnerability <= 0)
                    {
                        curHP -= 50;
                        invulnerability = 30;
                    }
                }
            }
            if (invulnerability > 0)
            {
                invulnerability--;
            }
        }

        private void checkBossCollisions(List<Boss> bList)
        {
            foreach (Boss e in bList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    if (invulnerability == 0)
                    {
                        curHP -= 100;
                        invulnerability = 30;
                    }
                    if (invulnerability > 0)
                    {
                        invulnerability--;
                    }
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

        public void checkLevelSuccess(Door door)
        {
            if (!(spriteX + spriteWidth < door.getX() || spriteX > door.getX() + door.getWidth() ||
                spriteY + spriteHeight < door.getY() || spriteY > door.getY() + door.getHeight()))
            {
                victory = true;
            }
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
            if (controls.onPress(Keys.Up, Buttons.A) && grounded)
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

        public override void Draw(SpriteBatch spriteBatch, int cameraX)
        {
            if (right)
            {
                if (!IsInverted)
                {
                    //spriteBatch.Draw(image, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
                    if (invulnerability > 20)
                    {
                        spriteBatch.Draw(image_i, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(image, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
                    }
                }
                else
                {
                    //spriteBatch.Draw(image_i, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
                    if (invulnerability > 20)
                    {
                        spriteBatch.Draw(image, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
                    }
                    else
                    {
                        spriteBatch.Draw(image_i, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
                    }
                }
            }
            else
            {
                if (!IsInverted)
                {
                    //spriteBatch.Draw(image, null, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                    if (invulnerability > 20)
                    {
                        spriteBatch.Draw(image_i, null, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(image, null, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                    }
                }
                else
                {
                    //spriteBatch.Draw(image_i, null, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                    if (invulnerability > 20)
                    {
                        spriteBatch.Draw(image, null, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                    }
                    else
                    {
                        spriteBatch.Draw(image_i, null, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), null, null, 0, null, Color.White, SpriteEffects.FlipHorizontally, 0);
                    }
                }
            }
        }
    }
}