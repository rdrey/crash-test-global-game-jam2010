using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class BulletCube : CubeNode
    {
        public int deadTime = 0;
        public BulletCube()
        {
            maxHp = 1;
            defaultAnimationSpeed = 0f;
            damageMultiplier *= 1f;
        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {
            base.Update(gameTime, speedAdjust);
        }

    }
}
