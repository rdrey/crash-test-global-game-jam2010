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
        
        //CubeSet parent;
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
            if (physicalObject.ID == PhysicsGameObject.PhysicsMapID.player1)
            {
                if (physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player1 &&
                    physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player2)
                    hp -= 10;//Console.WriteLine("some lulz have occurred p1 {0}", physicsController.geomLookup[g1].ID);
            }
            else
            {
                if (physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player2 &&
                    physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player1)
                    hp -= 10;//Console.WriteLine("some lulz have occurred p2 {0}", physicsController.geomLookup[g1].ID);
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
