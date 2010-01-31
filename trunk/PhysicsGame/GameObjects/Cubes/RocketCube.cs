﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Audio;

namespace PhysicsGame.GameObjects.Cubes
{
    class RocketCube : CubeNode
    {
        public static float rocketTimeStep = 40;
        bool rocketsFireing = false;
        public float activationCountdown = 0;

        float timeToDelete = 10.0f;

        public Direction dir;

        public RocketCube(Direction dir)
        {
            this.dir = dir;
            maxHp = 100;
            defaultAnimationSpeed = .0f;

            cost = 2;
        }

        public void activate()
        {
            rocketsFireing = true;
            this.physicalObject.addTextureSet("MainFlame");

            // enable fire overlay
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            if (hp < 0)
            {

            }


            if (startedCountDown)
            {
                if (activationCountdown <= 0)
                    activate();
                else
                    activationCountdown -= 1 * speedAdjust;
            }

            if (rocketsFireing)
            {
                float forceToApply = 10.0f / speedAdjust;
                physicalObject.boxBody.ApplyForce(Vector2.Transform(new Vector2(100,0), Matrix.CreateRotationZ(physicalObject.boxBody.Rotation))); // fire ze missiles! (but I am le tired)
            }


            physicalObject.getTextureSet("MainFlame").incrementIndex(1.0f * speedAdjust);

            if (isTempNode) // TODO make use that awesome get set thing of c#'s so don't have to set this each step
                physicalObject.colorValue = new Color(Color.White, 0.25f);
            else
                physicalObject.colorValue = new Color(Color.White, 1.0f);


            if (this.activationCountdown / 40 >= 1)
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = this.activationCountdown/40+1;
            else if (!rocketsFireing)
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = 0;
            else
            {
                physicalObject.getTextureSet("Countdown").currentTextureListIndex = 1;
            }
            

            base.Update(gameTime, speedAdjust);


            if (hp < 0)
            {
                markForDelete = false;
                timeToDelete -= speedAdjust;
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2 newPos = new Vector2(positionIndex.X + x, positionIndex.Y + y);
                        if (parent.isNodeAt(newPos))
                        {
                            CubeNode node = parent.getNodeAt(newPos);
                            node.hp -= 5;
                        }
                    }
                }
            }

            if (timeToDelete < 0)
                markForDelete = true;
        }

    }
}
