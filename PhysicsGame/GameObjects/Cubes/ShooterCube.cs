using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace PhysicsGame.GameObjects.Cubes
{
    class ShooterCube : CubeNode
    {
        bool shoot = false;
        int bullets;
        public float activationCountdown = 0;
        public List<Texture2D>  bulletTexture = new List<Texture2D>();

        public Direction dir;

        public ShooterCube(Direction dir)
        {
            this.dir = dir;
            bullets = 3;
            maxHp = 100;
            defaultAnimationSpeed = 0.1f;
        }

        public void fire()
        {
            BulletCube bullet = new BulletCube();
            
            PhysicsGameObject pgo = new PhysicsGameObject(physicsController.physicsSimulator, this.physicalObject.getWidth(), this.physicalObject.getHeight(), false);
            foreach (Texture2D tex in bulletTexture)
                pgo.getTextureSet("Default").addTexture(tex);


            physicsController.registerPhysicsGameObject(pgo);
            pgo.boxBody.Position = Vector2.Add(this.physicalObject.boxBody.Position, Vector2.Transform(new Vector2(this.physicalObject.getWidth(), 0), Matrix.CreateRotationZ(this.physicalObject.boxBody.Rotation - MathHelper.Pi)));
            pgo.ID = this.physicalObject.ID;
            
            bullet.physicalObject=pgo;

            bullet.physicsController = this.physicsController;

            bullet.physicalObject.boxBody.ApplyForce(Vector2.Transform(new Vector2(10000, 0), Matrix.CreateRotationZ(this.physicalObject.boxBody.Rotation- MathHelper.Pi)));
            

            
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
                        activationCountdown = 550;
                }
            }

            if (isTempNode) // TODO make use that awesome get set thing of c#'s so don't have to set this each step
                physicalObject.colorValue = new Color(Color.White, 0.25f);
            else
                physicalObject.colorValue = new Color(Color.White, 1.0f);


            if(this.activationCountdown/40>1)
                physicalObject.getTextureSet("Countdown").currentTextureListIndex=this.activationCountdown/40+1;
            else if (true)
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = 0;
            else
            {
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = 1;
            }

            base.Update(gameTime, speedAdjust);

            physicalObject.getTextureSet("Default").currentTextureListIndex = bullets;
        }

    }
}
