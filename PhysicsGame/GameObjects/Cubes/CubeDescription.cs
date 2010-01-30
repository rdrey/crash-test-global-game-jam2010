using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicsGame.GameObjects.Cubes
{
    class CubeDescription
    {
        public CubeType type;

        public CubeDescription(CubeType type)
        {
            this.type = type;
        }
    }
}
