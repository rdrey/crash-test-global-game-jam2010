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


namespace PhysicsGame
{


    /// <summary>
    /// This is the main type for your game
    /// </summary>
    /// 
    public class TextureStore
    {
        List<Texture2D> rocketTextures;


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


        enum GameState {Init, MainMenu, InitGame, BuildPhase, SimPhase, Pause };

        GameState currentGameState = GameState.InitGame; // TODO make Init


        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            graphics.PreferredBackBufferWidth = 1024;
            graphics.PreferredBackBufferHeight = 768;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
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

        Texture2D makeTexture(Color col)
        {
            Color[] dataz = new Color[1];
            dataz[0] = col;
            Texture2D result = new Texture2D(graphics.GraphicsDevice, 1, 1, 1, TextureUsage.None, SurfaceFormat.Color);
            result.SetData(dataz);
            return result;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);


            spriteFont = Content.Load<SpriteFont>("DefaultFont");
            
            backgroundTexture = Content.Load<Texture2D>("Sprites\\background");

            buildingTexture[0] = Content.Load<Texture2D>("Sprites\\building1");
            buildingTexture[1] = Content.Load<Texture2D>("Sprites\\building2");
            buildingTexture[2] = Content.Load<Texture2D>("Sprites\\building3");
            buildingTexture[3] = Content.Load<Texture2D>("Sprites\\building4");

            sounds = new ModSound();

            sounds.addSound("sound", Content.Load<SoundEffect>("Sounds/testsound2"));
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        private void runInitState()
        {

            player1 = new CubeSet(physicsController, buildingTexture[0], new Vector2(300, 300));

            player1.addCubeNodeAt(CubeSet.adjacentIndex(new Vector2(0, 0), Direction.North), player1.createNode(buildingTexture[0]));
            player1.addCubeNodeAt(CubeSet.adjacentIndex(new Vector2(0, 1), Direction.North), player1.createNode(buildingTexture[0]));
            player1.addCubeNodeAt(CubeSet.adjacentIndex(new Vector2(0, 2), Direction.North), player1.createNode(buildingTexture[0]));
            player1.addCubeNodeAt(CubeSet.adjacentIndex(new Vector2(0, 3), Direction.West), player1.createNode(buildingTexture[0]));


            /*cannon = new PhysicsGameObject(physicsSimulator, 66, 100, false);
            cannon.addTexture(buildingTexture[0]);
            cannon.addTexture(buildingTexture[1]);
            cannon.addTexture(buildingTexture[2]);
            cannon.addTexture(buildingTexture[3]);
            //cannon.setScale(2f);
            cannon.setScaleToFit();

            cannon.boxGeom.FrictionCoefficient = 0.05f;
            cannon.boxBody.Position = new Vector2(300, 300);
            cannon.boxBody.Rotation = -0.9f;*/

            int cubewidth = 600;
            int cubeheight = 600;
            int cubeborder = 10;

            floors[0] = new PhysicsGameObject(physicsController.physicsSimulator, cubewidth, cubeborder, true);
            floors[0].addTexture(backgroundTexture);
            floors[0].boxBody.Position = new Vector2(floors[0].boxBody.Position.X + cubewidth / 2 + cubeborder, cubeborder/2);

            floors[1] = new PhysicsGameObject(physicsController.physicsSimulator, cubewidth, cubeborder, true);
            floors[1].addTexture(backgroundTexture);
            floors[1].boxBody.Position = new Vector2(floors[1].boxBody.Position.X + cubewidth / 2, cubeheight + cubeborder/2);

            floors[2] = new PhysicsGameObject(physicsController.physicsSimulator, cubeborder, cubeheight, true);
            floors[2].addTexture(backgroundTexture);
            floors[2].boxBody.Position = new Vector2(cubeborder/2, floors[2].boxBody.Position.Y + cubeheight / 2);

            floors[3] = new PhysicsGameObject(physicsController.physicsSimulator, cubeborder, cubeheight, true);
            floors[3].addTexture(backgroundTexture);
            floors[3].boxBody.Position = new Vector2(cubewidth + cubeborder / 2, floors[3].boxBody.Position.Y + cubeheight / 2 + cubeborder);
            
            currentGameState = GameState.BuildPhase;
        }

        private void runBuildPhase(GameTime gameTime)
        {

            KeyboardState keyboardState = Keyboard.GetState();


            if (keyboardState.IsKeyDown(Keys.D))
                player1.getRootNode().physicalObject.boxBody.ApplyForce(new Vector2(100, 0));
            if (keyboardState.IsKeyDown(Keys.A))
                player1.getRootNode().physicalObject.boxBody.ApplyForce(new Vector2(-100, 0));
            if (keyboardState.IsKeyDown(Keys.W))
                player1.getRootNode().physicalObject.boxBody.ApplyForce(new Vector2(0, -100));
            if (keyboardState.IsKeyDown(Keys.S))
                player1.getRootNode().physicalObject.boxBody.ApplyForce(new Vector2(0, 100));
            if (keyboardState.IsKeyUp(Keys.P) && previousState.IsKeyDown(Keys.P))
                sounds.playSound("sound", Vector2.Zero);
            if (keyboardState.IsKeyUp(Keys.O) && previousState.IsKeyDown(Keys.O))
                sounds.stopAll();

            if (keyboardState.IsKeyDown(Keys.Space))
            {
                if (player1.isNodeAt(0, 1))
                {
                    CubeNode node = player1.getNodeAt(0, 1);
                    node.hp = 0;
                }

            }

            /*if (keyboardState.IsKeyDown(Keys.A))
                cannon.rotation += 0.1f;

            if (keyboardState.IsKeyDown(Keys.D))
                cannon.rotation -= 0.1f;


            cannon.rotation = MathHelper.Clamp(cannon.rotation, -MathHelper.PiOver2, 0);*/

            // TODO: Add your update logic here

            //cannon.Update();

            player1.Update();

            physicsController.physicsSimulator.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f);

        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
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
            }



            lastGameTime = gameTime;

            previousState = keyboardState;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            //spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);

            spriteBatch.DrawString(spriteFont, "" + lastGameTime.TotalGameTime.Seconds + ":" + (lastGameTime.TotalGameTime - lastGameTime.ElapsedGameTime).Seconds, new Vector2(10, 10), Color.White);

            //cannon.draw(spriteBatch);
            floors[0].draw(spriteBatch);
            floors[1].draw(spriteBatch);
            floors[2].draw(spriteBatch);
            floors[3].draw(spriteBatch);

            foreach(PhysicsGameObject phy in physicsController.physicsObjects) {
                phy.draw(spriteBatch);

            }

            /*foreach (Vector2 vertex in cannon.boxGeom.WorldVertices)
            {
                spriteBatch.DrawString(spriteFont, "" + vertex.X + ":" + vertex.Y, vertex, Color.White);
            }*/

            spriteBatch.End();

            

            basicEffect.Begin();

            //cannon.draw(basicEffect, GraphicsDevice);
            //floor.draw(basicEffect, GraphicsDevice);

            basicEffect.End();

            //GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, pointList, 0, points);

           

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }



    }


}
