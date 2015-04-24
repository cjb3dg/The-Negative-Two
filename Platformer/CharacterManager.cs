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

namespace The_Negative_One
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
            player = new Player(0, 645, 37, 41);
            enemyList = new List<Enemy>();
            projectileList = new List<Projectile>();
            lvlManager = lvl;
            invManager = inv;
            content = cont;
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
                    Texture2D image = content.Load<Texture2D>(file.ReadLine());
                    Texture2D image_i = content.Load<Texture2D>(file.ReadLine());
                    int totalFrames = Convert.ToInt32(file.ReadLine());
                    int curHP = Convert.ToInt32(file.ReadLine());
                    MovementPattern mPattern;
                    List<double> xVList = new List<double>();
                    List<double> yVList = new List<double>();
                    List<bool> shootList = new List<bool>();

                    while ((line = file.ReadLine()) != "endEnemy")
                    {
                        string line2 = file.ReadLine();
                        string line3 = file.ReadLine();
                        xVList.Add(Convert.ToDouble(line));
                        yVList.Add(Convert.ToDouble(line2));
                        shootList.Add(Convert.ToBoolean(line3));
                    }
                    mPattern = new MovementPattern(xVList, yVList, shootList);

                    Enemy newEnemy = new Enemy(x, y, width, height, image, image_i, totalFrames, curHP, mPattern);
                    enemyList.Add(newEnemy);
                    invManager.registerInvertible(newEnemy);
                }

                else if (line == "Boss")
                {
                    int x = Convert.ToInt32(file.ReadLine());
                    int y = Convert.ToInt32(file.ReadLine());
                    int width = Convert.ToInt32(file.ReadLine());
                    int height = Convert.ToInt32(file.ReadLine());
                    String color = file.ReadLine();
                    Texture2D image = content.Load<Texture2D>(file.ReadLine());
                    Texture2D image_i = content.Load<Texture2D>(file.ReadLine());
                    Texture2D pImage = content.Load<Texture2D>(file.ReadLine());
                    Texture2D pImage_i = content.Load<Texture2D>(file.ReadLine());
                    int totalFrames = Convert.ToInt32(file.ReadLine());
                    int maxHP = Convert.ToInt32(file.ReadLine());
                    MovementPattern mPattern;
                    List<MovementPattern> mPList = new List<MovementPattern>();
                    List<double> xVList = new List<double>();
                    List<double> yVList = new List<double>();
                    List<bool> shootList = new List<bool>();

                    while ((line = file.ReadLine()) != "endBoss")
                    {
                        if (line == "endPattern")
                        {
                            mPattern = new MovementPattern(xVList, yVList, shootList);
                            mPList.Add(mPattern);
                            xVList = new List<double>();
                            yVList = new List<double>();
                            shootList = new List<bool>();
                        }

                        else
                        {
                            string line2 = file.ReadLine();
                            string line3 = file.ReadLine();
                            xVList.Add(Convert.ToDouble(line));
                            yVList.Add(Convert.ToDouble(line2));
                            shootList.Add(Convert.ToBoolean(line3));
                        }

                    }
                    mPattern = new MovementPattern(xVList, yVList, shootList);
                    bool invert = false;
                    bool neutral = false;

                    if (color.Equals("b"))
                    {
                        invert = true;
                    }
                    if (color.Equals("n"))
                    {
                        neutral = true;
                    }

                    Boss newBoss = new Boss(x, y, width, height, image, image_i, totalFrames, maxHP, mPList, player, invert, neutral, false);
                    newBoss.setPTexture(pImage, pImage_i);
                    bossList.Add(newBoss);
                }

            }

            file.Close();

            // Suspend the screen.
            Console.ReadLine();
        }

        public void Load(int level)
        {
            switch (level)
            {
                case 0:
                    LoadEnemies("Content/Enemies0.txt");
                    break;
                case 1:
                    LoadEnemies("Content/Enemies1.txt");
                    break;
                case 2:
                    LoadEnemies("Content/Enemies2.txt");
                    break;
                case 3:
                    LoadEnemies("Content/Enemies3.txt");
                    break;
                case 4:
                    LoadEnemies("Content/Enemies1.txt");
                    break;
                default:
                    LoadEnemies("Content/Enemies0.txt");
                    break;
            }
            player = new Player(0, 645, 37, 41);
            player.LoadContent(this.content);
            invManager.registerInvertible(player);

        }

        public void unload()
        {
            enemyList.Clear();
            projectileList.Clear();
            bossList.Clear();
        }

        public int Update(Controls controls, Microsoft.Xna.Framework.GameTime gametime, List<Obstacle> oList, List<Item> itemList, Door door, bool cameraStill, int cameraX)
        {
            List<Boss> activeBossList = new List<Boss>();
            if (cameraStill)
            {
                foreach (Boss b in bossList)
                {
                    b.active = true;
                    b.Update(gametime, player, enemyList, content);
                }
            }
            foreach (Boss b in bossList)
            {
                if (b.active && b.IsActive(invManager))
                {
                    activeBossList.Add(b);
                }
            }
            foreach (Enemy e in enemyList)
            {
                e.Update(gametime, oList);
            }
            foreach (Projectile p in projectileList)
            {
                p.Update(oList, ref enemyList, ref activeBossList, cameraX, player);
            }

            projectileList.RemoveAll(p => !p.isAlive());
            enemyList.RemoveAll(e => !e.isAlive());
            bossList.RemoveAll(b => !b.isAlive());


            int retVal = player.Update(controls, gametime, oList, enemyList, bossList, itemList, projectileList, invManager, door, cameraStill, cameraX);

            if (player.Shoot(controls))
            {
                double projectileXVel = 8;
                int projectileX = player.getWidth();
                int projectileY = 25;
                if (!player.facingRight())
                {
                    projectileXVel = -8;
                    projectileX = -4;
                }
                projectileList.Add(new Projectile(player.getX() + projectileX, player.getY() + projectileY, 12, 6, content.Load<Texture2D>("Platform_grey"), content.Load<Texture2D>("Platform_grey"), projectileXVel, 0, true));
            }

            foreach (Boss b in bossList)
            {
                if (b.shoot)
                {
                    double x = (b.getX() + b.getWidth()/2) - (player.getX() + player.getWidth()/2);
                    double y = (b.getY() + b.getHeight() / 2) - (player.getY() + player.getHeight() / 2);
                    double d = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));

                    double projectileXVel = -8*x/d;
                    double projectileYVel = -8*y/d;
                    //int projectileX = player.getWidth() - 2;
                    //int projectileY = 18;
                    int pWidth = b.pTexture.Width;
                    int pHeight = b.pTexture.Height;
                    projectileList.Add(new Projectile(b.getX() + b.getWidth()/2 - pWidth/2, b.getY() + b.getHeight()/2 - pHeight/2, b.pTexture.Width, b.pTexture.Height, b.pTexture, b.pTexture_i, projectileXVel, projectileYVel, false));
                    b.shoot = false;
                }
            }
            return retVal;
        }

        public void DrawHPAndEnergy(SpriteBatch spriteBatch)
        {
            Texture2D temp = content.Load<Texture2D>("Platform_grey");
            Texture2D temp2 = content.Load<Texture2D>("Platform_white");
            Texture2D temp3 = content.Load<Texture2D>("Platform_black");

            if (player.IsInverted == true)
            {
                int x = player.maxHP;
                spriteBatch.Draw(temp2, new Rectangle(7, 9, (x / 10) + 6, 17), Color.White);
                spriteBatch.Draw(temp2, new Rectangle(9, 7, (x / 10) + 2, 21), Color.White);
                spriteBatch.Draw(temp3, new Rectangle(9, 9, (x / 10) + 2, 17), Color.Black);
                spriteBatch.Draw(temp2, new Rectangle(10, 10, player.getHP() / 10, 15), Color.White);
            }
            else
            {
                int x = player.maxHP;
                spriteBatch.Draw(temp3, new Rectangle(7, 9, (x / 10) + 6, 17), Color.Black);
                spriteBatch.Draw(temp3, new Rectangle(9, 7, (x / 10) + 2, 21), Color.Black);
                spriteBatch.Draw(temp2, new Rectangle(9, 9, (x / 10) + 2, 17), Color.White);
                spriteBatch.Draw(temp3, new Rectangle(10, 10, player.getHP() / 10, 15), Color.Black);
            }

            spriteBatch.Draw(temp, new Rectangle(10, 30, player.maxEnergy / 10, 15), Color.White);
            spriteBatch.Draw(temp, new Rectangle(45, 30, player.maxEnergy / 10, 15), Color.White);
            spriteBatch.Draw(temp, new Rectangle(80, 30, player.maxEnergy / 10, 15), Color.White);
            if (player.getEnergy() >= 100)
            {
                spriteBatch.Draw(temp, new Rectangle(10, 30, 30, 15), Color.Gray);
            }
            else
            {
                spriteBatch.Draw(temp, new Rectangle(10, 30, (player.getEnergy() * 3) / 10, 15), Color.Gray);
            }
            if (player.getEnergy() >= 200)
            {
                spriteBatch.Draw(temp, new Rectangle(45, 30, 30, 15), Color.Gray);
            }
            else if (player.getEnergy() > 100)
            {
                spriteBatch.Draw(temp, new Rectangle(45, 30, ((player.getEnergy() - 100) * 3) / 10, 15), Color.Gray);
            }
            if (player.getEnergy() >= 300)
            {
                spriteBatch.Draw(temp, new Rectangle(80, 30, 30, 15), Color.Gray);
            }
            else if (player.getEnergy() > 200)
            {
                spriteBatch.Draw(temp, new Rectangle(80, 30, ((player.getEnergy() - 200) * 3) / 10, 15), Color.Gray);
            }
        }

        public void DrawBossHP(SpriteBatch spriteBatch, List<Boss> bList, bool cameraStill)
        {
            if (cameraStill)
            {
                Texture2D temp = content.Load<Texture2D>("Platform_grey");
                Texture2D temp2 = content.Load<Texture2D>("Platform_white");
                Texture2D temp3 = content.Load<Texture2D>("Platform_black");
                for (int i = 0; i < bList.Count; i++)
                {
                    int yOffset = 30 * i;
                    Boss b = new Boss();
                    b = bList[i];
                    if (player.IsInverted == true)
                    {
                        int x = b.maxHP;
                        spriteBatch.Draw(temp2, new Rectangle(957, 9 + yOffset, (x * 30) + 6, 17), Color.White);
                        spriteBatch.Draw(temp2, new Rectangle(959, 7 + yOffset, (x * 30) + 2, 21), Color.White);
                        spriteBatch.Draw(temp3, new Rectangle(959, 9 + yOffset, (x * 30) + 2, 17), Color.Black);
                        spriteBatch.Draw(temp2, new Rectangle(960, 10 + yOffset, b.curHP * 30, 15), Color.White);
                    }
                    else
                    {
                        int x = b.maxHP;
                        spriteBatch.Draw(temp3, new Rectangle(957, 9 + yOffset, (x * 30) + 6, 17), Color.Black);
                        spriteBatch.Draw(temp3, new Rectangle(959, 7 + yOffset, (x * 30) + 2, 21), Color.Black);
                        spriteBatch.Draw(temp2, new Rectangle(959, 9 + yOffset, (x * 30) + 2, 17), Color.White);
                        spriteBatch.Draw(temp3, new Rectangle(960, 10 + yOffset, b.curHP * 30, 15), Color.Black);
                    }
                }
            }
        }

        public bool bossDead()
        {
            return (bossList.Count == 0);
        }

        public void Draw(SpriteBatch spriteBatch, int cameraX, bool cameraStill)
        {
            foreach (Projectile p in projectileList)
            {
                p.Draw(spriteBatch, cameraX);
            }
            foreach (Enemy e in enemyList)
            {
                e.Draw(spriteBatch, cameraX);
            }
            if (cameraStill)
            {
                foreach (Boss b in bossList)
                {
                    b.Draw(spriteBatch, cameraX);
                }
            }
            if (player.isAlive())
            {
                player.Draw(spriteBatch, cameraX);
            }

            DrawHPAndEnergy(spriteBatch);
            DrawBossHP(spriteBatch, bossList, cameraStill);
        }

    }
}