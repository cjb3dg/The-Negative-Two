using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace The_Negative_One
{

    class InversionManager
    {
        /* is the world inverted. True = negative world; False = normal world */
        public bool IsWorldInverted { get; private set; }

        /* all non-neutrals */
        public List<Invertible> invertibles { get; private set; }

        private Texture2D background;
        private Texture2D background_i;

        public InversionManager()
        {
            IsWorldInverted = false;
            if (invertibles == null)
            {
                invertibles = new List<Invertible>();
            }
        }

        public void invert()
        {
            IsWorldInverted = !IsWorldInverted;
            for (int i = 0; i < invertibles.Count; i++)
            {
                invertibles[i].invert();
            }
        }

        public void registerInvertible(Invertible i)
        {
            invertibles.Add(i);
        }

        public void Load(ContentManager contentManager)
        {
            background = contentManager.Load<Texture2D>("background.png");
            background_i = contentManager.Load<Texture2D>("background_i.png");
        }

        public void Draw(SpriteBatch sb, GraphicsDevice graphicsDevice)
        {
            if (!IsWorldInverted)
            {
                graphicsDevice.Clear(Color.White);
                sb.Draw(background, new Rectangle(0, 0, 1280, 720), Color.White);
            }
            else
            {
                graphicsDevice.Clear(Color.Black);
                sb.Draw(background_i, new Rectangle(0, 0, 1280, 720), Color.White);
            }
        }

        public void Update(Controls controls)
        {
            
        }
    }
}
