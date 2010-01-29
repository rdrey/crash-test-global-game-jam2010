using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects
{
    public class IntVector2
    {
        int X, Y;

        public IntVector2()
        {
            this.X = 0;
            this.Y = 0;
        }

        public IntVector2(int X, int Y)
        {
            this.X = X;
            this.Y = Y;

        }



        public IntVector2 adjacentIndex(Direction dir)
        {

            if (dir == Direction.North)
                return new IntVector2(X, Y + 1);
            else if (dir == Direction.South)
                return new IntVector2(X, Y - 1);
            else if (dir == Direction.East)
                return new IntVector2(X + 1, Y);
            else// if (dir == Direction.West)
                return new IntVector2(X - 1, Y);
        }

        public Vector2 getRealPosition(Vector2 cubeSize)
        {
            return new Vector2(X*cubeSize.X, Y*cubeSize.Y);
        }
    }
}
