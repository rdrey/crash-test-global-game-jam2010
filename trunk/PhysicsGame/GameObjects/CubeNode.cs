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

    class CubeNode
    {
        CubeSet parent;
        public Vector2 positionIndex;

        //Dictionary<Direction, CubeNode> neighbours; // TODO implement if needed

        public PhysicsGameObject physicalObject;

        int maxHP;
        int hp;

        /* insert other properties here */


        public CubeNode(PhysicsGameObject physicalObject)
        {
            this.physicalObject = physicalObject;
        }

        

    }
}
