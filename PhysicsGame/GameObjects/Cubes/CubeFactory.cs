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
                ret.activationCountdown = 200;
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


            physicsController.registerPhysicsGameObject(pgo);

            ret.physicalObject = pgo;
            return ret;


        }
    }
}
