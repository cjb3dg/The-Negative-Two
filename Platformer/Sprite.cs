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

        // the spritesheet containing our animation frames
        protected Texture2D spriteSheet;
        protected Texture2D spriteSheet_i;
        // the elapsed amount of time the frame has been shown for
        float time;
        // duration of time to show each frame
        float frameTime = 0.15f;
        // an index of the current frame being shown
        int frameIndex=0;
        // total number of frames in our spritesheet
        const int totalFrames = 2;
        // define the size of our animation frame
        protected int frameHeight;
        protected int frameWidth;

		public Sprite()
		{
		}

        public virtual void Draw(SpriteBatch sb, int cameraX)
        {
            sb.Draw(image, new Rectangle(spriteX - cameraX, spriteY, spriteWidth, spriteHeight), Color.White);
        }

        public virtual void UpdateAnimation(GameTime gameTime, bool moving, bool grounded)
        {
            // Process elapsed time
            time += (float)gameTime.ElapsedGameTime.TotalSeconds;
            while (time > frameTime)
            {
                // Play the next frame in the SpriteSheet
                if (moving && grounded)
                {
                    frameIndex++;
                }

                // reset elapsed time
                time = 0f;
            }
            if (frameIndex > totalFrames-1) frameIndex = 0;
        }

        /// Draws the sprite on the screen
        public void DrawAnimation(SpriteBatch spriteBatch, bool isInverted, float posX, float posY, bool left)
        {
            // Calculate the source rectangle of the current frame.
            Rectangle source = new Rectangle(frameIndex * frameWidth, 0, frameWidth, frameHeight);
            // Calculate position and origin to draw in the center of the screen
            Vector2 position = new Vector2(posX, posY);
            Vector2 origin = new Vector2(0, 0);

            if (isInverted)
            {
                if (left)
                {
                    spriteBatch.Draw(spriteSheet_i, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(spriteSheet_i, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
                }
            }
            else
            {
                if (left)
                {
                    spriteBatch.Draw(spriteSheet, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.FlipHorizontally, 0.0f);
                }
                else
                {
                    spriteBatch.Draw(spriteSheet, position, source, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
                }
            }
        }
	}
}

