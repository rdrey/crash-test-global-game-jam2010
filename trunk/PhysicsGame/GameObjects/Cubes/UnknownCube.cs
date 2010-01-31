using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class UnknownCube : CubeNode
    {

        public UnknownCube()
        {
            maxHp = 200;
            defaultAnimationSpeed = .0f;

            cost = 0;
        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {
            base.Update(gameTime, speedAdjust);
        }
    }
}
