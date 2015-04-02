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
        ContentManager content;
        public List<Enemy> enemyList;
        List<Projectile> projectileList;
        Player player;
        LevelManager lvlManager;
        InversionManager invManager;

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

        public void Load()
        {
            player.LoadContent(this.content);
            //shouldn't be in the final version, but here to provide some textures
            Texture2D platformGrey = content.Load<Texture2D>("Platform_grey");
            Texture2D platformBlack = content.Load<Texture2D>("Platform_black");
            Texture2D platformWhite = content.Load<Texture2D>("Platform_white");


            List<double> xVList = new List<double>();
            List<double> yVList = new List<double>();
            List<double> xVList2 = new List<double>();
            List<double> yVList2 = new List<double>();

            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(-1);
            yVList.Add(1);
            xVList.Add(-2);
            yVList.Add(0);

            xVList.Add(2);
            yVList.Add(0);
            xVList.Add(1);
            yVList.Add(-1);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);


            xVList.Add(2);
            yVList.Add(0);
            xVList.Add(2);
            yVList.Add(0);

            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(2);
            xVList.Add(0);
            yVList.Add(1);

            xVList.Add(0);
            yVList.Add(-1);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);
            xVList.Add(0);
            yVList.Add(-2);

            xVList.Add(-2);
            yVList.Add(0);
            xVList.Add(-2);
            yVList.Add(0);


            xVList2.Add(0);
            yVList2.Add(2);
            xVList2.Add(0);
            yVList2.Add(2);
            xVList2.Add(0);
            yVList2.Add(2);
            xVList2.Add(0);
            yVList2.Add(2);

            xVList2.Add(0);
            yVList2.Add(-2);
            xVList2.Add(0);
            yVList2.Add(-2);
            xVList2.Add(0);
            yVList2.Add(-2);
            xVList2.Add(0);
            yVList2.Add(-2);

            MovementPattern myPattern = new MovementPattern(xVList, yVList);
            MovementPattern myPattern2 = new MovementPattern(xVList2, yVList2);
            Enemy myEnemy = new Enemy(330, 80, 45, 45, content.Load<Texture2D>("Butterfly_s"), content.Load<Texture2D>("Butterfly_s_i"), 1, myPattern);
            Enemy myEnemy2 = new Enemy(195, 80, 45, 45, content.Load<Texture2D>("Butterfly_s"), content.Load<Texture2D>("Butterfly_s_i"), 1, myPattern2);
            myEnemy.yVel = 2;
            myEnemy2.yVel = 2;
            enemyList.Add(myEnemy);
            enemyList.Add(myEnemy2);

        }

        public void Shift(int movement)
        {
            foreach (Enemy e in enemyList)
            {
                e.setX(e.getX() + movement);
            }
            foreach (Projectile p in projectileList)
            {
                p.setX(p.getX() + movement);
            }
        }

        public int Update(Controls controls, Microsoft.Xna.Framework.GameTime gametime, List<Obstacle> oList, bool cameraStill)
        {
            foreach (Enemy e in enemyList)
            {
                e.Update(gametime, oList);
            }
            foreach (Projectile p in projectileList)
            {
                p.Update(oList, ref enemyList);
            }
            projectileList.RemoveAll(p => !p.isAlive());
            enemyList.RemoveAll(e => !e.isAlive());

            int retVal = player.Update(controls, gametime, oList, enemyList, invManager, cameraStill);

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

            //update levelmanager and inversion manager
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

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Projectile p in projectileList)
            {
                p.Draw(spriteBatch);
            }
            foreach (Enemy e in enemyList)
            {
                e.Draw(spriteBatch);
            }
            if (player.isAlive())
            {
                player.Draw(spriteBatch);
            }
            DrawHPAndEnergy(spriteBatch);
            if (player.victory == true)
            {
                spriteBatch.Draw(content.Load<Texture2D>("Victory"), new Rectangle(50, 50, 700, 400), Color.White);
            }
        }

    }
}