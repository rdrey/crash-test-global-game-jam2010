using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
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
using PhysicsGame.GameObjects;
using PhysicsGame.GameObjects.Cubes;


//game
namespace PhysicsGame
{
    public class TextureStore
    {
        public List<Texture2D> rocketTextures = new List<Texture2D>();
        public List<Texture2D> rocketFlame = new List<Texture2D>();
        public List<Texture2D> shieldTextures = new List<Texture2D>();
        public List<Texture2D> heavyTextures = new List<Texture2D>();
        public List<Texture2D> selectTextures = new List<Texture2D>();

        public List<Texture2D> plainTextures = new List<Texture2D>();
        public List<Texture2D> unknownTextures = new List<Texture2D>();

        public TextureStore()
        {
        }

        public TextureStore(ContentManager Content)
        {
            loadTextures(Content, rocketTextures, 9, "Sprites/RocketB/Rocket");
            loadTextures(Content, rocketFlame, 17, "Sprites/FlameB/Flame");
            loadTextures(Content, shieldTextures, 23, "Sprites/ShieldB/Shield");
            loadTextures(Content, heavyTextures, 13, "Sprites/HeavyB/Iron");

            plainTextures.Add(Content.Load<Texture2D>("Sprites/plain_block"));
            unknownTextures.Add(Content.Load<Texture2D>("Sprites/plain_block"));
        }

        //loads textures into desired texture list from given directory
        public void loadTextures(ContentManager Content, List<Texture2D> textureList, int numTextures, string directory)
        {
            for (int i = 0; i < numTextures; i++)
            {
                textureList.Add(Content.Load<Texture2D>(directory + "_" + ((i < 9) ? "0" : "") + (i + 1)));
            }
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        PhysicsGameObject [] floors = new PhysicsGameObject [4];

        Texture2D backgroundTexture;
        Texture2D [] buildingTexture = new Texture2D[4];

        PhysicsController physicsController;

        BasicEffect basicEffect;

        GameTime lastGameTime;
        KeyboardState previousState;

        CubeSet player1;
        ModSound sounds;

        float lol;// = 0;
        int lolcount = 0;

        enum GameState {Init, MainMenu, InitGame, BuildPhase, SimPhase, Pause };

        GameState currentGameState = GameState.InitGame; // TODO make Init

        TextureStore textureStore;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            base.Initialize();

            IsFixedTimeStep = true;
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 10);


            Matrix viewMatrix;
            Matrix projectionMatrix;

            viewMatrix = Matrix.CreateLookAt(
                new Vector3(0.0f, 0.0f, 1.0f),
                Vector3.Zero,
                Vector3.Up
                );

            projectionMatrix = Matrix.CreateOrthographicOffCenter(
                0,
                (float)GraphicsDevice.Viewport.Width,
                (float)GraphicsDevice.Viewport.Height,
                0,
                1.0f, 1000.0f);


            basicEffect = new BasicEffect(GraphicsDevice, null);
            basicEffect.VertexColorEnabled = true;

            basicEffect.View = viewMatrix;
            basicEffect.Projection = projectionMatrix;

            GraphicsDevice.RenderState.PointSize = 10;
            physicsController = new PhysicsController();


        }

        private Texture2D makeTexture(Color col)
        {
            Color[] dataz = new Color[1];
            dataz[0] = col;
            Texture2D result = new Texture2D(graphics.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            result.SetData(dataz);
            return result;
        }

        protected override void LoadContent()
        {
            textureStore = new TextureStore(Content);

            for (int i = 0; i < 100; i++)
            {
                textureStore.selectTextures.Add(makeTexture(new Color(1.0f, 0.0f, 0.0f, 0.75f - (0.005f * i))));
            }
            for (int i = 0; i < 100; i++)
            {
                textureStore.selectTextures.Add(makeTexture(new Color(1.0f, 0.0f, 0.0f, 0.25f+0.005f * i)));
            }
            

            spriteBatch = new SpriteBatch(GraphicsDevice);


            spriteFont = Content.Load<SpriteFont>("DefaultFont");
            
            backgroundTexture = Content.Load<Texture2D>("Sprites/bg");

            sounds = new ModSound();

            sounds.addSound("sound", Content.Load<SoundEffect>("Sounds/testsound2"));
            sounds.addSound("bang", Content.Load<SoundEffect>("Sounds/bang"));
        }

        protected override void UnloadContent()
        {
            //Apply content directly to face
        }

        private void runInitState()
        {

            player1 = new CubeSet(physicsController, textureStore, new Vector2(300, 300));

            player1.addCubeNodeFrom(new Vector2(0, 0), Direction.North, new CubeDescription(CubeType.RocketCube));
            player1.addCubeNodeFrom(new Vector2(0, -1), Direction.North, new CubeDescription(CubeType.ShieldCube));
            player1.addCubeNodeFrom(new Vector2(0, -2), Direction.North, new CubeDescription(CubeType.HeavyCube));
            player1.addCubeNodeFrom(new Vector2(0, -3), Direction.West, new CubeDescription(CubeType.UnknownCube));


            int cubewidth = 1000;
            int cubeheight = 700;
            int cubeborder = 10;

            floors[0] = new PhysicsGameObject(physicsController.physicsSimulator, cubewidth, cubeborder, true);
            floors[0].getTextureSet("Default").addTexture(backgroundTexture);
            floors[0].boxBody.Position = new Vector2(floors[0].boxBody.Position.X + cubewidth / 2 + cubeborder, cubeborder/2);

            floors[1] = new PhysicsGameObject(physicsController.physicsSimulator, cubewidth, cubeborder, true);
            floors[1].getTextureSet("Default").addTexture(backgroundTexture);
            floors[1].boxBody.Position = new Vector2(floors[1].boxBody.Position.X + cubewidth / 2, cubeheight + cubeborder/2);

            floors[2] = new PhysicsGameObject(physicsController.physicsSimulator, cubeborder, cubeheight, true);
            floors[2].getTextureSet("Default").addTexture(backgroundTexture);
            floors[2].boxBody.Position = new Vector2(cubeborder/2, floors[2].boxBody.Position.Y + cubeheight / 2);

            floors[3] = new PhysicsGameObject(physicsController.physicsSimulator, cubeborder, cubeheight, true);
            floors[3].getTextureSet("Default").addTexture(backgroundTexture);
            floors[3].boxBody.Position = new Vector2(cubewidth + cubeborder / 2, floors[3].boxBody.Position.Y + cubeheight / 2 + cubeborder);

            physicsController.registerPhysicsGameObject(floors[0]);
            physicsController.registerPhysicsGameObject(floors[1]);
            physicsController.registerPhysicsGameObject(floors[2]);
            physicsController.registerPhysicsGameObject(floors[3]);

            currentGameState = GameState.BuildPhase;
        }

        private void runBuildPhase(GameTime gameTime)
        {

            KeyboardState keyboardState = Keyboard.GetState();


            if (keyboardState.IsKeyDown(Keys.A) && previousState.IsKeyUp(Keys.A))
                player1.changeSelectedNode(Direction.West);
            if (keyboardState.IsKeyDown(Keys.D) && previousState.IsKeyUp(Keys.D))
                player1.changeSelectedNode(Direction.East);
            if (keyboardState.IsKeyDown(Keys.W) && previousState.IsKeyUp(Keys.W))
                player1.changeSelectedNode(Direction.North);
            if (keyboardState.IsKeyDown(Keys.S) && previousState.IsKeyUp(Keys.S))
                player1.changeSelectedNode(Direction.South);

            if (keyboardState.IsKeyDown(Keys.Left))
                player1.getSelectedNode().physicalObject.boxBody.ApplyForce(new Vector2(-100, 0));
            if (keyboardState.IsKeyDown(Keys.Right))
                player1.getSelectedNode().physicalObject.boxBody.ApplyForce(new Vector2(100, 0));
            if (keyboardState.IsKeyDown(Keys.Up))
                player1.getSelectedNode().physicalObject.boxBody.ApplyForce(new Vector2(0, -100));
            if (keyboardState.IsKeyDown(Keys.Down))
                player1.getSelectedNode().physicalObject.boxBody.ApplyForce(new Vector2(0, 100));

            //if (keyboardState.IsKeyUp(Keys.P) && previousState.IsKeyDown(Keys.P))
            //    sounds.playSound("sound", Vector2.Zero);
            //if (keyboardState.IsKeyUp(Keys.O) && previousState.IsKeyDown(Keys.O))
            //    sounds.stopAll();

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                player1.makeCurrentSelectionPermanent();
            }

            if (keyboardState.IsKeyDown(Keys.Enter))
            {
                player1.deselectAll();
                player1.startActivationCountdowns();
                currentGameState = GameState.SimPhase;
            }


            float speedAdjust = 1.0f;
            if (keyboardState.IsKeyDown(Keys.R))
                speedAdjust = 0.2f;


            player1.Update(gameTime, speedAdjust);

            physicsController.physicsSimulator.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f*speedAdjust);

        }


        private void runSimPhase(GameTime gameTime)
        {

            KeyboardState keyboardState = Keyboard.GetState();
            float speedAdjust = 1.0f;
            if (keyboardState.IsKeyDown(Keys.R))
                speedAdjust = 0.2f;

            player1.Update(gameTime, speedAdjust);

            physicsController.physicsSimulator.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f * speedAdjust);
        }

        //Collision Events

        //BroadPhaseCollision
        //Detects any collision between two geoms but doesn't have a list of contact points
        
        private bool OnBroadPhaseCollision(Geom geom1, Geom geom2)
        {
            return true;
        }

         //Detects a collision between a specified geom and any other geom. Contains a list of contact points
        private bool OnCollision(Geom geom2, Geom geom1, ContactList list)
        {
            Vector2 position = list[0].Normal; //get the normal of the impact

            float angle = (float)Math.Atan2(position.Y, position.X); //calculate the angle of the normal

            Vector2 force = Vector2.Zero; //stores the force of the impact
            if (angle < 0) //detect the angle
                force = new Vector2((float)(Math.Cos(angle) * geom1.Body.LinearVelocity.X),(float)Math.Sin(MathHelper.TwoPi + angle) * geom1.Body.LinearVelocity.Y);
            else
                force = new Vector2((float)(Math.Cos(angle) * geom1.Body.LinearVelocity.X),(float)Math.Sin(MathHelper.TwoPi - angle) * geom1.Body.LinearVelocity.Y);
            //apply the angle to the velocity to work out the impact vel
            //now to get the force of the collison just do force.Length()
            //you now have the impact force on collison
            
            lol = force.Length();
            //Console.WriteLine("{0}", force);

            if (force.LengthSquared() > 0.5f)
            {

                if (physicsController.geomSndLookup[geom1] == 0)
                {
                    sounds.playSound("bang", geom1, Vector2.One, force.LengthSquared()/10000f);
                }
                physicsController.geomSndLookup[geom1]++;
                if (physicsController.geomSndLookup[geom1] > 1000) physicsController.geomSndLookup[geom1] = 0;
            }
            return true;
            
        }
        


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();


            KeyboardState keyboardState = Keyboard.GetState();
            if (previousState == null) previousState = keyboardState;

            switch (currentGameState)
            {
                case GameState.InitGame:
                    runInitState();
                    break;
                case GameState.BuildPhase:
                    runBuildPhase(gameTime);
                    break;
                case GameState.SimPhase:
                    runSimPhase(gameTime);
                    break;
            }
            //Registers collision events
            //physicsController.physicsSimulator.BroadPhaseCollider.OnBroadPhaseCollision += OnBroadPhaseCollision;
            floors[0].boxGeom.OnCollision += OnCollision;
            floors[1].boxGeom.OnCollision += OnCollision;
            floors[2].boxGeom.OnCollision += OnCollision;
            floors[3].boxGeom.OnCollision += OnCollision;

            lastGameTime = gameTime;

            previousState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            //spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);

            spriteBatch.DrawString(spriteFont, "" + lastGameTime.TotalGameTime.Seconds + ":" + (lastGameTime.TotalGameTime - lastGameTime.ElapsedGameTime).Seconds, new Vector2(10, 10), Color.White);
            spriteBatch.DrawString(spriteFont, "" + player1.selectedCube.Value.X + ":" + player1.selectedCube.Value.Y, new Vector2(10, 30), Color.White);
            spriteBatch.DrawString(spriteFont, "" + lol, new Vector2(10, 50), Color.White);
            
            //cannon.draw(spriteBatch);
            floors[0].draw(spriteBatch);
            floors[1].draw(spriteBatch);
            floors[2].draw(spriteBatch);
            floors[3].draw(spriteBatch);

            foreach(PhysicsGameObject phy in physicsController.physicsObjects) {
                phy.draw(spriteBatch);
            }

            //spriteBatch.Draw(backgroundTexture, new Vector2(0,0), null, Color.White, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, -0.5f);

            /*foreach (Vector2 vertex in cannon.boxGeom.WorldVertices)
            {
                spriteBatch.DrawString(spriteFont, "" + vertex.X + ":" + vertex.Y, vertex, Color.White);
            }*/

            spriteBatch.End();

            

            basicEffect.Begin();

            foreach (PhysicsGameJoint phy in physicsController.physicsJoints)
            {
                phy.draw(basicEffect, GraphicsDevice);
            }

            //cannon.draw(basicEffect, GraphicsDevice);
            //floor.draw(basicEffect, GraphicsDevice);

            basicEffect.End();

            //GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, pointList, 0, points);

            base.Draw(gameTime);
        }
    }
}
