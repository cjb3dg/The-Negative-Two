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

namespace Platformer
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class PlatformerMain : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Controls controls;
        private Obstacle[] oList = new Obstacle[4];
        CharacterManager characterManager;
        LevelManager levelManager;
        InversionManager inversionManager;
        private Screen currentScreen;
        private static SoundEffect song;
        private static SoundEffect song_i;
        private static SoundEffectInstance backSong;
        private static SoundEffectInstance backSong_i;

        public PlatformerMain()
        {
            graphics = new GraphicsDeviceManager(this);
            inversionManager = new InversionManager();
            characterManager = new CharacterManager(levelManager, inversionManager, Content);
            levelManager = new LevelManager(inversionManager, characterManager, Content);
            Content.RootDirectory = "Content";
            currentScreen.Type = "MenuScreen";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            Joystick.Init();
            Console.WriteLine("Number of joysticks: " + Sdl.SDL_NumJoysticks());
            controls = new Controls();

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
            song = this.Content.Load<SoundEffect>("HawkeTheme");
            song_i = this.Content.Load<SoundEffect>("HawkeTheme_i");
            backSong = song.CreateInstance();
            backSong_i = song_i.CreateInstance();
            backSong.IsLooped = true;
            backSong_i.IsLooped = true;
            backSong.Play();
            backSong_i.Play();
            backSong_i.Volume = 0;

            levelManager.load();
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

            if (currentScreen.Type == "GameScreen")
            {
                if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) {
                    //Exit();

                    currentScreen.Type = "PauseScreen";
                }

                levelManager.Update(controls, gameTime);

                if (controls.onPress(Keys.Space, Buttons.A))
                {
                    float x = backSong_i.Volume;
                    backSong_i.Volume = backSong.Volume;
                    backSong.Volume = x;
                }

                base.Update(gameTime);
            } else {
                //currentScreen.Update(this);
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            if (currentScreen.Type == "GameScreen")
            {
                levelManager.Draw(spriteBatch, GraphicsDevice);
            } else {
                currentScreen.Draw(spriteBatch, GraphicsDevice);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}
