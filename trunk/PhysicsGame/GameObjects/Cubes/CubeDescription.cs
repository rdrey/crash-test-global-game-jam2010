using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PhysicsGame.GameObjects.Cubes
{
    public class CubeDescription
    {
        public CubeType type;

        public Direction dir;

        public float value;

        public CubeDescription()
        {
            this.type = CubeType.PlainCube;
            dir = Direction.West;
            value = 400;
        }

        public CubeDescription(CubeType type)
        {
            this.type = type;
            dir = Direction.West;
            value = 400;
        }

        public CubeDescription(CubeType type, Direction dir)
        {
            this.type = type;
            this.dir = dir;
            value = 400;
        }
    }
}
