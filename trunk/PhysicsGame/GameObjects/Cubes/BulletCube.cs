using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class BulletCube : CubeNode
    {
        public int timeToKill = 500;
        public int deadTime = 0;
        public BulletCube()
        {
            maxHp = 1;
            defaultAnimationSpeed = 0f;
            damageMultiplier = 50f;
        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {
            timeToKill --;

            if (timeToKill < 0)
            {
                markForDelete = true;
                damageMultiplier = 0f;
            }

            physicalObject.boxGeom.CollisionResponseEnabled = false;
            base.Update(gameTime, speedAdjust);
        }

    }
}
