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

        public PhysicsController()
        {
            physicsSimulator = new PhysicsSimulator(new Vector2(0, -100));
            physicsObjects = new LinkedList<PhysicsGameObject>();
        }

        public void registerPhysicsGameObject(PhysicsGameObject pgo)
        {
            physicsObjects.AddLast(pgo);
        }
    }
}
