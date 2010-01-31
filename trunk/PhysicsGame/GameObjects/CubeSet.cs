using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using PhysicsGame.GameObjects.Cubes;

namespace PhysicsGame.GameObjects
{
    



    public class CubeSet : GameObject
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

        public bool finishedEditing = false;

        PhysicsController physicsController;
        public Dictionary<Vector2, CubeNode> cubeLookUp;
        //Dictionary<CubeSet, IntVector2> poseLookUp;

        TextureStore textureStore;
        PhysicsGameObject.PhysicsMapID ID = PhysicsGameObject.PhysicsMapID.anything;
        public ModSound sound;

        CubeDescription currentCubeDescription = new CubeDescription();

        public CubeSet(PhysicsController physicsController, TextureStore textureStore, Vector2 position, int playnum, ModSound s)
        {
            this.textureStore = textureStore;
            this.physicsController = physicsController;
            sound = s;
            if (playnum == 1) ID = PhysicsGameObject.PhysicsMapID.player1;
            else ID = PhysicsGameObject.PhysicsMapID.player2;
            CubeNode rootNode = createNode(new CubeDescription(CubeType.PlainCube));
            rootNode.physicalObject.boxBody.Position = position;

            cubeLookUp = new Dictionary<Vector2, CubeNode>();
            rootNode.positionIndex = new Vector2();
            cubeLookUp[rootNode.positionIndex] = rootNode;

            changeSelecetedNode(rootNode.positionIndex);

            rootNode.physicalObject.boxBody.Position = position;

        }

        public void startActivationCountdowns()
        {
            foreach (KeyValuePair<Vector2, CubeNode> node in cubeLookUp)
                node.Value.startedCountDown = true;
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

        public void deselectAll()
        {
            if (cubeLookUp[selectedCube.Value].isTempNode)
            {
                cubeLookUp[selectedCube.Value].markForDelete = true;
            }
            cubeLookUp[selectedCube.Value].deselect();
        }

        enum Option {NoOption, Type, Option1, Option2};

        private bool changeSelecetedNode(Vector2 newIndexPosition)
        {
            return changeSelecetedNode(newIndexPosition, Option.NoOption);
        }

        private bool changeSelecetedNode(Vector2 newIndexPosition, Option optionToCycle)
        {
            bool justChangingType = (selectedCube == newIndexPosition);

            if (!justChangingType && cubeLookUp.ContainsKey(newIndexPosition))
            {
                Nullable<Vector2> oldIndex = selectedCube;
                changeSelection(selectedCube, newIndexPosition);
                if (oldIndex.HasValue)
                {
                    if (getNodeAt(oldIndex.Value).isTempNode) // current selection is temporary
                    {
                        //delete oldIndex
                        getNodeAt(oldIndex.Value).markForDelete = true;

                    }
                }


                return true;
            }
            else // there is no cube at the new position
            {
                if (justChangingType /*then it def is temporary*/ || !getSelectedNode().isTempNode) // current selection is not temporary
                {
                    // then add a temporary block at new position

                    if (justChangingType)
                    {
                        if (optionToCycle == Option.Type)
                        {
                            if (getSelectedNode() is UnknownCube)
                                currentCubeDescription.type = CubeType.RocketCube;
                            else if (getSelectedNode() is RocketCube)
                                currentCubeDescription.type = CubeType.PlainCube;
                            else if (getSelectedNode() is PlainCube)
                                currentCubeDescription.type = CubeType.ChainCube;
                            else if (getSelectedNode() is ChainCube)
                                currentCubeDescription.type = CubeType.ShieldCube;
                            else if (getSelectedNode() is ShieldCube)
                                currentCubeDescription.type = CubeType.HeavyCube;
                            else if (getSelectedNode() is HeavyCube)
                                currentCubeDescription.type = CubeType.UnknownCube;


                        }

                        if (optionToCycle == Option.Option1)
                        {
                            if (getSelectedNode() is RocketCube)
                            {
                                if (((RocketCube)getSelectedNode()).dir == Direction.North)
                                    currentCubeDescription.dir = Direction.East;
                                else if (((RocketCube)getSelectedNode()).dir == Direction.East)
                                    currentCubeDescription.dir = Direction.South;
                                else if (((RocketCube)getSelectedNode()).dir == Direction.South)
                                    currentCubeDescription.dir = Direction.West;
                                else if (((RocketCube)getSelectedNode()).dir == Direction.West)
                                    currentCubeDescription.dir = Direction.North;
                            }
                        }


                        cleanUpNodeAt(selectedCube.Value);
                        //replaceCubeNodeAndCleanOld(selectedCube.Value, new CubeDescription(type));
                    }
                    //else
                    {

                        addCubeNodeFromTo(selectedCube.Value, newIndexPosition, currentCubeDescription);
                        getNodeAt(newIndexPosition).isTempNode = true;

                        changeSelection(selectedCube, newIndexPosition);
                    }

                    return true;
                }
                else // current selection is temporary
                {

                    return false;
                }
            }
        }

        public void replaceCubeNodeAndCleanOld(Vector2 replacePosition, CubeDescription cubeDescription)
        {
            cleanUpNodeAt(replacePosition);
            CubeNode toReplace = createNode(cubeDescription);

            cubeLookUp[replacePosition] = toReplace;
            if (selectedCube.Value == replacePosition)
            {
                getSelectedNode().deselect();
                toReplace.select();
            }
        }


        public void changeSelectedNode(Direction dir)
        {
            changeSelecetedNode(adjacentIndex(selectedCube.Value, dir));
        }


        public void makeCurrentSelectionPermanent()
        {
            if (getSelectedNode().isTempNode)
            {
                getSelectedNode().isTempNode = false;
            }
        }

        public void cycleSelectedNode()
        {
            if (getSelectedNode().isTempNode)
                changeSelecetedNode(selectedCube.Value, Option.Type);
        }

        public void cycleOption1()
        {
            if (getSelectedNode().isTempNode)
                changeSelecetedNode(selectedCube.Value, Option.Option1);
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {

            /*foreach (KeyValuePair<Vector2, CubeNode> node in cubeLookUp)
            {
            }*/

            List<Vector2> keys = new List<Vector2>(cubeLookUp.Keys);

            foreach (Vector2 key in keys)
            {
                cubeLookUp[key].Update(gameTime, speedAdjust);
                if (cubeLookUp[key].markForDelete)
                {
                    cleanUpNodeAt(key);

                }
            }
        }

        public void cleanUpNodeAt(Vector2 key) {

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

        public CubeNode createNode(CubeDescription cubeDescription)
        {
            CubeNode temp = CubeFactory.createCubeNode(textureStore, physicsController, cubeDescription, cubeSize);
            temp.physicalObject.ID = ID;
            temp.parent = this;
            return temp;
        }

        public CubeNode createNode(CubeDescription cubeDescription, PhysicsGameObject pgo)
        {
            CubeNode temp = CubeFactory.createCubeNode(textureStore, physicsController, cubeDescription, cubeSize);
            temp.physicalObject.ID = ID;
            return temp;
        }

        public bool addCubeNodeAtPossible(Vector2 targetPosition)
        {
            if (cubeLookUp.ContainsKey(targetPosition))
            {
                //if (cubeLookUp[targetPosition].markForDelete)
                //    return true; // it's gonna be deleted so it's fine to adda a node
                return false; // already has a block in target position
            }
            else
                return true;
        }

        public bool addCubeNodePossible(Direction dir, CubeNode fromNode)
        {
            return addCubeNodeAtPossible(adjacentIndex(fromNode.positionIndex, dir));
        }

        public bool addCubeNodeFromTo(Vector2 fromPosition, Vector2 targetPosition, CubeDescription cubeDescription)
        {
            CubeNode nodeToAdd = createNode(cubeDescription);
            if (!addCubeNodeAtPossible(targetPosition))
                return false; // already has a block in target position
            else
            {
                nodeToAdd.positionIndex = targetPosition;
                cubeLookUp[targetPosition] = nodeToAdd;

                if (cubeDescription.type == CubeType.ChainCube)
                {
                    ChainCube chain = (ChainCube)nodeToAdd;
                    ChainPhysicsGameObject chainPhys = (ChainPhysicsGameObject)chain.physicalObject;
                    for (int i = 0; i < 10; i++)
                        chainPhys.path.Add(new Vector2(
                            fromPosition.X + (i * (targetPosition.X-fromPosition.X)/10f),
                            fromPosition.Y + (i * (targetPosition.Y-fromPosition.Y)/10f)));
                    chainPhys.makeLink();

                }
                nodeToAdd.physicalObject.boxBody.Position =
                        getRootNode().physicalObject.boxBody.Position + getRealPosition(targetPosition, cubeSize);

                // joining up blocks (from fromPosition to targetPosition)
                //physicsController.registerPhysicsGameJoint(new PhysicsGameJoint(physicsController.physicsSimulator, this.cubeLookUp[fromPosition].physicalObject, nodeToAdd.physicalObject));

                foreach (Direction dir in Enum.GetValues(typeof(Direction)))
                {
                    Vector2 neighbourIndex = adjacentIndex(targetPosition, dir);
                    if (cubeLookUp.ContainsKey(neighbourIndex)) // TODO check for duplicate joints
                        physicsController.registerPhysicsGameJoint(new PhysicsGameJoint(physicsController.physicsSimulator, cubeLookUp[neighbourIndex].physicalObject, nodeToAdd.physicalObject));
                }

                return true;
            }
        }

        public bool addCubeNodeFrom(Vector2 fromPosition, Direction dir, CubeDescription cubeDescription)
        {
            return addCubeNodeFromTo(fromPosition, adjacentIndex(cubeLookUp[fromPosition].positionIndex, dir), cubeDescription);
        }




    }
}
