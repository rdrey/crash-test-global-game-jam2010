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
        
        public void Update() {

            if (hp == 0)
                markForDelete = true;


            // cycle through images
            foreach (string name in physicalObject.textureNames) {
                physicalObject.getTextureSet(name).incrementIndex(1);
            }
        }
        

    }
}
