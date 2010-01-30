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

        public RocketCube()
        {
            maxHp = 100;
            defaultAnimationSpeed = .5f;

        }

        public override void activate()
        {
            rocketsFireing = true;

            // enable fire overlay
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            if (rocketsFireing)
                physicalObject.boxBody.ApplyForce(new Vector2(-100,0)); // fire ze missiles! (but I am le tired)


            base.Update(gameTime, speedAdjust);
        }
    }
}
