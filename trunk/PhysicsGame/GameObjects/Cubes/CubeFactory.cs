﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysicsGame.GameObjects;
using PhysicsGame.GameObjects.Cubes;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsGame.GameObjects.Cubes
{
    class CubeFactory
    {
        public static CubeNode createCubeNode(TextureStore textureStore, PhysicsController physicsController, CubeDescription cubeDescription, Vector2 cubeSize)
        {
            PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, cubeSize.X, cubeSize.Y, false);

            List<Texture2D> defaultTextureList = new List<Texture2D>();

            CubeNode ret;


            //public enum CubeType { UnknownCube, PlainCube, RocketCube, ShieldCube, HeavyCube };

            if (cubeDescription.type == CubeType.UnknownCube)
            {
                ret = new UnknownCube();
                defaultTextureList = textureStore.unknownTextures;
            }
            else if (cubeDescription.type == CubeType.PlainCube)
            {
                ret = new PlainCube();
                defaultTextureList = textureStore.plainTextures;
            }
            else if (cubeDescription.type == CubeType.RocketCube)
            {
                ret = new RocketCube();
                defaultTextureList = textureStore.rocketTextures;
                ((RocketCube)ret).activationCountdown = 500;

                foreach (Texture2D tex in textureStore.rocketFlame)
                    pgo.getTextureSet("MainFlame").addTexture(tex);

                pgo.getTextureSet("MainFlame").offset = new Vector2(cubeSize.X-1, 0);

                /*foreach (Texture2D tex in textureStore.rocketFlame)
                    pgo.getTextureSet("InternalFlame").addTexture(tex);

                pgo.addTextureSet("InternalFlame");*/

                float rotation = 0.0f;
                switch (cubeDescription.dir)
                {
                    case Direction.East:
                        rotation = 0;
                        break;
                    case Direction.South:
                        rotation = MathHelper.Pi/2;
                        break;
                    case Direction.West:
                        rotation = MathHelper.Pi;
                        break;
                    case Direction.North:
                        rotation = MathHelper.Pi*3/2;
                        break;
                }
                pgo.boxBody.Rotation = rotation;

            }
            else if (cubeDescription.type == CubeType.ShieldCube)
            {
                ret = new ShieldCube();
                defaultTextureList = textureStore.shieldTextures;
            }
            else// ; if (cubeDescription.type == CubeType.HeavyCube)
            {
                ret = new HeavyCube();
                defaultTextureList = textureStore.heavyTextures;
            }

            foreach (Texture2D tex in defaultTextureList)
                pgo.getTextureSet("Default").addTexture(tex);


            foreach (Texture2D tex in textureStore.selectTextures)
                pgo.getTextureSet("Selected").addTexture(tex);

            foreach (Texture2D tex in textureStore.rocketTimer)
                pgo.getTextureSet("Countdown").addTexture(tex);

            pgo.addTextureSet("Countdown");


            physicsController.registerPhysicsGameObject(pgo);

            ret.physicalObject = pgo;
            return ret;


        }
    }
}
