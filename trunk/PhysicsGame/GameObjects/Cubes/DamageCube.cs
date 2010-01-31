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
            maxHp = 250;
            defaultAnimationSpeed = 0.3f;
            damageMultiplier *= 2;
            cost = 4;
        }
    }
}
