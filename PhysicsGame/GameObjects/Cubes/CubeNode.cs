using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Dynamics.Joints;

namespace PhysicsGame.GameObjects.Cubes
{
    //TODO put in separate class
    public enum Direction { North, South, West, East };

    public enum CubeType { UnknownCube, PlainCube, RocketCube, ShieldCube, HeavyCube, ChainCube, DamageCube, ShooterCube, BulletCube };

    //==============================================================
    //==============================================================
    //==============================================================

    public class CubeNode : GameObject
    {
        
        public CubeSet parent;
        public PhysicsController physicsController;

        public CubeDescription cubeDescription;

        public bool isTempNode = false;
        public Vector2 positionIndex;

        public bool visualOnly = false;

        //Dictionary<Direction, CubeNode> neighbours; // TODO implement if needed

        public float defaultAnimationSpeed = 1.0f;


        public PhysicsGameObject physicalObject;

        public float damageMultiplier = 1f;
        public int cost;

        public float hp;
        float _maxHp;

        public float maxHp
        {
            get { return _maxHp; }
            set { hp = _maxHp = value; }
        }


        public bool startedCountDown = false;
        public bool markForDelete = false;

        /* insert other properties here */

        protected bool lulz (Geom g1, Geom g2, ContactList p) 
        {
            //ModSound sounds = parent.sound;
            if (physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player1 &&
                        physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player2
                ||
                physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player1 &&
                        physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player2
                )
            {
                this.hp -= 20;
                if (this is BulletCube)
                    this.markForDelete = true;

                if (physicsController.geomLookup[g1].ID == this.physicalObject.ID)
                {
                    CubeNode other = physicsController.nodeLookup[
                        physicsController.geomLookup[g2]];
                    other.hp -= 20 * damageMultiplier;

                    if (other is BulletCube)
                        other.markForDelete = true;

                    if (PhysicsGameObject.PhysicsMapID.player1 == other.physicalObject.ID)
                    {
                        physicsController.damageTotalPlayer1 += 20 * damageMultiplier;
                        physicsController.damageTotalPlayer2 += 20;

                    }
                }
                else
                {
                    CubeNode other = physicsController.nodeLookup[
                        physicsController.geomLookup[g1]];
                    other.hp -= 20 * damageMultiplier;

                    if (other is BulletCube)
                        other.markForDelete = true;

                    if (PhysicsGameObject.PhysicsMapID.player2 == other.physicalObject.ID)
                    {
                        physicsController.damageTotalPlayer2 += 20 * damageMultiplier;
                        physicsController.damageTotalPlayer1 += 20;
                    }
                }
            }

            /*ModSound sounds = parent.sound;

            Vector2 position = p[0].Normal;

            float angle = (float)Math.Atan2(position.Y, position.X);

            Vector2 force = Vector2.Zero;
            if (angle < 0)
                force = new Vector2((float)(Math.Cos(angle) * g2.Body.LinearVelocity.X), (float)Math.Sin(MathHelper.TwoPi + angle) * g2.Body.LinearVelocity.Y);
            else
                force = new Vector2((float)(Math.Cos(angle) * g2.Body.LinearVelocity.X), (float)Math.Sin(MathHelper.TwoPi - angle) * g2.Body.LinearVelocity.Y);
            //try
            //{
                if (physicalObject.ID == PhysicsGameObject.PhysicsMapID.player1)
                {
                    if (physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player1 &&
                        physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player2)
                    {
                        hp -= (int)(damageMultiplier / 500f * force.LengthSquared());
                        Console.WriteLine("A " + hp);
                        //Console.WriteLine("some lulz have occurred p1 {0}", physicsController.geomLookup[g1].ID);
                    }


                }
                else
                {
                    if (physicsController.geomLookup[g1].ID == PhysicsGameObject.PhysicsMapID.player2 &&
                        physicsController.geomLookup[g2].ID == PhysicsGameObject.PhysicsMapID.player1)
                    {
                        hp -= (int)(damageMultiplier / 500f * force.LengthSquared());
                        Console.WriteLine("B " + hp);//Console.WriteLine("some lulz have occurred p2 {0}", physicsController.geomLookup[g1].ID);
                    }
                }
            //}
            //catch (KeyNotFoundException k)
            //{

            }*/
            return true;
        }

        protected CubeNode()
        {
            
        }

        public void hackz0r () 
        {
            physicalObject.boxGeom.OnCollision += lulz;
            physicalObject.boxGeom.OnCollision += parent.currentRound.OnCollision2;
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

        public void startSparking()
        {
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            //TODO remove this!!!
            //foreach (PhysicsGameJoint joint in this.physicalObject.joints)
            {
            //        ((PinJoint)joint.joint).TargetDistance = 60;
            }

            if (isTempNode)
            {
                for (int i = 1 ; i <= cost ; i++)
                    physicalObject.addTextureSet("coin" + i);
            }
            else
            {
                for (int i = 1; i <= cost; i++)
                    physicalObject.removeTextureSet("coin" + i);
            }

            if (hp < 0)
                markForDelete = true;
            if (hp < 50)
                startSparking();

            /*if (isTempNode) // TODO make use that awesome get set thing of c#'s so don't have to set this each step
                physicalObject.colorValue = new Color(Color.White, 0.25f);
            else
                physicalObject.colorValue = new Color(Color.White, 1.0f);*/

            physicalObject.colorValue = new Color(Color.White, (float)hp / (float)maxHp);
            if (isTempNode)
                physicalObject.colorValue.A /= 2;

            physicalObject.getTextureSet("Default").incrementIndex(defaultAnimationSpeed * speedAdjust);

            physicalObject.getTextureSet("Selected").incrementIndex(1.0f * speedAdjust);
            

        }
        

    }
}
