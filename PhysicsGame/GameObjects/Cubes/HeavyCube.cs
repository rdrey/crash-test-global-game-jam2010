using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class HeavyCube : CubeNode
    {

        public HeavyCube()
        {
            maxHp = 200;
            defaultAnimationSpeed = 0.3f;
            
        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {
            physicalObject.boxBody.Mass = 5;
            base.Update(gameTime, speedAdjust);
        }
    }
}
