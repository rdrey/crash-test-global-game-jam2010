using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;
using FarseerGames.FarseerPhysics.Factories;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects
{
    class PhysicsGameJoint : Drawable
    {
        public Joint joint;

        public PhysicsGameJoint(PhysicsSimulator physicsSimulator, PhysicsGameObject obj1, PhysicsGameObject obj2)
        {
            joint = JointFactory.Instance.CreateRevoluteJoint(physicsSimulator, obj1.boxBody, obj2.boxBody, obj1.boxBody.Position);
            obj1.joints.Add(this);
            obj2.joints.Add(this);
        }


        public void draw(SpriteBatch spriteBatch)
        {
        }


        public void draw(BasicEffect basicEffect, GraphicsDevice graphicsDevice)
        {

            //Vector2[] pointsList = boxGeom.WorldVertices.ToArray();

            VertexPositionColor[] verticies = new VertexPositionColor[2];
            verticies[0] = new VertexPositionColor(new Vector3(((RevoluteJoint)joint).Body1.Position, 0), Color.Blue);
            verticies[1] = new VertexPositionColor(new Vector3(((RevoluteJoint)joint).Body2.Position, 0), Color.Blue);


            /*for (int i = 0; i < pointsList.Length; i++)
            {
                verticies[i] = new VertexPositionColor(new Vector3(pointsList[i], 0), Color.Black);
            }
            verticies[verticies.Length - 1] = new VertexPositionColor(new Vector3(pointsList[0], 0), Color.Black);*/

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineList,
                    verticies, 0, verticies.Length - 1
                );
                pass.End();
            }
        }
    }
}
