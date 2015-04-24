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
	abstract class Sprite
	{
		protected int spriteX, spriteY;
		protected int spriteWidth, spriteHeight;
        protected Texture2D image;
        protected Texture2D image_i;

        // the spritesheet containing our animation frames
        //protected Texture2D spriteSheet;
        //protected Texture2D spriteSheet_i;
        // the elapsed amount of time the frame has been shown for
        float time;
        // duration of time to show each frame
        protected float frameTime = 0.15f;
        // an index of the current frame being shown
        protected int frameIndex=0;
        // total number of frames in our spritesheet
        protected int totalFrames = 1; // default is 1

        // define the size of our animation frame - use SpriteWidth, SpriteHeight instead
        //protected int frameHeight;
        //protected int frameWidth;

		public Sprite()
		{
		}

        public virtual void Draw(SpriteBatch sb, int cameraX)
        {
            sb.Draw(image, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
            //Rectangle source = new Rectangle(frameIndex * spriteX, 0, spriteX, spriteY);
            //sb.Draw(image, new Vector2(spriteX - cameraX, spriteY), source, Color.White, 0.0f, new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
        }

        public virtual void UpdateAnimation(GameTime gameTime, bool moving, bool allowAnimate)
        {
            if (totalFrames > 1)
            {
                // Process elapsed time
                time += (float)gameTime.ElapsedGameTime.TotalSeconds;
                while (time > frameTime)
                {
                    // Play the next frame in the SpriteSheet
                    if (moving && allowAnimate)
                    {
                        frameIndex++;
                    }

                    // reset elapsed time
                    time = 0f;
                }
                if (frameIndex > totalFrames - 1) frameIndex = 0;
            }
        }

        /// Draws the sprite on the screen
        public void DrawAnimation(SpriteBatch spriteBatch, bool isInverted, float posX, float posY, bool left)
        {
            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(frameIndex * spriteWidth, 0, spriteWidth, spriteHeight);
            // Calculate position and origin to draw in the center of the screen
            Vector2 position = new Vector2(posX, posY);
            Vector2 origin = new Vector2(0, 0);

            if (isInverted)
            {
                if (left)
                {
                    spriteBatch.Draw(image_i, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(image_i, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
                }
            }
            else
            {
                if (left)
                {
                    spriteBatch.Draw(image, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(image, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
                }
            }
        }
	}
}

