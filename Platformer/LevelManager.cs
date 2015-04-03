﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Platformer
{
    class LevelManager
    {
        private InversionManager inversionManager; /* handles invertibles */
        private CharacterManager characterManager;
        private ContentManager contentManager;

        //public List<Sprite> objects { get; private set; } /* all objects in level */

        public List<Item> items { get; set; } /* TODO */

        public List<Obstacle> neutralObstacles { get; set; }
        public List<Obstacle> whiteObstacles { get; set; }
        public List<Obstacle> blackObstacles { get; set; }

        public List<Boss> bosses { get; set; }

        public LevelManager(InversionManager inversionManager, CharacterManager characterManager, ContentManager contentManager)
        {
            this.inversionManager = inversionManager;
            this.characterManager = characterManager;
            this.contentManager = contentManager;

            neutralObstacles = new List<Obstacle>();
            whiteObstacles = new List<Obstacle>();
            blackObstacles = new List<Obstacle>();

            bosses = new List<Boss>();

            //objects = new List<Sprite>();
        }

        public void load()
        {
            Texture2D platformGrey = contentManager.Load<Texture2D>("Platform_grey");
            Texture2D platformBlack = contentManager.Load<Texture2D>("Platform_black");
            Texture2D platformWhite = contentManager.Load<Texture2D>("Platform_white");

            Obstacle neutralObstacle = new Obstacle(123, 284, 50, 50, platformGrey, platformGrey);
            Obstacle neutralObstacle2 = new Obstacle(250, 332, 50, 50, platformGrey, platformGrey);
            Obstacle neutralObstacle3 = new Obstacle(387, 137, 50, 350, platformGrey, platformGrey);
            neutralObstacle.setNeutral();
            neutralObstacle2.setNeutral();
            neutralObstacle3.setNeutral();

            Obstacle blackObstacle = new Obstacle(123, 332, 50, 150, platformBlack, platformBlack);
            Obstacle blackObstacle2 = new Obstacle(250, 50, 50, 50, platformBlack, platformBlack);
            Obstacle blackObstacle3 = new Obstacle(595, 284, 50, 50, platformBlack, platformBlack);
            blackObstacle.IsInverted = true;
            blackObstacle2.IsInverted = true;
            blackObstacle3.IsInverted = true;

            Obstacle whiteObstacle = new Obstacle(250, 180, 50, 50, platformWhite, platformWhite);
            Obstacle whiteObstacle2 = new Obstacle(513, 182, 50, 50, platformWhite, platformWhite);
            Obstacle whiteObstacle3 = new Obstacle(512, 373, 50, 50, platformWhite, platformWhite);

            //Texture2D goal = contentManager.Load<Texture2D>("Goal");
            //Texture2D goal_i = contentManager.Load<Texture2D>("Goal_i");

            addObject(neutralObstacle);
            addObject(neutralObstacle2);
            addObject(neutralObstacle3);
            addObject(whiteObstacle);
            addObject(whiteObstacle2);
            addObject(whiteObstacle3);
            addObject(blackObstacle);
            addObject(blackObstacle2);
            addObject(blackObstacle3);

            characterManager.Load();

            for (int i = 0; i < characterManager.enemyList.Count; i++)
            {
                inversionManager.registerInvertible((Invertible)characterManager.enemyList[i]);
            }
        }

        /*
         * Registers all objects with LevelManager
         * Registers all Invertibles with InversionManager
         */
        public void setObjects(List<Sprite> objects)
        {
            //this.objects = objects;

            for (int i = 0; i < objects.Count; i++)
            {
                addObject(objects[i]);
            }
        }

        /*
         * Adds a new object to LevelManager
         * If it is Invertible, it is also registered with InversionManager
         */
        public void addObject(Sprite obj)
        {
            //objects.Add(obj);

            /* list from top of potential inheritance tree to bottom */
            if (obj is Boss)
            {
                bosses.Add((Boss)obj);
            }
            else if (obj is Obstacle)
            {
                if (((Invertible)obj).IsNeutral)
                {
                    neutralObstacles.Add((Obstacle)obj);
                }
                else if (((Invertible)obj).IsInverted)
                {
                    blackObstacles.Add((Obstacle)obj);
                }
                else
                {
                    whiteObstacles.Add((Obstacle)obj);
                }

            }
            //else if (obj is Invertible)
            //{
            //    inversionManager.registerInvertible((Invertible)obj);
            //}
        }

        public void Draw(SpriteBatch sb, GraphicsDevice graphicsDevice)
        {
            inversionManager.Draw(sb, graphicsDevice);
            characterManager.Draw(sb);

            for (int i = 0; i < neutralObstacles.Count; i++)
            {
                neutralObstacles[i].Draw(sb);
            }

            if (inversionManager.IsWorldInverted)
            {
                for (int i = 0; i < whiteObstacles.Count; i++)
                {
                    whiteObstacles[i].Draw(sb);
                }
            }
            else
            {
                for (int i = 0; i < blackObstacles.Count; i++)
                {
                    blackObstacles[i].Draw(sb);
                }
            }


            //for (int i = 0; i < obstacles.Count; i++)
            //{
            //    Sprite obstacle = obstacles[i];
            //    if (obstacle is Invertible)
            //    {
            //        if (((Invertible)obstacle).IsInverted != inversionManager.IsWorldInverted || ((Invertible) obstacle).IsNeutral)
            //        {
            //            obstacle.Draw(sb);
            //        }
            //    }
            //    else
            //    {
            //        obstacles[i].Draw(sb);
            //    }
            //}
        }

        public void Update(Controls controls, GameTime gameTime)
        {
            inversionManager.Update(controls);

            int activeObstacleCount = neutralObstacles.Count;

            if (inversionManager.IsWorldInverted)
            {
                activeObstacleCount += whiteObstacles.Count;
            }
            else
            {
                activeObstacleCount += blackObstacles.Count;
            }

            List<Obstacle> activeObstacles = new List<Obstacle>(activeObstacleCount);
            activeObstacles.AddRange(neutralObstacles);

            if (inversionManager.IsWorldInverted)
            {
                activeObstacles.AddRange(whiteObstacles);
            }
            else
            {
                activeObstacles.AddRange(blackObstacles);
            }

            characterManager.Update(controls, gameTime, activeObstacles);
        }
    }
}
