﻿#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Tao.Sdl;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;
using System.Collections;
#endregion

namespace The_Negative_One
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class GameMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Controls controls;
        CharacterManager characterManager;
        LevelManager levelManager;
        InversionManager inversionManager;

        private Screen currentMenuScreen;
        private MenuScreen mainMenuScreen;
        private MenuScreen levelMenuScreen;
        private MenuScreen pauseMenu;
        private MenuScreen victoryMenu;
        private MenuScreen deathMenu;
        private MenuScreen winMenu;
        private MenuScreen credits;

        private bool IsGameRunning = false;
        private int currentLevel;

        //private SoundEffect backgroundMusic;
        //private SoundEffectInstance backgroundMusicInstance;

        public GameMain()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = 1280;  // set this value to the desired width of your window
            graphics.PreferredBackBufferHeight = 720;   // set this value to the desired height of your window
            graphics.ApplyChanges();


            inversionManager = new InversionManager();
            characterManager = new CharacterManager(levelManager, inversionManager, Content);
            levelManager = new LevelManager(inversionManager, characterManager, Content);
            Content.RootDirectory = "Content";

            currentLevel = 0;

            mainMenuScreen = new MenuScreen(new List<MenuItem> { 
                new MenuItem("START GAME", "Tutorial"),
                new MenuItem("LEVEL SELECT", "LevelSelect"),
                new MenuItem("EXIT", "Exit")
            }, "MainMenu");
            levelMenuScreen = new MenuScreen(new List<MenuItem>
            {
                new MenuItem("TUTORIAL", "Tutorial"),
                new MenuItem("LEVEL 1", "LevelOne"),
                new MenuItem("LEVEL 2", "LevelTwo"),
                new MenuItem("BACK", "Back")
            }, "LevelMenu");
            pauseMenu = new MenuScreen(new List<MenuItem> { 
                new MenuItem("RESUME", "GameScreen"),
                new MenuItem("MAIN MENU", "Back"),
                new MenuItem("EXIT", "Exit")
            }, "PauseMenu");
            victoryMenu = new MenuScreen(new List<MenuItem> { 
                new MenuItem("NEXT LEVEL", "NextLevelScreen"),
                new MenuItem("EXIT", "Exit")
            }, "VictoryMenu");
            deathMenu = new MenuScreen(new List<MenuItem> {
                new MenuItem("MAIN MENU", "Back"),
                new MenuItem("RESTART LEVEL", "SameLevelScreen"),
                new MenuItem("EXIT", "Exit")
            }, "DeathMenu");
            winMenu = new MenuScreen(new List<MenuItem> {
                new MenuItem("NEXT", "Credits"),
            }, "WinMenu");

            credits = new MenuScreen(new List<MenuItem> {
                new MenuItem("MAIN MENU", "Back"),
            }, "Credits");

            currentMenuScreen = mainMenuScreen;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();

            //Type type = typeof(OpenTKGameWindow);
            //System.Reflection.FieldInfo field = type.GetField("window", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            //OpenTK.GameWindow window = (OpenTK.GameWindow)field.GetValue(this.Window);
            //this.Window.SetPosition(new Point(0,0));
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            // TODO: use this.Content to load your game content here

            spriteBatch = new SpriteBatch(GraphicsDevice);

            mainMenuScreen.LoadContent(Content);
            levelMenuScreen.LoadContent(Content);
            pauseMenu.LoadContent(Content);
            victoryMenu.LoadContent(Content);
            deathMenu.LoadContent(Content);
            winMenu.LoadContent(Content);

            //backgroundMusic = Content.Load<SoundEffect>("TimePassingBy");
            //backgroundMusicInstance = backgroundMusic.CreateInstance();
            //backgroundMusicInstance.IsLooped = true;
            //backgroundMusicInstance.Volume = 0.02f;
            //backgroundMusicInstance.Play();

            levelManager.load(currentLevel, true);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            //set our keyboardstate tracker update can change the gamestate on every cycle
            
            controls.Update();

            if (characterManager.player.victory)
            {
                ChangeScreen(victoryMenu.Type);
                characterManager.player.victory = false;
            }
            else if (!characterManager.player.isAlive())
            {
                ChangeScreen(deathMenu.Type);
                characterManager.player.setAlive(true);
            }
            else if (IsGameRunning)
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                {
                    ChangeScreen(pauseMenu.Type);
                }
                else
                {
                    levelManager.Update(controls, gameTime);

                    base.Update(gameTime);
                }
             } else {
                currentMenuScreen.Update(this, controls);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (IsGameRunning)
            {
                levelManager.Draw(spriteBatch, GraphicsDevice, inversionManager);
            } else {
                currentMenuScreen.Draw(spriteBatch, GraphicsDevice);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void ChangeScreen(string targetScreen)
        {
            if (targetScreen == "GameScreen")
            {
                IsGameRunning = true;
            }
            else if (targetScreen == "LevelSelect")
            {
                currentMenuScreen = levelMenuScreen;
                IsGameRunning = false;
            }
            else if (targetScreen == "Tutorial")
            {
                levelManager.unload();
                currentLevel = 0;
                levelManager.load(currentLevel, true);
                IsGameRunning = true;
            }
            else if (targetScreen == "LevelOne")
            {
                levelManager.unload();
                currentLevel = 1;
                levelManager.load(currentLevel, true);
                IsGameRunning = true;
            }
            else if (targetScreen == "LevelTwo")
            {
                levelManager.unload();
                currentLevel = 2;
                levelManager.load(currentLevel, true);
                IsGameRunning = true;
            }
            else if (targetScreen == "Back")
            {
                currentMenuScreen = mainMenuScreen;
                IsGameRunning = false;
            }
            else if (targetScreen == mainMenuScreen.Type)
            {
                currentMenuScreen = mainMenuScreen;
                IsGameRunning = false;
            }
            else if (targetScreen == pauseMenu.Type)
            {
                currentMenuScreen = pauseMenu;
                IsGameRunning = false;
            }
            else if (targetScreen == victoryMenu.Type)
            {
                currentMenuScreen = victoryMenu;
                IsGameRunning = false;
            }
            else if (targetScreen == deathMenu.Type)
            {
                currentMenuScreen = deathMenu;
                IsGameRunning = false;
            }
            else if (targetScreen == "SameLevelScreen")
            {
                levelManager.unload();
                levelManager.load(currentLevel, true);
                IsGameRunning = true;
            }
            else if (targetScreen == "NextLevelScreen")
            {
                if (currentLevel == 3)
                {
                    currentMenuScreen = winMenu;
                    IsGameRunning = false;
                }
                else
                {
                    levelManager.unload();
                    currentLevel++;
                    backSong.Volume = Math.Max(backSong.Volume, backSong_i.Volume);
                    backSong_i.Volume = 0;
                    levelManager.load(currentLevel, true);
                    IsGameRunning = true;
                }
            }
            else if (targetScreen == "Credits")
            {
                currentMenuScreen = credits;
                levelManager.unload();
                currentLevel++;
                levelManager.load(currentLevel, true);
                IsGameRunning = true;
            }
            else if (targetScreen == "Exit")
            {
                Exit();
            }
        }
    }

}
