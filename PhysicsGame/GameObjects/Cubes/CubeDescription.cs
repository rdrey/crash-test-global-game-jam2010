using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicsGame.GameObjects.Cubes
{
    class CubeDescription
    {
        public CubeType type;

        public Direction dir;

        public CubeDescription()
        {
            this.type = CubeType.UnknownCube;
            dir = Direction.West;
        }

        public CubeDescription(CubeType type)
        {
            this.type = type;
            dir = Direction.West;
        }

        public CubeDescription(CubeType type, Direction dir)
        {
            this.type = type;
            this.dir = dir;
        }
    }
}
