﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using System.IO;

namespace The_Negative_One
{
    class LevelManager
    {
        private InversionManager inversionManager;
        private CharacterManager characterManager;
        private ContentManager contentManager;

        public List<Item> items { get; set; } /* TODO */

        public List<Obstacle> neutralObstacles { get; set; }
        public List<Obstacle> whiteObstacles { get; set; }
        public List<Obstacle> blackObstacles { get; set; }

        //public List<Boss> bosses { get; set; }

        public Door door;
        private int cameraX;
        private bool cameraStill;
        private int bossCameraX;

        public LevelManager(InversionManager inversionManager, CharacterManager characterManager, ContentManager contentManager)
        {
            this.inversionManager = inversionManager;
            this.characterManager = characterManager;
            this.contentManager = contentManager;

            neutralObstacles = new List<Obstacle>();
            whiteObstacles = new List<Obstacle>();
            blackObstacles = new List<Obstacle>();
            items = new List<Item>();

            //bosses = new List<Boss>();
            //this.cameraStill = false;
            //this.cameraX = -350;
        }

        public void load(int level, bool replenishHealth)
        {
            switch (level)
            {
                case 0:
                    LoadFromFile("Content/Level0.txt");
                    break;
                case 1:
                    LoadFromFile("Content/Level1.txt");
                    break;
                case 2:
                    LoadFromFile("Content/Level2.txt");
                    break;
                case 3:
                    LoadFromFile("Content/Level3.txt");
                    break;
                case 4:
                    LoadFromFile("Content/Level1.txt");
                    break;
                default:
                    LoadFromFile("Content/Level0.txt");
                    break;
            }
            characterManager.Load(level, replenishHealth);
        }

        public void LoadFromFile(String filename)
        {
            Texture2D platformGrey = contentManager.Load<Texture2D>("Platform_grey");
            Texture2D platformBlack = contentManager.Load<Texture2D>("Platform_black");
            Texture2D platformWhite = contentManager.Load<Texture2D>("Platform_white");
            Texture2D doorTex = contentManager.Load<Texture2D>("door");

            this.cameraX = -655;
            this.cameraStill = false;
            if (inversionManager.IsWorldInverted)
            {
                inversionManager.invert();
            }

            try
            {
                using (StreamReader sr = new StreamReader(filename))
                {
                    String firstLine = sr.ReadLine();
                    this.bossCameraX = Convert.ToInt32(firstLine);
                    while (!sr.EndOfStream)
                    {
                        String line = sr.ReadLine();
                        String[] divided = line.Split(',');
                        if (divided[0].Equals("g"))
                        {
                            Obstacle neutralObstacle = new Obstacle(Convert.ToInt32(divided[1]), Convert.ToInt32(divided[2]), Convert.ToInt32(divided[3]), Convert.ToInt32(divided[4]), platformGrey, platformGrey);
                            neutralObstacle.setNeutral();
                            addObject(neutralObstacle);
                        }
                        else if (divided[0].Equals("b"))
                        {
                            Obstacle blackObstacle = new Obstacle(Convert.ToInt32(divided[1]), Convert.ToInt32(divided[2]), Convert.ToInt32(divided[3]), Convert.ToInt32(divided[4]), platformBlack, platformBlack);
                            blackObstacle.IsInverted = true;
                            addObject(blackObstacle);
                        }
                        else if (divided[0].Equals("w"))
                        {
                            Obstacle whiteObstacle = new Obstacle(Convert.ToInt32(divided[1]), Convert.ToInt32(divided[2]), Convert.ToInt32(divided[3]), Convert.ToInt32(divided[4]), platformWhite, platformWhite);
                            addObject(whiteObstacle);
                        }
                        else if (divided[0].Equals("d"))
                        {
                            this.door = new Door(Convert.ToInt32(divided[1]), Convert.ToInt32(divided[2]), Convert.ToInt32(divided[3]), Convert.ToInt32(divided[4]), doorTex, doorTex, false);
                            this.door.setNeutral();
                        }
                        else if (divided[0].Equals("i"))
                        {
                            items.Add(new Item(Convert.ToInt32(divided[1]), Convert.ToInt32(divided[2]), Convert.ToInt32(divided[3]), Convert.ToInt32(divided[4]), 0, contentManager.Load<Texture2D>("healing_item"), contentManager.Load<Texture2D>("healing_item_i")));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }

            inversionManager.Load(contentManager);

            foreach (Item i in items)
            {
                inversionManager.registerInvertible(i);
            }

            for (int i = 0; i < characterManager.enemyList.Count; i++)
            {
                inversionManager.registerInvertible((Invertible)characterManager.enemyList[i]);
            }
        }

        public void unload()
        {
            neutralObstacles.Clear();
            blackObstacles.Clear();
            whiteObstacles.Clear();
            //bosses.Clear();
            items.Clear();
            characterManager.unload();
        }

        /*
         * Registers all objects with LevelManager
         * Registers all Invertibles with InversionManager
         */
        public void setObjects(List<Sprite> objects)
        {
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
            if (obj is Obstacle)
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
        }

        public void CameraFollow()
        {
            this.cameraStill = false;
        }

        public void CameraStill()
        {
            this.cameraStill = true;
        }

        public void Draw(SpriteBatch sb, GraphicsDevice graphicsDevice)
        {
            inversionManager.Draw(sb, graphicsDevice);
            if (door.isActive())
            {
                door.Draw(sb, cameraX);
            }

            foreach (Item i in items)
            {
                i.Draw(sb, cameraX);
            }

            for (int i = 0; i < neutralObstacles.Count; i++)
            {
                neutralObstacles[i].Draw(sb, cameraX);
            }

            if (inversionManager.IsWorldInverted)
            {
                for (int i = 0; i < whiteObstacles.Count; i++)
                {
                    whiteObstacles[i].Draw(sb, cameraX);
                }
            }
            else
            {
                for (int i = 0; i < blackObstacles.Count; i++)
                {
                    blackObstacles[i].Draw(sb, cameraX);
                }
            }
            characterManager.Draw(sb, cameraX, cameraStill);
        }

        public void MoveCamera(int movement)
        {
            cameraX += movement;
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
            foreach (Obstacle o in neutralObstacles)
            {
                if (!(o.getX() + o.getWidth() < cameraX || o.getX() > cameraX + 1280))
                {
                    activeObstacles.Add(o);
                }
            }
            //activeObstacles.AddRange(neutralObstacles);

            if (inversionManager.IsWorldInverted)
            {
                //activeObstacles.AddRange(whiteObstacles);
                foreach (Obstacle o in whiteObstacles)
                {
                    if (!(o.getX() + o.getWidth() < cameraX || o.getX() > cameraX + 1280))
                    {
                        activeObstacles.Add(o);
                    }
                }
            }
            else
            {
                //activeObstacles.AddRange(blackObstacles);
                foreach (Obstacle o in blackObstacles)
                {
                    if (!(o.getX() + o.getWidth() < cameraX || o.getX() > cameraX + 1280))
                    {
                        activeObstacles.Add(o);
                    }
                }
            }

            int distanceMoved = characterManager.Update(controls, gameTime, activeObstacles, items, door, cameraStill, cameraX);

            if (characterManager.bossDead())
            {
                door.setActive(true);
            }

            items.RemoveAll(i => !i.isAlive());

            if (!cameraStill)
            {
                MoveCamera(distanceMoved);
            }
            if (cameraX > bossCameraX)
            {
                CameraStill();
            }
        }
    }
}
