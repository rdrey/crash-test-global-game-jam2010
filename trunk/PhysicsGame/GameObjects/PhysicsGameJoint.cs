using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsGame.GameObjects
{
    class PhysicsGameJoint : Drawable
    {
        Joint joint;

        public PhysicsGameJoint(PhysicsSimulator physicsSimulator, PhysicsGameObject obj1, PhysicsGameObject obj2)
        {
            joint = JointFactory.Instance.CreateRevoluteJoint(physicsSimulator, obj1.boxBody, obj2.boxBody, obj1.boxBody.Position); 

        }


        public void draw(SpriteBatch spriteBatch)
        {
        }


        public void draw(BasicEffect basicEffect, GraphicsDevice graphicsDevice)
        {
        }
    }
}
