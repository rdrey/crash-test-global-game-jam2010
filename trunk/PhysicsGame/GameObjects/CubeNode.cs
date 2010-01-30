using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace PhysicsGame.GameObjects
{
    //TODO put in separate class
    public enum Direction { North, South, West, East };

    //==============================================================
    //==============================================================
    //==============================================================

    class CubeNode : GameObject
    {
        
        CubeSet parentsd;

        public bool isTempNode = false;
        public Vector2 positionIndex;

        //Dictionary<Direction, CubeNode> neighbours; // TODO implement if needed

        public PhysicsGameObject physicalObject;

        public int maxHp;
        public int hp;

        public bool markForDelete = false;

        /* insert other properties here */


        public CubeNode(PhysicsGameObject physicalObject)
        {
            this.physicalObject = physicalObject;
            maxHp = 100;
            hp = maxHp;
        }

        public void select()
        {
            physicalObject.addTextureSet("Selected");

        }
        public void deselect()
        {
            physicalObject.removeTextureSet("Selected");
            physicalObject.getTextureSet("Selected").currentTextureListIndex = 0;
        }

        public void Update() {

            if (hp == 0)
                markForDelete = true;

            if (isTempNode) // TODO make use that awesome get set thing of c#'s so don't have to set this each step
                physicalObject.colorValue = new Color(Color.White, 0.25f);
            else
                physicalObject.colorValue = new Color(Color.White, 1.0f);


            // cycle through images
            foreach (string name in physicalObject.textureNames) {
                physicalObject.getTextureSet(name).incrementIndex(1);
            }
        }
        

    }
}
