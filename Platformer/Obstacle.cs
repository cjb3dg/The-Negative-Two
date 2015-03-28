using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace Platformer
{
    class Obstacle : Invertible
    {
        public MovementPattern mPattern;

        public Obstacle(int x, int y, int width, int height, Texture2D normal, Texture2D inverted) //Stationary constructor
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.image = normal;
            this.image_i = inverted;

        }

        public Obstacle(int x, int y, int width, int height, Texture2D normal, Texture2D inverted, MovementPattern mPattern) //Moving Constructor
        {
            this.spriteX = x;
            this.spriteY = y;
            this.spriteWidth = width;
            this.spriteHeight = height;
            this.image = normal;
            this.image_i = inverted;
            this.mPattern = mPattern;

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
        public void LoadContent(ContentManager content, string filename1, string filename2)
        {
            {
                image = content.Load<Texture2D>(filename1);
                image_i = content.Load<Texture2D>(filename2);
            }

        }

        public void Draw(SpriteBatch sb, Player player1)
        {
            if (player1.IsInverted == false)
            {
                sb.Draw(image, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White); //Add array of sprites, choose which one to call
            }
            else
            {
                sb.Draw(image_i, new Rectangle(spriteX, spriteY, spriteWidth, spriteHeight), Color.White); //Add array of sprites, choose which one to call
            }
        }

        public void Update(Controls controls, GameTime gameTime)
        {

        }
    }
}
