using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PhysicsGame.GameObjects;
using FarseerGames.FarseerPhysics;
using Microsoft.Xna.Framework;
using FarseerGames.FarseerPhysics.Collisions;
using PhysicsGame.GameObjects.Cubes;

namespace PhysicsGame
{
    public class PhysicsController
    {
        public PhysicsSimulator physicsSimulator;
        public LinkedList<PhysicsGameObject> physicsObjects;
        public List<PhysicsGameObject> visuals;
        public LinkedList<PhysicsGameJoint> physicsJoints;

        public Dictionary<Geom, PhysicsGameObject> geomLookup;
        public Dictionary<PhysicsGameObject, CubeNode> nodeLookup;
        public Dictionary<Geom, int> geomSndLookup;
        public Dictionary<Geom, PhysicsGameObject.PhysicsMapID> IDLookup;

        public float damageTotalPlayer1 = 0;
        public float damageTotalPlayer2 = 0;

        public PhysicsController()
        {
            physicsSimulator = new PhysicsSimulator(new Vector2(0, 0));
            physicsObjects = new LinkedList<PhysicsGameObject>();
            visuals = new List<PhysicsGameObject>();
            physicsJoints = new LinkedList<PhysicsGameJoint>();
            geomLookup = new Dictionary<Geom, PhysicsGameObject>();
            nodeLookup = new Dictionary<PhysicsGameObject, CubeNode>();
            geomSndLookup = new Dictionary<Geom, int>();
            IDLookup = new Dictionary<Geom, PhysicsGameObject.PhysicsMapID>();
        }

        public void registerPhysicsGameObject(PhysicsGameObject pgo)
        {
            physicsObjects.AddLast(pgo);
            geomLookup[pgo.boxGeom] = pgo;
            geomSndLookup[pgo.boxGeom] = 0;
            IDLookup[pgo.boxGeom] = pgo.ID;
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
