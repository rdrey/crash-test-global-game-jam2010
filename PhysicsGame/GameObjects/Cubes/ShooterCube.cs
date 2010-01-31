using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;
using FarseerGames.FarseerPhysics.Factories;

namespace PhysicsGame.GameObjects.Cubes
{
    class ShooterCube : CubeNode
    {
        bool shoot = false;
        int bullets;
        public float activationCountdownTotal = 0;
        public float activationCountdown = 0;
        public List<Texture2D>  bulletTexture = new List<Texture2D>();

        public Direction dir;

        public ShooterCube(Direction dir)
        {
            this.dir = dir;
            bullets = 3;
            maxHp = 100;
            defaultAnimationSpeed = 0.1f;

            cost = 8;
        }

        public void fire()
        {

            CubeNode node = CubeFactory.createCubeNode(parent.textureStore, physicsController, new CubeDescription(CubeType.BulletCube), new Vector2(20, 20), parent);
            //BulletCube bullet = new BulletCube();
            
            //PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, this.physicalObject.getWidth()/.6f, this.physicalObject.getHeight()/.6f, false);

            node.physicalObject.boxBody.Position = Vector2.Add(this.physicalObject.boxBody.Position, 
                                    Vector2.Transform(new Vector2(this.physicalObject.getWidth()+15, 0), 
                                    Matrix.CreateRotationZ(this.physicalObject.boxBody.Rotation - MathHelper.Pi)));
            
            
            
            //physicsController.nodeLookup[pgo] = bullet;
            //bullet.physicsController = this.physicsController;
            node.parent = this.parent;
            node.hackz0r();
            node.physicalObject.boxBody.ApplyForce(Vector2.Transform(new Vector2(10000, 0), Matrix.CreateRotationZ(this.physicalObject.boxBody.Rotation- MathHelper.Pi)));
            

            
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            if (startedCountDown)
            {
                if (activationCountdown > 0)
                    activationCountdown -= 1 * speedAdjust;
                else if (bullets > 0)
                {
                    fire();
                    bullets--;
                    if (bullets > 0)
                        activationCountdown = activationCountdownTotal;
                }
            }

            if (isTempNode) // TODO make use that awesome get set thing of c#'s so don't have to set this each step
                physicalObject.colorValue = new Color(Color.White, 0.25f);
            else
                physicalObject.colorValue = new Color(Color.White, 1.0f);


            if(this.activationCountdown/40>1)
                physicalObject.getTextureSet("Countdown").currentTextureListIndex=this.activationCountdown/40+1;
            else
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = 0;
            

            base.Update(gameTime, speedAdjust);

            physicalObject.getTextureSet("Default").currentTextureListIndex = bullets;
        }

    }
}
