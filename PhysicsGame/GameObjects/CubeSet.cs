﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsGame.GameObjects
{
    



    class CubeSet : GameObject
    {

        public static Vector2 adjacentIndex(Vector2 orig, Direction dir)
        {

            if (dir == Direction.North)
                return new Vector2(orig.X, orig.Y + 1);
            else if (dir == Direction.South)
                return new Vector2(orig.X, orig.Y - 1);
            else if (dir == Direction.East)
                return new Vector2(orig.X + 1, orig.Y);
            else// if (dir == Direction.West)
                return new Vector2(orig.X - 1, orig.Y);
        }

        public static Vector2 getRealPosition(Vector2 orig, Vector2 cubeSize)
        {
            return new Vector2(orig.X * cubeSize.X, orig.Y * cubeSize.Y);
        }

        static Vector2 cubeSize = new Vector2(20,20);

        Vector2 selectedCube;

        PhysicsController physicsController;
        public Dictionary<Vector2, CubeNode> cubeLookUp;
        //Dictionary<CubeSet, IntVector2> poseLookUp;


        public CubeSet(PhysicsController physicsController, TextureStore textureStore, Vector2 position)
        {
            this.physicsController = physicsController;
            CubeNode rootNode = createNode(textureStore);
            rootNode.physicalObject.boxBody.Position = position;

            cubeLookUp = new Dictionary<Vector2, CubeNode>();
            rootNode.positionIndex = new Vector2();
            cubeLookUp[rootNode.positionIndex] = rootNode;

            selectedCube = rootNode.positionIndex;

            rootNode.physicalObject.addTextureSet("Selected");

            rootNode.physicalObject.boxBody.Position = position;

        }

        public void Update() {

            foreach (KeyValuePair<Vector2, CubeNode> node in cubeLookUp)
            {
                node.Value.Update();
            }

            List<Vector2> keys = new List<Vector2>(cubeLookUp.Keys);

            foreach (Vector2 key in keys)
            {
                if (cubeLookUp[key].markForDelete)
                {
                    physicsController.deregisterPhysicsGameObject(cubeLookUp[key].physicalObject);
                    cubeLookUp[key].physicalObject.boxBody.Dispose();

                    cubeLookUp.Remove(key);


                    //physicsController.physicsSimulator.Remove(node.Value.physicalObject.boxGeom);

                }
            }
        }

        public bool isNodeAt(int x, int y)
        {
            return cubeLookUp.ContainsKey(new Vector2(x, y));
        }


        public CubeNode getNodeAt(int x, int y)
        {
            return cubeLookUp[new Vector2(x, y)];
        }

        public CubeNode getRootNode()
        {
            return cubeLookUp[new Vector2()];
        }

        public CubeNode createNode(TextureStore textureStore)
        {
            PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, cubeSize.X, cubeSize.Y, false);
            pgo.getTextureSet("Default").addTexture(textureStore.selectTextures[0]);
            pgo.getTextureSet("Default").addTexture(textureStore.rocketTextures[0]);
            pgo.getTextureSet("Default").setScaleToFit();

            pgo.getTextureSet("Selected").addTexture(textureStore.rocketTextures[0]);
            pgo.getTextureSet("Selected").addTexture(textureStore.selectTextures[0]);
            pgo.getTextureSet("Selected").setScaleToFit();

            //pgo.removeTextureSet("Default");

            physicsController.registerPhysicsGameObject(pgo);

            return new CubeNode(pgo);
        }

        public bool addCubeNodeAtPossible(Vector2 targetPosition)
        {
            if (cubeLookUp.ContainsKey(targetPosition))
                return false; // already has a block in target position
            else
                return true;
        }

        public bool addCubeNodePossible(Direction dir, CubeNode fromNode)
        {
            return addCubeNodeAtPossible(adjacentIndex(fromNode.positionIndex, dir));
        }

        public bool addCubeNodeAt(Vector2 targetPosition, CubeNode nodeToAdd)
        {
            if (!addCubeNodeAtPossible(targetPosition))
                return false; // already has a block in target position
            else
            {
                nodeToAdd.positionIndex = targetPosition;
                cubeLookUp[targetPosition] = nodeToAdd;

                nodeToAdd.physicalObject.boxBody.Position = getRootNode().physicalObject.boxBody.Position+getRealPosition(targetPosition, cubeSize);
                // TODO change toNode's position and join to adjacent nodes

                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Vector2 neighbourIndex = adjacentIndex(targetPosition, dir);
                    if (cubeLookUp.ContainsKey(neighbourIndex))
                        physicsController.registerPhysicsGameJoint(new PhysicsGameJoint(physicsController.physicsSimulator, cubeLookUp[neighbourIndex].physicalObject, nodeToAdd.physicalObject));
                }

                return true;
            }
        }

        public bool addCubeNode(Direction dir, CubeNode fromNode, CubeNode toNode)
        {
            return addCubeNodeAt(adjacentIndex(fromNode.positionIndex, dir), toNode);
        }




    }
}