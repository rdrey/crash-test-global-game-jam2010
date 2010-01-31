using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicsGame.GameObjects.Cubes
{
    class DamageCube : CubeNode
    {
        public DamageCube()
        {
            maxHp = 500;
            defaultAnimationSpeed = 0.3f;
            damageMultiplier *= 2.5f;
            cost = 4;
        }
    }
}
