using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace PhysicsGame.GameObjects.Cubes
{
    class ShieldCube : CubeNode
    {

        public float totalShieldBattery = 750;
        public float shieldBattery = 750;

        public ShieldCube()
        {
            maxHp = 200;
            defaultAnimationSpeed = .5f;

            cost = 6;
        }

        private bool supplement(CubeNode node)
        {
            float toAdd = node.maxHp - node.hp;
            if (toAdd > 50) toAdd = 50;

            node.hp += toAdd;
            shieldBattery -= toAdd;

            return toAdd > 0.1;
        }

        public override void Update(GameTime gameTime, float speedAdjust)
        {
            physicalObject.removeTextureSet("Energize");
            if (shieldBattery > 0) {
                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        Vector2 newPos = new Vector2(positionIndex.X+x, positionIndex.Y+y);
                        if (parent.isNodeAt(newPos))
                        {
                            CubeNode node = parent.getNodeAt(newPos);
                            if (supplement(node))
                            {
                                parent.currentRound.soundsToPlay[physicalObject.boxGeom] = new PhysicsGame.Game1.RoundSpecific.SoundInfo("shield", 100f, physicalObject.boxGeom);
                                physicalObject.addTextureSet("Energize");
                            }
                        }
                    }
                }
            }

            physicalObject.getTextureSet("Energize").incrementIndex(1.0f);

            defaultAnimationSpeed = shieldBattery / totalShieldBattery;

            base.Update(gameTime, speedAdjust);
        }
    }
}
