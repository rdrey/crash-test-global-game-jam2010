using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Collisions;

namespace PhysicsGame.GameObjects.Cubes
{
    //TODO put in separate class
    public enum Direction { North, South, West, East };

    public enum CubeType { UnknownCube, PlainCube, RocketCube, ShieldCube, HeavyCube };

    //==============================================================
    //==============================================================
    //==============================================================

    class CubeNode : GameObject
    {
        
        public CubeSet parent;
        public PhysicsController physicsController;

        public bool isTempNode = false;
        public Vector2 positionIndex;

        //Dictionary<Direction, CubeNode> neighbours; // TODO implement if needed

        public float defaultAnimationSpeed = 1.0f;


        public PhysicsGameObject physicalObject;

        public int hp;
        int _maxHp;

        public int maxHp
        {
            get { return _maxHp; }
            set { hp = _maxHp = value; }
        }


        public bool startedCountDown = false;
        public bool markForDelete = false;

        /* insert other properties here */

        protected bool lulz (Geom g1, Geom g2, ContactList p) 
        {
            ModSound sounds = parent.sound;

            Vector2 position = p[0].Normal;

            float angle = (float)Math.Atan2(position.Y, position.X);

            Vector2 force = Vector2.Zero;
            if (angle < 0)
                force = new Vector2((float)(Math.Cos(angle) * g2.Body.LinearVelocity.X), (float)Math.Sin(MathHelper.TwoPi + angle) * g2.Body.LinearVelocity.Y);
            else
                force = new Vector2((float)(Math.Cos(angle) * g2.Body.LinearVelocity.X), (float)Math.Sin(MathHelper.TwoPi - angle) * g2.Body.LinearVelocity.Y);

            if (physicalObject.ID == PhysicsGameObject.PhysicsMapID.player1)
            {
                if (physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player1 &&
                    physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player2)
                {
                    hp -= (int)(.0025f * force.LengthSquared());//Console.WriteLine("some lulz have occurred p1 {0}", physicsController.geomLookup[g1].ID);
                    sounds.playSound("bang", g2, Vector2.One, force.LengthSquared() / 10000f);
                }
            }
            else
            {
                if (physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player2 &&
                    physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player1)
                {
                    hp -= (int)(.0025f * force.LengthSquared());//Console.WriteLine("some lulz have occurred p2 {0}", physicsController.geomLookup[g1].ID);
                    sounds.playSound("bang", g2, Vector2.One, force.LengthSquared() / 10000f);
                }
            }
            
            return true;
        }

        protected CubeNode()
        {
        }

        public void hackz0r () 
        {
            physicalObject.boxGeom.OnCollision += lulz;
        }

        public void select()
        {
            physicalObject.addTextureSet("Selected");

        }
        public void deselect()
        {
            physicalObject.removeTextureSet("Selected");
            physicalObject.getTextureSet("Selected").currentTextureListIndex = 0;
        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {

            if (hp < 0)
                markForDelete = true;

            if (isTempNode) // TODO make use that awesome get set thing of c#'s so don't have to set this each step
                physicalObject.colorValue = new Color(Color.White, 0.25f);
            else
                physicalObject.colorValue = new Color(Color.White, 1.0f);

            physicalObject.colorValue = new Color(Color.White, (float)hp / (float)maxHp);

                physicalObject.getTextureSet("Default").incrementIndex(defaultAnimationSpeed * speedAdjust);

                physicalObject.getTextureSet("Selected").incrementIndex(1.0f * speedAdjust);
            

        }
        

    }
}
