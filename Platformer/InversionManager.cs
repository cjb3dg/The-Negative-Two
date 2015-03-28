using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    class InversionManager
    {
        /* is the world inverted. True = negative world; False = normal world */
        public bool IsWorldInverted { get; private set; }

        /* all non-neutrals */
        public List<Invertible> invertibles { get; private set; }

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

        public void Draw(SpriteBatch sb, GraphicsDevice graphicsDevice)
        {
            if (!IsWorldInverted)
            {
                graphicsDevice.Clear(Color.White);
            }
            else
            {
                graphicsDevice.Clear(Color.Black);
            }
        }

        public void Update(Controls controls)
        {
            if (controls.onPress(Keys.Space, Buttons.A))
            {
                invert();
            }
        }
    }
}
