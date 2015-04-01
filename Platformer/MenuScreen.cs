using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Platformer
{
    public class MenuScreen : Screen
    {
        private SpriteFont font;
        private List<MenuItem> menuItems;
        private MenuItem selectedItem;
        private int selectedIndex = 0;
        private float fontHeight = 20;

        public MenuScreen(List<MenuItem> menuItems, string type)
        {
            this.Type = type;
            this.menuItems = menuItems;
            this.selectedItem = this.menuItems[selectedIndex];

            for (int i = 0; i < this.menuItems.Count; i++)
            {
                menuItems[i].y = 100 + i * fontHeight;
                menuItems[i].x = 100;
            }

            this.selectedItem.IsHighlighted = true;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("MenuFont");
        }

        public override void Update(PlatformerMain game) {
            // if down key, go down in array
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (selectedIndex < menuItems.Count - 1)
                {
                    selectedIndex += 1;
                    selectedItem = menuItems[selectedIndex];
                    selectedItem.IsHighlighted = true;
                }
            }
            // if up key, go up in array
            else if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                if (selectedIndex > 0)
                {
                    selectedIndex -= 1;
                    selectedItem = menuItems[selectedIndex];
                    selectedItem.IsHighlighted = true;
                }
            }
            // if enter key, enter target screen
            else if (Keyboard.GetState().IsKeyDown(Keys.Enter)) {
                game.ChangeScreen(menuItems[selectedIndex].TargetScreen);
            }

            // Unhighlight other menu items
            for (int i = 0; i < menuItems.Count; i++)
            {
                if (i != selectedIndex)
                {
                    menuItems[i].IsHighlighted = false;
                }
            }
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].Draw(spriteBatch, font);
            }
        }
    }
}
