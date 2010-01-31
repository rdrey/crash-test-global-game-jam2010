using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class BulletCube : CubeNode
    {

        public BulletCube()
        {
            maxHp = 100;
            defaultAnimationSpeed = 0f;

        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {
            base.Update(gameTime, speedAdjust);
        }

    }
}
