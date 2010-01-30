﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class ChainCube : CubeNode
    {

        public ChainCube()
        {
            maxHp = 1000;
            defaultAnimationSpeed = 0f;
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            base.Update(gameTime, speedAdjust);
        }
    }
}