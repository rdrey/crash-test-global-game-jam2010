﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;

namespace PhysicsGame.GameObjects
{
    



    class CubeSet : GameObject
    {

        public static Vector2 adjacentIndex(Vector2 orig, Direction dir)
        {
            if (dir == Direction.North)
                return new Vector2(orig.X, orig.Y - 1);
            else if (dir == Direction.South)
                return new Vector2(orig.X, orig.Y + 1);
            else if (dir == Direction.East)
                return new Vector2(orig.X + 1, orig.Y);
            else// if (dir == Direction.West)
                return new Vector2(orig.X - 1, orig.Y);
        }

        public static Vector2 getRealPosition(Vector2 orig, Vector2 cubeSize)
        {
            return new Vector2(orig.X * cubeSize.X, orig.Y * cubeSize.Y);
        }

        static Vector2 cubeSize = new Vector2(50,50);

        public Nullable<Vector2> selectedCube = null;

        PhysicsController physicsController;
        public Dictionary<Vector2, CubeNode> cubeLookUp;
        //Dictionary<CubeSet, IntVector2> poseLookUp;

        TextureStore textureStore;

        public CubeSet(PhysicsController physicsController, TextureStore textureStore, Vector2 position)
        {
            this.textureStore = textureStore;
            this.physicsController = physicsController;
            CubeNode rootNode = createNode();
            rootNode.physicalObject.boxBody.Position = position;

            cubeLookUp = new Dictionary<Vector2, CubeNode>();
            rootNode.positionIndex = new Vector2();
            cubeLookUp[rootNode.positionIndex] = rootNode;

            changeSelecetedNode(rootNode.positionIndex);

            rootNode.physicalObject.boxBody.Position = position;

        }

        public CubeNode getSelectedNode()
        {
            return cubeLookUp[selectedCube.Value];
        }

        private void changeSelection(Nullable<Vector2> fromNodePosition, Vector2 toNodePosition)
        {
            if (fromNodePosition.HasValue)
                cubeLookUp[fromNodePosition.Value].deselect();

            selectedCube = toNodePosition;
            cubeLookUp[selectedCube.Value].select();

        }

        private bool changeSelecetedNode(Vector2 newIndexPosition)
        {
            if (cubeLookUp.ContainsKey(newIndexPosition))
            {
                Nullable<Vector2> oldIndex = selectedCube;
                changeSelection(selectedCube, newIndexPosition);
                if (oldIndex.HasValue)
                {
                    if (getNodeAt(oldIndex.Value).isTempNode)// current selection is temporary
                    {
                        //delete oldIndex
                        getNodeAt(oldIndex.Value).markForDelete = true;

                    }
                }


                return true;
            }
            else // there is no cube at the new position
            {
                if (!getSelectedNode().isTempNode) // current selection is not temporary
                {
                    // then add a temporary block at new position
                    addCubeNodeFromTo(selectedCube.Value, newIndexPosition);
                    getNodeAt(newIndexPosition).isTempNode = true;

                    changeSelection(selectedCube, newIndexPosition);

                    return true;
                }
                else // current selection is temporary
                {

                    return false;
                }
            }
        }


        public void makeCurrentSelectionPermanent()
        {
            if (getSelectedNode().isTempNode)
            {
                getSelectedNode().isTempNode = false;
            }
        }

        public void changeSelectedNode(Direction dir)
        {
            changeSelecetedNode(adjacentIndex(selectedCube.Value, dir));
        }

        public void Update() {

            /*foreach (KeyValuePair<Vector2, CubeNode> node in cubeLookUp)
            {
            }*/

            List<Vector2> keys = new List<Vector2>(cubeLookUp.Keys);

            foreach (Vector2 key in keys)
            {
                cubeLookUp[key].Update();
                if (cubeLookUp[key].markForDelete)
                {
                    //delete all joints

                    foreach (PhysicsGameJoint joint in cubeLookUp[key].physicalObject.joints)
                    {
                        physicsController.deregisterPhysicsGameJoint(joint);
                        joint.joint.Dispose();
                    }

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

        public bool isNodeAt(Vector2 pos)
        {
            return cubeLookUp.ContainsKey(pos);
        }


        public CubeNode getNodeAt(int x, int y)
        {
            return cubeLookUp[new Vector2(x, y)];
        }

        public CubeNode getNodeAt(Vector2 pos)
        {
            return cubeLookUp[pos];
        }

        public CubeNode getRootNode()
        {
            return cubeLookUp[new Vector2()];
        }

        public CubeNode createNode()
        {
            PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, cubeSize.X, cubeSize.Y, false);

            foreach (Texture2D tex in textureStore.shieldTextures)
            {
                for (int i = 0; i < 6; i++ )
                    pgo.getTextureSet("Default").addTexture(tex);
            }

            foreach (Texture2D tex in textureStore.selectTextures)
            {
                pgo.getTextureSet("Selected").addTexture(tex);
            }

            //pgo.removeTextureSet("Default");
            //pgo.addTextureSet("Selected");
            //pgo.addTextureSet("Default");

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

        public bool addCubeNodeFromTo(Vector2 fromPosition, Vector2 targetPosition)
        {
            CubeNode nodeToAdd = createNode();
            if (!addCubeNodeAtPossible(targetPosition))
                return false; // already has a block in target position
            else
            {
                nodeToAdd.positionIndex = targetPosition;
                cubeLookUp[targetPosition] = nodeToAdd;

                // TODO FIXME only works if the objects have not rotated!!
                nodeToAdd.physicalObject.boxBody.Position = getRootNode().physicalObject.boxBody.Position+getRealPosition(targetPosition, cubeSize);
                

                // joining up blocks (from fromPosition to targetPosition)
                physicsController.registerPhysicsGameJoint(new PhysicsGameJoint(physicsController.physicsSimulator, this.cubeLookUp[fromPosition].physicalObject, nodeToAdd.physicalObject));

                /*foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Vector2 neighbourIndex = adjacentIndex(targetPosition, dir);
                    if (cubeLookUp.ContainsKey(neighbourIndex)) // TODO check for duplicate joints
                        physicsController.registerPhysicsGameJoint(new PhysicsGameJoint(physicsController.physicsSimulator, cubeLookUp[neighbourIndex].physicalObject, nodeToAdd.physicalObject));
                }*/

                return true;
            }
        }

        public bool addCubeNodeFrom(Vector2 fromPosition, Direction dir)
        {
            return addCubeNodeFromTo(fromPosition, adjacentIndex(cubeLookUp[fromPosition].positionIndex, dir));
        }




    }
}
