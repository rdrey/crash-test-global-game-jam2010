using System;
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
        /*public static CubeNode createCubeNode(TextureStore textureStore, PhysicsController physicsController, CubeDescription cubeDescription, Vector2 cubeSize)
        {
            PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, cubeSize.X, cubeSize.Y, false);


            return createCubeNode(textureStore, physicsController, cubeDescription, cubeSize, pgo);
        }*/
        public static CubeNode createCubeNode(TextureStore textureStore, PhysicsController physicsController, CubeDescription cubeDescription, Vector2 cubeSize, CubeSet parent)
        {


            PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, cubeSize.X, cubeSize.Y, false);

            pgo.ID = parent.ID;

            //physicsController.deregisterPhysicsGameObject(pgo);
            //pgo.textures.Clear();

            List<Texture2D> defaultTextureList = new List<Texture2D>();

            CubeNode ret;

            //public enum CubeType { UnknownCube, PlainCube, RocketCube, ShieldCube, HeavyCube };

            if (cubeDescription.type == CubeType.UnknownCube)
            {
                ret = new UnknownCube();
                defaultTextureList = textureStore.unknownTextures;
                pgo.boxBody.Mass = 0.05f;
            }
            else if (cubeDescription.type == CubeType.ChainCube)
            {
                ret = new ChainCube();
                //todo TEXTURE
                pgo = new ChainPhysicsGameObject(physicsController.physicsSimulator);
                defaultTextureList = textureStore.plainTextures;
                pgo.getTextureSet("Default").scale.X *= 2;
                pgo.getTextureSet("Default").scale.Y *= 2;
                //ret.visualOnly = true;
                pgo.boxGeom.CollisionGroup = 13;
            }
            else if (cubeDescription.type == CubeType.PlainCube)
            {
                ret = new PlainCube();
                defaultTextureList = textureStore.plainTextures;
                pgo.boxBody.Mass = 1f;
            }
            else if (cubeDescription.type == CubeType.DamageCube)
            {
                ret = new DamageCube();
                defaultTextureList = textureStore.spike;
            }
            else if (cubeDescription.type == CubeType.RocketCube)
            {
                ret = new RocketCube(cubeDescription.dir);
                defaultTextureList = textureStore.rocketTextures;
                ((RocketCube)ret).activationCountdown = cubeDescription.value;

                foreach (Texture2D tex in textureStore.rocketFlame)
                    pgo.getTextureSet("MainFlame").addTexture(tex);

                pgo.getTextureSet("MainFlame").offset = new Vector2(cubeSize.X - 1, 0);


                foreach (Texture2D tex in textureStore.boomTextures)
                    pgo.getTextureSet("Boom").addTexture(tex);

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
                        rotation = MathHelper.Pi / 2;
                        break;
                    case Direction.West:
                        rotation = MathHelper.Pi;
                        break;
                    case Direction.North:
                        rotation = MathHelper.Pi * 3 / 2;
                        break;
                }
                pgo.boxBody.Rotation = rotation;


            }
            else if (cubeDescription.type == CubeType.ShooterCube)
            {
                ret = new ShooterCube(cubeDescription.dir);
                defaultTextureList = textureStore.shooterTextures;
                ((ShooterCube)ret).bulletTexture = textureStore.bulletTexture;
                ((ShooterCube)ret).activationCountdownTotal = cubeDescription.value;
                ((ShooterCube)ret).activationCountdown = cubeDescription.value;

                float rotation = 0.0f;
                switch (cubeDescription.dir)
                {
                    case Direction.East:
                        rotation = 0;
                        break;
                    case Direction.South:
                        rotation = MathHelper.Pi / 2;
                        break;
                    case Direction.West:
                        rotation = MathHelper.Pi;
                        break;
                    case Direction.North:
                        rotation = MathHelper.Pi * 3 / 2;
                        break;
                }
                pgo.boxBody.Rotation = rotation;



            }
            else if (cubeDescription.type == CubeType.BulletCube)
            {
                ret = new BulletCube();
                defaultTextureList = textureStore.bulletTexture;
                pgo.getTextureSet("Default").scale *= 2;
            }
            else if (cubeDescription.type == CubeType.ShieldCube)
            {
                ret = new ShieldCube();
                defaultTextureList = textureStore.shieldTextures;

                pgo.boxBody.Mass = 1f;

                foreach (Texture2D tex in textureStore.buzzTextures)
                    pgo.getTextureSet("Energize").addTexture(tex);
            }
            else// ; if (cubeDescription.type == CubeType.HeavyCube)
            {
                ret = new HeavyCube();
                pgo.boxBody.Mass = 5;
                defaultTextureList = textureStore.heavyTextures;
                pgo.boxBody.Mass = 2f;
            }

            foreach (Texture2D tex in defaultTextureList)
                pgo.getTextureSet("Default").addTexture(tex);


            foreach (Texture2D tex in textureStore.selectTextures)
                pgo.getTextureSet("Selected").addTexture(tex);

            pgo.addTextureSet("Countdown"); 
            
            foreach (Texture2D tex in textureStore.rocketTimer)
                pgo.getTextureSet("Countdown").addTexture(tex);

            for (int i = 1; i <= 8; i++)
            {
                string name = "coin" + i;
                pgo.getTextureSet(name).addTexture(textureStore.coinTexture);
                pgo.getTextureSet(name).offset.Y += cubeSize.Y + (i*20);
                if (cubeDescription.type == CubeType.ChainCube)
                {
                    pgo.getTextureSet(name).scale *= 40f;
                    pgo.getTextureSet(name).offset.X += cubeSize.X / 2;
                    pgo.getTextureSet(name).offset.Y += cubeSize.Y / 2;
                }
                
            }


            ret.cubeDescription = cubeDescription;

            physicsController.registerPhysicsGameObject(pgo);

            ret.physicalObject = pgo;
            ret.physicsController = physicsController;
            ret.parent = parent;
            ret.hackz0r();
            physicsController.nodeLookup[pgo] = ret;
            return ret;


        }
    }
}
