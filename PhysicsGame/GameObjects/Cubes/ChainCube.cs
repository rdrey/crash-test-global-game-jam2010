using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class ChainCube : CubeNode
    {

        public ChainCube()
        {
            maxHp = 1200;
            defaultAnimationSpeed = 0f;
            damageMultiplier = 0f;
            cost = 1;
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            base.Update(gameTime, speedAdjust);
        }
    }
}