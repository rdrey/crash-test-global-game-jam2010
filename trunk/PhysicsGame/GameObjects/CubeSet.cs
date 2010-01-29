using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsGame.GameObjects
{
    



    class CubeSet
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


        PhysicsController physicsController;
        Dictionary<Vector2, CubeNode> cubeLookUp;
        //Dictionary<CubeSet, IntVector2> poseLookUp;


        public CubeSet(PhysicsController physicsController, Texture2D tex, Vector2 position)
        {
            this.physicsController = physicsController;
            CubeNode rootNode = createNode(tex);
            rootNode.physicalObject.boxBody.Position = position;

            cubeLookUp = new Dictionary<Vector2, CubeNode>();
            rootNode.positionIndex = new Vector2();
            cubeLookUp[rootNode.positionIndex] = rootNode;

            rootNode.physicalObject.boxBody.Position = position;

        }

        public CubeNode getRootNode()
        {
            return cubeLookUp[new Vector2()];
        }

        public CubeNode createNode(Texture2D tex)
        {
            PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, cubeSize.X, cubeSize.Y, false);
            pgo.addTexture(tex);
            pgo.setScaleToFit();

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
