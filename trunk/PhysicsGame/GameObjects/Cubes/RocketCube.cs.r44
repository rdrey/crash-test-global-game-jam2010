using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class RocketCube : CubeNode
    {
        bool rocketsFireing = false;
        public float activationCountdown = 0;

        public RocketCube()
        {
            maxHp = 100;
            defaultAnimationSpeed = .5f;

        }

        public void activate()
        {
            rocketsFireing = true;

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


            base.Update(gameTime, speedAdjust);
        }
    }
}
