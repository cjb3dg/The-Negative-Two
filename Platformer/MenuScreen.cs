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
    class MenuScreen : Screen
    {
        private SpriteFont font;
        private List<MenuItem> menuItems;
        private MenuItem selectedItem;
        private int selectedIndex = 0;
        private float fontHeight = 20;
        private Texture2D title;
        private Rectangle titleRec;

        private Texture2D neggy;
        private Rectangle neggyRec;

        private Texture2D butterfly;
        private Rectangle butterflyRec;
        private Texture2D butterfly1;
        private Rectangle butterflyRec1;
        private Texture2D butterfly2;
        private Rectangle butterflyRec2;

        public MenuScreen(List<MenuItem> menuItems, string type)
        {
            this.Type = type;
            this.menuItems = menuItems;
            this.selectedItem = this.menuItems[selectedIndex];

            if (type == "WinMenu" || type == "Credits")
            {
                menuItems[0].y = 625;
                menuItems[0].x = 825;
            }
            else
            {
                for (int i = 0; i < this.menuItems.Count; i++)
                {
                    menuItems[i].y = 300 + i * fontHeight+5;
                    menuItems[i].x = 100;
                }
            }

            this.selectedItem.IsHighlighted = true;
        }

        public override void LoadContent(ContentManager contentManager)
        {
            font = contentManager.Load<SpriteFont>("MenuFont");

            if (this.Type == "PauseMenu")
            {
                title = contentManager.Load<Texture2D>("pauseTitle.png");
                titleRec = new Rectangle(100, 100, 350, 60);
            }
            else if(this.Type == "DeathMenu")
            {
                title = contentManager.Load<Texture2D>("deathTitle.png");
                titleRec = new Rectangle(100, 100, 600, 60);
            }
            else if (this.Type == "LevelMenu")
            {
                title = contentManager.Load<Texture2D>("levelselectTitle.png");
                titleRec = new Rectangle(100, 100, 744, 58);
            }
            else if (this.Type == "VictoryMenu")
            {
                title = contentManager.Load<Texture2D>("levelcompleteTitle.png");
                titleRec = new Rectangle(100, 100, 924, 79);
            }
            else if (this.Type == "WinMenu")
            {
                title = contentManager.Load<Texture2D>("winTitle.png");
                titleRec = new Rectangle(0, 0, 1280, 720);
            }
            else if (this.Type == "Credits")
            {
                title = contentManager.Load<Texture2D>("creditsTitle.png");
                titleRec = new Rectangle(0, 0, 1280, 720);
            }
            else
            {
                title = contentManager.Load<Texture2D>("title.png");
                titleRec = new Rectangle(85, 100, 800, 90);
            }

            neggy = contentManager.Load<Texture2D>("neggy_menuscreen");
            neggyRec = new Rectangle(875, 325, 190, 250);

            butterfly = contentManager.Load<Texture2D>("butterfly_i0.png");
            butterflyRec = new Rectangle(1150, 250, 78,80);

            butterfly1 = contentManager.Load<Texture2D>("butterfly_i0.png");
            butterflyRec1 = new Rectangle(1050, 215, 59, 60);

            butterfly2 = contentManager.Load<Texture2D>("butterfly_i0.png");
            butterflyRec2 = new Rectangle(1115, 200, 39, 40);
        }

        public override void Update(GameMain game, Controls controls) {
            // if down key, go down in array
            if (controls.onPress(Keys.Down, Buttons.LeftThumbstickDown))
            {
                if (selectedIndex < menuItems.Count - 1)
                {
                    selectedIndex += 1;
                    selectedItem = menuItems[selectedIndex];
                    selectedItem.IsHighlighted = true;
                }
            }
            // if up key, go up in array
            else if (controls.onPress(Keys.Up, Buttons.LeftThumbstickUp))
            {
                if (selectedIndex > 0)
                {
                    selectedIndex -= 1;
                    selectedItem = menuItems[selectedIndex];
                    selectedItem.IsHighlighted = true;
                }
            }
            // if enter key, enter target screen
            else if (controls.onPress(Keys.Enter, Buttons.A)) {
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
            graphicsDevice.Clear(Color.Black);

            spriteBatch.Draw(title, titleRec, Color.White);
            spriteBatch.Draw(neggy, neggyRec, Color.White);
            spriteBatch.Draw(butterfly, butterflyRec, Color.White);
            spriteBatch.Draw(butterfly1, butterflyRec1, Color.White);
            spriteBatch.Draw(butterfly2, butterflyRec2, Color.White);

            for (int i = 0; i < menuItems.Count; i++)
            {
                menuItems[i].Draw(spriteBatch, font);
            }
        }
    }
}
