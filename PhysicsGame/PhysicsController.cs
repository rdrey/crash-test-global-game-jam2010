using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysicsGame.GameObjects;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;

namespace PhysicsGame
{
    class PhysicsController
    {
        public PhysicsSimulator physicsSimulator;
        public LinkedList<PhysicsGameObject> physicsObjects;
        public LinkedList<PhysicsGameJoint> physicsJoints;

        public PhysicsController()
        {
            physicsSimulator = new PhysicsSimulator(new Vector2(0, 0));
            physicsObjects = new LinkedList<PhysicsGameObject>();
            physicsJoints = new LinkedList<PhysicsGameJoint>();
        }

        public void registerPhysicsGameObject(PhysicsGameObject pgo)
        {
            physicsObjects.AddLast(pgo);
        }

        public void registerPhysicsGameJoint(PhysicsGameJoint pgj)
        {
            physicsJoints.AddLast(pgj);
        }
    }
}
