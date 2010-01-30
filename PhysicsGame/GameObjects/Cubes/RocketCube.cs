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
    class RocketCube : CubeNode
    {
        bool rocketsFireing = false;
        public float activationCountdown = 0;


        public RocketCube()
        {
            maxHp = 100;
            defaultAnimationSpeed = .0f;
        }

        public void activate()
        {
            rocketsFireing = true;
            this.physicalObject.addTextureSet("MainFlame");

            // enable fire overlay
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            if (startedCountDown)
            {
                if (activationCountdown <= 0)
                    activate();
                else
                    activationCountdown -= 1 * speedAdjust;
            }

            if (rocketsFireing)
            {
                float forceToApply = 10.0f / speedAdjust;
                physicalObject.boxBody.ApplyForce(Vector2.Transform(new Vector2(100,0), Matrix.CreateRotationZ(physicalObject.boxBody.Rotation))); // fire ze missiles! (but I am le tired)

            }


            physicalObject.getTextureSet("MainFlame").incrementIndex(1.0f * speedAdjust);
            //physicalObject.getTextureSet("InternalFlame").incrementIndex(1.0f * speedAdjust);

            if (isTempNode) // TODO make use that awesome get set thing of c#'s so don't have to set this each step
                physicalObject.colorValue = new Color(Color.White, 0.25f);
            else
                physicalObject.colorValue = new Color(Color.White, 1.0f);


            if(this.activationCountdown/100>1)
                physicalObject.getTextureSet("Countdown").currentTextureListIndex=this.activationCountdown/100+1;
            else if (!rocketsFireing)
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = 0;
            else
            {
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = 1;
            }

            base.Update(gameTime, speedAdjust);
            
        }

    }
}
