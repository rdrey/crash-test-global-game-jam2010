using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class PlainCube : CubeNode
    {

        public PlainCube()
        {
            maxHp = 100;
            defaultAnimationSpeed = 0f;

            cost = 1;
        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {
            base.Update(gameTime, speedAdjust);
        }

    }
}
