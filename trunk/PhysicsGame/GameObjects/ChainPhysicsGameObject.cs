using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;

using FarseerGames.FarseerPhysics;
using FarseerGames.FarseerPhysics.Dynamics;
using FarseerGames.FarseerPhysics.Collisions;
using FarseerGames.FarseerPhysics.Factories;
using FarseerGames.FarseerPhysics.Dynamics.Joints;


namespace PhysicsGame.GameObjects
{
    class ChainPhysicsGameObject : PhysicsGameObject
    {
        // Create path and control points
        int min = 1;
        int max = 3;
        int springConstant = 200;
        int dampingConstant = 4;
        public Path path;
        PhysicsSimulator physicsSimulator;

        public ChainPhysicsGameObject(PhysicsSimulator physicsSimulator) :
            base(physicsSimulator, 1,1, false)            
        {
            this.physicsSimulator = physicsSimulator;
            path = new Path(30, 30, .2f, false);
            this.boxBody.Mass = .25f;

        }

        public void makeLink()
        {
            path.LinkBodies(LinkType.LinearSpring, min, max, springConstant, dampingConstant);
            path.LinkBodies(LinkType.SliderJoint, min, max, springConstant, dampingConstant);
            path.AddToPhysicsSimulator(physicsSimulator);
            //path.CreateGeoms(CollisionCategories, CollidesWith, simulator);
        }

        //public void setOldPos()
    }
}
