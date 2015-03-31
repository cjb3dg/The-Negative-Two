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
    class MenuScreen : Screen
    {
        private SpriteFont font;
        private List<MenuItem> menuItems = new List<MenuItem> { 
            new MenuItem("START GAME", "GameScreen"),
            new MenuItem("EXIT", "Exit")
        };
        private MenuItem selectedItem;
        private int selectedIndex = 0;

        public void MenuScreen(ContentManager content)
        {
            this.Type = "MenuScreen";
            this.content = content;
            this.selectedItem = this.menuItems[selectedIndex];

            font = content.Load<SpriteFont>("MenuFont");
        }
        public override void Update(GameTime gameTime) {
            // if down key, go down in array
            if (Keyboard.GetState().IsKeyDown(Keys.Down))
            {
                if (selectedIndex < menuItems.Count - 1)
                {
                    selectedIndex += 1;
                    selectedItem = menuItems[selectedIndex];
                    selectedItem.Highlight();
                }
            }
            // if up key, go up in array
            else if (Keyboard.GetState().IsKeyDown(Keys.Up)) {
                if (selectedIndex > 0)
                {
                    selectedIndex -= 1;
                    selectedItem = menuItems[selectedIndex];
                    selectedItem.Highlight();
                }
            }
            // if enter key, enter new screen
            else if (Keyboard.GetState().IsKeyDown(Keys.Enter)) { 
                
            }
        }
        public override void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].Draw(spriteBatch);
            }
        }
    }
}
