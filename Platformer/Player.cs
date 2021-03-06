using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Audio;

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
        private int invCooldown;
        private int invulnerability;
        public int maxHP;
        public int curHP;
        public int maxEnergy;
        public int curEnergy;
        public int energyCost;
        public int energyRecover;
        private int floorHeight = 800;
        private bool moving = false;

        private static SoundEffect hitSound;
        private static SoundEffect shootSound;
        private static SoundEffect healSound;

        public Player(int x, int y, int width, int height, int health)
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;

            this.maxHP = 1000;
            this.curHP = health;
            this.maxEnergy = 300;
            this.curEnergy = 300;
            this.energyCost = 100;
            this.energyRecover = 1;
            this.cooldown = 0;
            this.invCooldown = 0;
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
                image = content.Load<Texture2D>("neggy_spritesheet.png");
                image_i = content.Load<Texture2D>("neggy_spritesheet_i.png");
                totalFrames = 2;

                hitSound = content.Load<SoundEffect>("Hit");
                shootSound = content.Load<SoundEffect>("Shoot");
                healSound = content.Load<SoundEffect>("heal");
            }
        }

        public void CheckDeath()
        {
            if (curHP <= 0 || spriteY > floorHeight)
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
            if (controls.onPress(Keys.Space, Buttons.LeftTrigger) && invCooldown <= 0)
            {
                inv.invert();
                invCooldown = 12;
            }
        }

        public int Move(Controls controls, List<Obstacle> oList, List<Enemy> eList, List<Boss> bList, List<Item> iList, List<Projectile> pList, Door door, bool cameraStill, int cameraX, InversionManager inv)
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
            checkEnemyCollisions(eList, inv);
            checkBossCollisions(bList, cameraStill);
            checkItemCollisions(iList);
            checkProjectileCollisions(pList);

            if (invulnerability > 0)
            {
                invulnerability--;
            }

            checkLevelSuccess(door);

            int diffX = spriteX - oldX;

            if (diffX != 0)
            {
                moving = true;
            }
            else
            {
                moving = false;
            }

            if (cameraStill)
            {
                if (spriteX < cameraX)
                {
                    spriteX = cameraX;
                }
                if (spriteX > cameraX + 1280 - spriteWidth)
                {
                    spriteX = cameraX + 1280 - spriteWidth;
                }
                return 0;
            }

            return diffX;
        }

        private void checkProjectileCollisions(List<Projectile> pList)
        {
            foreach (Projectile e in pList)
            {
                if (!e.IsFriendly() && !(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    if (invulnerability <= 0)
                    {
                        curHP -= 100;
                        invulnerability = 30;

                        hitSound.Play();
                    }
                }
            }
        }

        private void checkItemCollisions(List<Item> iList)
        {
            foreach (Item e in iList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    if (curHP < maxHP && e.getType() == 0)
                    {
                        curHP += 300;
                        if (curHP > maxHP)
                        {
                            curHP = maxHP;
                        }
                        e.remove();

                        healSound.Play();
                    }
                }
            }
        }

        private void checkEnemyCollisions(List<Enemy> eList, InversionManager inv)
        {
            foreach (Enemy e in eList)
            {
                if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                {
                    int healthDecrease = 100;
                    if (!e.IsActive(inv))
                    {
                        healthDecrease = 50;
                    }
                    if (invulnerability <= 0)
                    {
                        curHP -= healthDecrease;
                        invulnerability = 30;

                        hitSound.Play();
                    }
                }
            }
        }

        private void checkBossCollisions(List<Boss> bList, bool cameraStill)
        {
            if (cameraStill)
            {
                foreach (Boss e in bList)
                {
                    if (!(spriteX + spriteWidth < e.getX() || spriteX > e.getX() + e.getWidth() || spriteY + spriteHeight < e.getY() || spriteY > e.getY() + e.getHeight()))
                    {
                        if (invulnerability == 0)
                        {
                            curHP -= 150;
                            invulnerability = 30;

                            hitSound.Play();
                        }
                    }
                }
            }
        }

        private void checkObstacleCollisions(List<Obstacle> oList)
        {
            grounded = false;
            foreach (Obstacle o in oList)
            {
                if (!(spriteX + spriteWidth < o.getX() || spriteX > o.getX() + o.getWidth() || spriteY + spriteHeight < o.getY() || spriteY > o.getY() + o.getHeight()))
                {
                    if (spriteY + spriteHeight <= o.getY() + y_vel && y_vel >= 0 && spriteX + spriteWidth > o.getX() && spriteX < o.getX() + o.getWidth())
                    {
                        spriteY = o.getY() - spriteHeight;
                        y_vel = 0;
                        grounded = true;
                    }
                    if (spriteY >= o.getY() + o.getHeight() + y_vel && y_vel < 0 && spriteX + spriteWidth > o.getX() && spriteX < o.getX() + o.getWidth() && !grounded)
                    {
                        spriteY = o.getY() + o.getHeight();
                        y_vel = 0;
                    }
                    if (spriteX + spriteWidth <= o.getX() + x_vel && x_vel > 0 && spriteY + spriteHeight > o.getY() && spriteY < o.getY() + o.getHeight())
                    {
                        spriteX = o.getX() - spriteWidth;
                        x_vel = 0;
                    }
                    if (spriteX >= o.getX() + o.getWidth() + x_vel && x_vel < 0 && spriteY + spriteHeight > o.getY() && spriteY < o.getY() + o.getHeight())
                    {
                        spriteX = o.getX() + o.getWidth();
                        x_vel = 0;
                    }
                }
            }
        }

        public void checkLevelSuccess(Door door)
        {
            if (door.isActive() && !(spriteX + spriteWidth < door.getX() || spriteX > door.getX() + door.getWidth() ||
                spriteY + spriteHeight < door.getY() || spriteY > door.getY() + door.getHeight()))
            {
                victory = true;
            }
        }

        public bool Shoot(Controls controls)
        {
            if (controls.onPress(Keys.S, Buttons.RightTrigger) && cooldown == 0)
            {
                cooldown = 20;
                shootSound.Play();
                return true;
            }
            return false;
        }

        private void Jump(Controls controls, GameTime gameTime)
        {
            // Jump on button press
            if (controls.onPress(Keys.Up, Buttons.A) && grounded)
            {
                y_vel = -11;
                jumpPoint = (int)(gameTime.TotalGameTime.TotalMilliseconds);
                grounded = false;
            }
            // Cut jump short on button release
            else if (controls.onRelease(Keys.Up, Buttons.A) && y_vel < 0)
            {
                y_vel /= 2;
            }
        }

        public int Update(Controls controls, GameTime gameTime, List<Obstacle> oList, List<Enemy> eList, List<Boss> bList, List<Item> iList, List<Projectile> pList, InversionManager inv, Door door, bool cameraStill, int cameraX)
        {
            int retVal = Move(controls, oList, eList, bList, iList, pList, door, cameraStill, cameraX, inv);
            Jump(controls, gameTime);
            Invert(controls, inv);
            if (cooldown > 0)
            {
                cooldown--;
            }
            if (invCooldown > 0)
            {
                invCooldown--;
            }
            CheckDeath();
            UpdateAnimation(gameTime, moving, grounded);
            return retVal;
        }

        public override void Draw(SpriteBatch spriteBatch, int cameraX)
        {
            if (invulnerability > 20)
            {
                DrawAnimation(spriteBatch, !IsInverted, spriteX - cameraX, spriteY, !right);
            }
            else
            {
                DrawAnimation(spriteBatch, IsInverted, spriteX - cameraX, spriteY, !right);
            }
        }
    }
}