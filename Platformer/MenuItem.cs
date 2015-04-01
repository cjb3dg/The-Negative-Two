using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    class MenuItem
    {
        public string Text;
        public string TargetScreen;
        public bool IsHighlighted = false;

        public float x;
        public float y;

        public MenuItem(string text, string targetScreen)
        {
            this.Text = text;
            this.TargetScreen = targetScreen;
        }

        public void Draw(SpriteBatch sb, SpriteFont font)
        {
            if (IsHighlighted)
            {
                sb.DrawString(font, Text, new Vector2(x, y), Color.White);
            }
            else
            {
                sb.DrawString(font, Text, new Vector2(x, y), Color.Gray);
            }
        }
    }
}
