﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Tao.Sdl;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Audio;

namespace Platformer
{
    class CharacterManager
    {
        public ContentManager content;
        public List<Enemy> enemyList;
        public List<Boss> bossList = new List<Boss>();
        public List<Projectile> projectileList;
        public Player player;
        public LevelManager lvlManager;
        public InversionManager invManager;

        public CharacterManager(LevelManager lvl, InversionManager inv, ContentManager cont)
        {
            player = new Player(400, 400, 50, 50);
            enemyList = new List<Enemy>();
            projectileList = new List<Projectile>();
            lvlManager = lvl;
            invManager = inv;
            content = cont;
            inv.registerInvertible(player);
        }

        public void LoadEnemies(string filename)
        {
            string line;

            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(filename);
            while ((line = file.ReadLine()) != null)
            {
                if (line == "Enemy")
                {
                    int x = Convert.ToInt32(file.ReadLine());
                    int y = Convert.ToInt32(file.ReadLine());
                    int width = Convert.ToInt32(file.ReadLine());
                    int height = Convert.ToInt32(file.ReadLine());
                    Texture2D normal = content.Load<Texture2D>(file.ReadLine());
                    Texture2D inverted = content.Load<Texture2D>(file.ReadLine());
                    int curHP = Convert.ToInt32(file.ReadLine());
                    MovementPattern mPattern;
                    List<double> xVList = new List<double>();
                    List<double> yVList = new List<double>();

                    while ((line = file.ReadLine()) != "endEnemy")
                    {
                        string line2 = file.ReadLine();
                        xVList.Add(Convert.ToDouble(line));
                        yVList.Add(Convert.ToDouble(line2));
                    }
                    mPattern = new MovementPattern(xVList, yVList);

                    Enemy newEnemy = new Enemy(x, y, width, height, normal, inverted, curHP, mPattern);
                    enemyList.Add(newEnemy);
                }

                else if (line == "Boss")
                {
                    int x = Convert.ToInt32(file.ReadLine());
                    int y = Convert.ToInt32(file.ReadLine());
                    int width = Convert.ToInt32(file.ReadLine());
                    int height = Convert.ToInt32(file.ReadLine());
                    Texture2D normal = content.Load<Texture2D>(file.ReadLine());
                    Texture2D inverted = content.Load<Texture2D>(file.ReadLine());
                    int maxHP = Convert.ToInt32(file.ReadLine());
                    MovementPattern mPattern;
                    List<MovementPattern> mPList = new List<MovementPattern>();
                    List<double> xVList = new List<double>();
                    List<double> yVList = new List<double>();

                    while ((line = file.ReadLine()) != "endBoss")
                    {
                        if (line == "endPattern")
                        {
                            mPattern = new MovementPattern(xVList, yVList);
                            mPList.Add(mPattern);
                            xVList = new List<double>();
                            yVList = new List<double>();
                        }

                        else
                        {
                            string line2 = file.ReadLine();
                            xVList.Add(Convert.ToDouble(line));
                            yVList.Add(Convert.ToDouble(line2));
                        }

                    }
                    mPattern = new MovementPattern(xVList, yVList);

                    Boss newBoss = new Boss(x, y, width, height, normal, inverted, maxHP, mPList, player);
                    bossList.Add(newBoss);
                }

            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();
        }

        public void Load()
        {
            LoadEnemies("Content/test.txt");
            player.LoadContent(this.content);
            //shouldn't be in the final version, but here to provide some textures
            Texture2D platformGrey = content.Load<Texture2D>("Platform_grey");
            Texture2D platformBlack = content.Load<Texture2D>("Platform_black");
            Texture2D platformWhite = content.Load<Texture2D>("Platform_white");

        }

        public int Update(Controls controls, Microsoft.Xna.Framework.GameTime gametime, List<Obstacle> oList, Door door, bool cameraStill, int cameraX)
        {
            foreach (Enemy e in enemyList)
            {
                e.Update(gametime, oList);
            }
            foreach (Projectile p in projectileList)
            {
                p.Update(oList, ref enemyList, cameraX);
            }
            foreach (Boss b in bossList)
            {
                b.Update(gametime, player);
            }

            projectileList.RemoveAll(p => !p.isAlive());
            enemyList.RemoveAll(e => !e.isAlive());
            bossList.RemoveAll(b => !b.isAlive());


            int retVal = player.Update(controls, gametime, oList, enemyList, invManager, door, cameraStill);

            if (player.Shoot(controls))
            {
                double projectileXVel = 4;
                int projectileX = player.getWidth() - 2;
                int projectileY = 18;
                if (!player.facingRight())
                {
                    projectileXVel = -4;
                    projectileX = 2;
                }
                projectileList.Add(new Projectile(player.getX() + projectileX, player.getY() + projectileY, 20, 10, content.Load<Texture2D>("Platform_grey"), content.Load<Texture2D>("Platform_grey"), projectileXVel, 0));
            }
            return retVal;
        }

        public void DrawHPAndEnergy(SpriteBatch spriteBatch)
        {
            Texture2D temp = content.Load<Texture2D>("Platform_grey");
            spriteBatch.Draw(temp, new Rectangle(10, 10, player.maxHP / 10, 15), Color.Red);
            spriteBatch.Draw(temp, new Rectangle(10, 10, player.getHP() / 10, 15), Color.Green);
            spriteBatch.Draw(temp, new Rectangle(10, 30, player.maxEnergy / 10, 15), Color.Gray);
            spriteBatch.Draw(temp, new Rectangle(45, 30, player.maxEnergy / 10, 15), Color.Gray);
            spriteBatch.Draw(temp, new Rectangle(80, 30, player.maxEnergy / 10, 15), Color.Gray);
            if (player.getEnergy() >= 100)
            {
                spriteBatch.Draw(temp, new Rectangle(10, 30, 30, 15), Color.Yellow);
            }
            else
            {
                spriteBatch.Draw(temp, new Rectangle(10, 30, (player.getEnergy() * 3) / 10, 15), Color.Yellow);
            }
            if (player.getEnergy() >= 200)
            {
                spriteBatch.Draw(temp, new Rectangle(45, 30, 30, 15), Color.Yellow);
            }
            else if (player.getEnergy() > 100)
            {
                spriteBatch.Draw(temp, new Rectangle(45, 30, ((player.getEnergy() - 100) * 3) / 10, 15), Color.Yellow);
            }
            if (player.getEnergy() >= 300)
            {
                spriteBatch.Draw(temp, new Rectangle(80, 30, 30, 15), Color.Yellow);
            }
            else if (player.getEnergy() > 200)
            {
                spriteBatch.Draw(temp, new Rectangle(80, 30, ((player.getEnergy() - 200) * 3) / 10, 15), Color.Yellow);
            }
        }

        public void Draw(SpriteBatch spriteBatch, int cameraX)
        {
            foreach (Projectile p in projectileList)
            {
                p.Draw(spriteBatch, cameraX);
            }
            foreach (Enemy e in enemyList)
            {
                e.Draw(spriteBatch, cameraX);
            }
            foreach (Boss b in bossList)
            {
                b.Draw(spriteBatch, cameraX);
            }
            if (player.isAlive())
            {
                player.Draw(spriteBatch, cameraX);
            }

            DrawHPAndEnergy(spriteBatch);

            if (player.victory == true)
            {
                spriteBatch.Draw(content.Load<Texture2D>("Victory"), new Rectangle(50, 50, 700, 400), Color.White);
            }
        }

    }
}