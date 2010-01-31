using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class ShieldCube : CubeNode
    {

        public float shieldBattery = 500;

        public ShieldCube()
        {
            maxHp = 100;
            defaultAnimationSpeed = .5f;

        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {

        }
    }
}
