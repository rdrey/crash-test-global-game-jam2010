using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysicsGame.GameObjects;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Collisions;

namespace PhysicsGame
{
    class PhysicsController
    {
        public PhysicsSimulator physicsSimulator;
        public LinkedList<PhysicsGameObject> physicsObjects;
        public LinkedList<PhysicsGameJoint> physicsJoints;

        public Dictionary<Geom, PhysicsGameObject> geomLookup;

        public PhysicsController()
        {
            physicsSimulator = new PhysicsSimulator(new Vector2(0, 0));
            physicsObjects = new LinkedList<PhysicsGameObject>();
            physicsJoints = new LinkedList<PhysicsGameJoint>();
            geomLookup = new Dictionary<Geom, PhysicsGameObject>();
        }

        public void registerPhysicsGameObject(PhysicsGameObject pgo)
        {
            physicsObjects.AddLast(pgo);
            geomLookup[pgo.boxGeom] = pgo;
        }

        public void deregisterPhysicsGameObject(PhysicsGameObject pgo)
        {
            physicsObjects.Remove(pgo);
        }

        public void registerPhysicsGameJoint(PhysicsGameJoint pgj)
        {
            physicsJoints.AddLast(pgj);
        }

        public void deregisterPhysicsGameJoint(PhysicsGameJoint pgj)
        {
            physicsJoints.Remove(pgj);

        }
    }
}
