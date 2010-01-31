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
        public List<Texture2D> rocketTimer = new List<Texture2D>();
        public List<Texture2D> rocketTextures = new List<Texture2D>();
        public List<Texture2D> rocketFlame = new List<Texture2D>();
        public List<Texture2D> spike = new List<Texture2D>();
        public List<Texture2D> shieldTextures = new List<Texture2D>();
        public List<Texture2D> heavyTextures = new List<Texture2D>();
        public List<Texture2D> shooterTextures = new List<Texture2D>();
        public List<Texture2D> selectTextures = new List<Texture2D>();
        public List<Texture2D> menuTextures = new List<Texture2D>();

        public List<Texture2D> plainTextures = new List<Texture2D>();
        public List<Texture2D> unknownTextures = new List<Texture2D>();
        public List<Texture2D> buzzTextures = new List<Texture2D>();

        public List<Texture2D> bulletTexture = new List<Texture2D>();


        public TextureStore(ContentManager Content)
        {
            loadTextures(Content, rocketTimer, 16, "Sprites/RocketTimerB/RocketTiming");
            loadTextures(Content, rocketTextures, 1, "Sprites/RocketB/Rocket");
            loadTextures(Content, rocketFlame, 17, "Sprites/FlameB/Flame");
            loadTextures(Content, spike, 12, "Sprites/SpikeB/Spike");
            loadTextures(Content, shieldTextures, 23, "Sprites/ShieldB/Shield");
            loadTextures(Content, shooterTextures, 4, "Sprites/ShooterB/Bang_block");
            loadTextures(Content, heavyTextures, 13, "Sprites/HeavyB/Iron");
            loadTextures(Content, menuTextures, 6, "Sprites/Menu/Menu");
            loadTextures(Content, buzzTextures, 10, "Sprites/buzz/Buzz");

            plainTextures.Add(Content.Load<Texture2D>("Sprites/plain_block"));
            unknownTextures.Add(Content.Load<Texture2D>("Sprites/plain_block"));

            bulletTexture.Add(Content.Load<Texture2D>("Sprites/Bullet"));
        }

        //loads textures into desired texture list from given directory
        public void loadTextures(ContentManager Content, List<Texture2D> textureList, int numTextures, string directory)
        {
            for (int i = 0; i < numTextures; i++)
            {
                textureList.Add(Content.Load<Texture2D>(directory + ((i < 9) ? "_0" : "_") + (i + 1)));
            }
        }
    }

    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public class GameSpecific
        {
            public class PlayerInfo
            {

                public List<Instruction> instructionList = new List<Instruction>();
                public Dictionary<Keys, Instruction> keyMap = new Dictionary<Keys, Instruction>();

                public int money = 0;
            }

            public PlayerInfo p1 = new PlayerInfo();
            public PlayerInfo p2 = new PlayerInfo();

            public int completedRounds = 0;

            public GameSpecific()
            {
                setUpKeyboardMap();
            }

            public void setUpKeyboardMap()
            {
                p1.keyMap[Keys.W] = Instruction.Up;
                p1.keyMap[Keys.S] = Instruction.Down;
                p1.keyMap[Keys.A] = Instruction.Left;
                p1.keyMap[Keys.D] = Instruction.Right;


                p1.keyMap[Keys.Q] = Instruction.CycleType;
                p1.keyMap[Keys.E] = Instruction.CycleOption1;
                p1.keyMap[Keys.R] = Instruction.CycleOption2;
                p1.keyMap[Keys.Space] = Instruction.MainAction;
                p1.keyMap[Keys.Tab] = Instruction.EndBuild;


                p2.keyMap[Keys.Up] = Instruction.Up;
                p2.keyMap[Keys.Down] = Instruction.Down;
                p2.keyMap[Keys.Left] = Instruction.Left;
                p2.keyMap[Keys.Right] = Instruction.Right;


                p2.keyMap[Keys.RightShift] = Instruction.CycleType;
                p2.keyMap[Keys.RightControl] = Instruction.CycleOption1;
                p2.keyMap[Keys.RightAlt] = Instruction.CycleOption2;
                p2.keyMap[Keys.Enter] = Instruction.MainAction;
                p2.keyMap[Keys.Insert] = Instruction.EndBuild;

            }
        }

        public class RoundSpecific
        {
            public class RoundPlayerInfo {
                public int instructionsPos = 0;
                public CubeSet cubeSet = null;
            }
            public RoundPlayerInfo p1 = new RoundPlayerInfo();
            public RoundPlayerInfo p2 = new RoundPlayerInfo();

            public float counter = 2000;

            public bool recording = false;

            public PhysicsController physicsController;

            public PhysicsGameObject[] floors = new PhysicsGameObject[4];

            Game1 _parent;

            public RoundSpecific(Game1 parent)
            {
                _parent = parent;
                physicsController = new PhysicsController();

                p1.cubeSet = new CubeSet(physicsController, parent.textureStore, new Vector2(300, 300), 1, parent.sounds);

                p2.cubeSet = new CubeSet(physicsController, parent.textureStore, new Vector2(900, 300), 2, parent.sounds);


                int cubewidth = 1024;
                int cubeheight = 768;
                int cubeborder = 100;

                floors[0] = new PhysicsGameObject(physicsController.physicsSimulator, cubewidth, cubeborder, true);
                floors[0].getTextureSet("Default").addTexture(parent.backgroundTexture);
                floors[0].boxBody.Position = new Vector2(floors[0].boxBody.Position.X + cubewidth / 2, -cubeborder / 2);

                floors[1] = new PhysicsGameObject(physicsController.physicsSimulator, cubewidth, cubeborder, true);
                floors[1].getTextureSet("Default").addTexture(parent.backgroundTexture);
                floors[1].boxBody.Position = new Vector2(floors[1].boxBody.Position.X + cubewidth / 2, cubeheight + cubeborder / 2);

                floors[2] = new PhysicsGameObject(physicsController.physicsSimulator, cubeborder, cubeheight, true);
                floors[2].getTextureSet("Default").addTexture(parent.backgroundTexture);
                floors[2].boxBody.Position = new Vector2(-cubeborder / 2, floors[2].boxBody.Position.Y + cubeheight / 2);

                floors[3] = new PhysicsGameObject(physicsController.physicsSimulator, cubeborder, cubeheight, true);
                floors[3].getTextureSet("Default").addTexture(parent.backgroundTexture);
                floors[3].boxBody.Position = new Vector2(cubewidth + cubeborder / 2, floors[3].boxBody.Position.Y + cubeheight / 2);

                physicsController.registerPhysicsGameObject(floors[0]);
                physicsController.registerPhysicsGameObject(floors[1]);
                physicsController.registerPhysicsGameObject(floors[2]);
                physicsController.registerPhysicsGameObject(floors[3]);


                //Registers collision events
                //physicsController.physicsSimulator.BroadPhaseCollider.OnBroadPhaseCollision += OnBroadPhaseCollision;
                floors[0].boxGeom.OnCollision += OnCollision;
                floors[1].boxGeom.OnCollision += OnCollision;
                floors[2].boxGeom.OnCollision += OnCollision;
                floors[3].boxGeom.OnCollision += OnCollision;

            }

            public bool OnBroadPhaseCollision(Geom geom1, Geom geom2)
            {
                return true;
            }

            //Detects a collision between a specified geom and any other geom. Contains a list of contact points
            public bool OnCollision(Geom geom2, Geom geom1, ContactList list)
            {
                Vector2 position = list[0].Normal;

                float angle = (float)Math.Atan2(position.Y, position.X);

                Vector2 force = Vector2.Zero;
                if (angle < 0)
                    force = new Vector2((float)(Math.Cos(angle) * geom1.Body.LinearVelocity.X), (float)Math.Sin(MathHelper.TwoPi + angle) * geom1.Body.LinearVelocity.Y);
                else
                    force = new Vector2((float)(Math.Cos(angle) * geom1.Body.LinearVelocity.X), (float)Math.Sin(MathHelper.TwoPi - angle) * geom1.Body.LinearVelocity.Y);

                if (force.LengthSquared() > 0.5f)
                {

                    if (physicsController.geomSndLookup[geom1] == 0)
                    {
                        _parent.sounds.playSound("dank", geom1, Vector2.One, force.LengthSquared() / 10000f);
                    }
                    physicsController.geomSndLookup[geom1]++;
                    if (physicsController.geomSndLookup[geom1] > 1000) physicsController.geomSndLookup[geom1] = 0;
                }
                return true;

            }
        

        }

        RoundSpecific currentRound = null;
        GameSpecific currentGame = null;
        public enum Instruction { None, Up, Down, Left, Right, CycleType, CycleOption1, CycleOption2, MainAction, EndBuild };

        // NOTE rest is application specific
        GraphicsDeviceManager graphics;

        SpriteBatch spriteBatch;
        SpriteFont spriteFont;

        int menuCount=0;

        Texture2D backgroundTexture;
        Texture2D hurr;
        Texture2D selector;
        //Texture2D [] buildingTexture = new Texture2D[4];


        BasicEffect basicEffect;

        KeyboardState keyboardState;
        KeyboardState previousState;

        ModSound sounds;

        float lol;// = 0;
        int lolcount = 0;

        PhysicsSimulator applicationPhysicsSimHach = new PhysicsSimulator();

        PhysicsGameObject[] menuObjects = new PhysicsGameObject[4];
        PhysicsGameObject[] pauseObjects = new PhysicsGameObject[2];

        enum GameState { MainMenu, StartGame, StartRound, EndGame, EndRound, BuildPhase, SimPhase, Pause };

        GameState currentApplicationState = GameState.MainMenu; // TODO make Init
        GameState previousApplicationState;

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

            //currentRound.setUpKeyboardMap();



            
             
            //Menu Objects
            for (int i = 0; i < 6; i=i+2)
            {
                menuObjects[i / 2] = new PhysicsGameObject(applicationPhysicsSimHach, 75, 75, false);
                menuObjects[i / 2].getTextureSet("Default").addTexture(textureStore.menuTextures[i]);
                menuObjects[i / 2].getTextureSet("Default").addTexture(textureStore.menuTextures[i + 1]);
                menuObjects[i / 2].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[0].getWidth() / 2), (i / 2) * 200 + 200);
                applicationPhysicsSimHach.Remove(menuObjects[i / 2].boxGeom);
            }
            

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
            hurr = Content.Load<Texture2D>("Sprites/Purple_block");
            //Change
            selector = Content.Load<Texture2D>("Sprites/SpikeB/spike_01");



            sounds = new ModSound();

            sounds.addSound("sound", Content.Load<SoundEffect>("Sounds/testsound2"));
            sounds.addSound("dank", Content.Load<SoundEffect>("Sounds/Dank2"));
            sounds.addSound("bang", Content.Load<SoundEffect>("Sounds/bang"));
        }

        protected override void UnloadContent()
        {
            //Apply content directly to face
        }

        //Main Menu state. Original game state and only reachable through pause menu.
        public void runMainMenu(GameTime gameTime)
        {

            bool menuChange = false;
            if ((keyboardState.IsKeyDown(Keys.W) && previousState.IsKeyUp(Keys.W)) || (keyboardState.IsKeyDown(Keys.Up) && previousState.IsKeyUp(Keys.Up)))
            {
                menuCount = ((menuCount - 1) + 3) % 3;
                menuChange = true;
            }
            if (keyboardState.IsKeyDown(Keys.S) && previousState.IsKeyUp(Keys.S) || (keyboardState.IsKeyDown(Keys.Down) && previousState.IsKeyUp(Keys.Down)))
            {
                menuCount = ((menuCount + 1) + 3) % 3;
                menuChange = true;
            }

            if (menuChange == true || (menuCount == 0 && menuObjects[0].getTextureSet("Default").currentTextureListIndex != 1))
            {
                for (int i = 0; i < 3; i++)
                {
                    if(i==menuCount)
                        menuObjects[i].getTextureSet("Default").currentTextureListIndex = 1;
                    else
                        menuObjects[i].getTextureSet("Default").currentTextureListIndex = 0;

                }
            }

            //Menu Select
            if (keyboardState.IsKeyDown(Keys.Enter) && previousState.IsKeyUp(Keys.Enter))
            {
                switch (menuCount)
                {
                    //Menu case 1
                    case 0:
                        currentApplicationState = GameState.StartGame;
                        break;
                    //Menu case 2
                    case 1:
                        Exit();
                        break;
                    //Menu case 3
                        //I can quit
                    case 2:
                        Exit();
                        break;


                }
            }
        }

        public void runPauseState(GameTime gameTime)
        {
            bool pauseChange = false;
            if ((keyboardState.IsKeyDown(Keys.W) && previousState.IsKeyUp(Keys.W)) || (keyboardState.IsKeyDown(Keys.Up) && previousState.IsKeyUp(Keys.Up)))
            {
                menuCount = ((menuCount - 1) + 2) % 2;
                pauseChange = true;
            }
            if (keyboardState.IsKeyDown(Keys.S) && previousState.IsKeyUp(Keys.S) || (keyboardState.IsKeyDown(Keys.Down) && previousState.IsKeyUp(Keys.Down)))
            {
                menuCount = ((menuCount + 1) + 2) % 2;
                pauseChange = true;
            }

            if (pauseChange == true)
            {
                if (menuCount == 0)
                {
                    menuObjects[0].getTextureSet("Default").currentTextureListIndex = 1;
                    menuObjects[2].getTextureSet("Default").currentTextureListIndex = 0;
                }
                else
                {
                    menuObjects[0].getTextureSet("Default").currentTextureListIndex = 0;
                    menuObjects[2].getTextureSet("Default").currentTextureListIndex = 1;
                }
            }

            //Menu Select
            if (keyboardState.IsKeyDown(Keys.Enter) && previousState.IsKeyUp(Keys.Enter))
            {
                switch (menuCount)
                {
                    //Menu case 1: Return to game
                    case 0:
                        currentApplicationState = previousApplicationState;
                        break;
                    //Menu case 2: Return to Main Menu (call cleanup)
                    case 1:
                        currentApplicationState = GameState.MainMenu;
                        menuCount = 0;
                        break;
                }
            }
        }

        public void runStartGame(GameTime gameTime)
        {

            Console.Write("runStartGame\n");
            currentGame = new GameSpecific();

            currentApplicationState = GameState.StartRound;
        }

        public void runStartRound(GameTime gameTime)
        {
            currentGame.p1.money = 20 + currentGame.completedRounds * 10;
            currentGame.p2.money = 20 + currentGame.completedRounds * 10;


            Console.Write("runStartRound " + currentGame.p1.money + ":" + currentGame.p2.money + "\n");
            currentRound = new RoundSpecific(this);


            currentApplicationState = GameState.BuildPhase;
        }

        public void runEndRound(GameTime gameTime)
        {
            Console.Write("runEndRound\n");
            currentRound = null;

            currentGame.completedRounds++;

            if (currentGame.completedRounds == 5)
            {
                currentApplicationState = GameState.EndGame;
                return;
            }

            currentApplicationState = GameState.StartRound;
        }

        public void runEndGame(GameTime gameTime)
        {
            Console.Write("runEndGame\n");
            currentGame = null;

            currentApplicationState = GameState.MainMenu;
        }

        public void runBuildPhase(GameTime gameTime)
        {
            //Console.Write("runBuildPhase\n");

            bool player1PlaybackComplete = false;
            bool player2PlaybackComplete = false;
            for (int i = 0; i < 2; i++)
            {
                RoundSpecific.RoundPlayerInfo rplayer;
                GameSpecific.PlayerInfo player;

                if (i == 0)
                {
                    rplayer = currentRound.p1;
                    player = currentGame.p1;
                }
                else
                {
                    rplayer = currentRound.p2;
                    player = currentGame.p2;
                }

                if (rplayer.cubeSet.finishedEditing)
                    continue;

                Instruction inst = Instruction.None;
                if (currentRound.recording)
                {
                    foreach (Keys k in player.keyMap.Keys)
                    {
                        if (keyboardState.IsKeyDown(k) && previousState.IsKeyUp(k))
                        {
                            inst = player.keyMap[k];
                            break;
                        }

                    }
                }
                else // playback
                {
                    Console.Write("" + player.instructionList + " - " + rplayer.instructionsPos + "\n");
                    if (rplayer.instructionsPos < player.instructionList.Count())
                    {
                        inst = player.instructionList[rplayer.instructionsPos];
                        rplayer.instructionsPos++;
                    }
                    else
                    {
                        if (i == 0) player1PlaybackComplete = true;
                        else player2PlaybackComplete = true;
                    }
                }
                bool failedInstruction = false ;

                if (inst == Instruction.None)
                    failedInstruction = true;
                else if (inst == Instruction.Right)
                    rplayer.cubeSet.changeSelectedNode(Direction.East);
                else if (inst == Instruction.Left)
                    rplayer.cubeSet.changeSelectedNode(Direction.West);
                else if (inst == Instruction.Up)
                    rplayer.cubeSet.changeSelectedNode(Direction.North);
                else if (inst == Instruction.Down)
                    rplayer.cubeSet.changeSelectedNode(Direction.South);
                else if (inst == Instruction.CycleType)
                    rplayer.cubeSet.cycleSelectedNode();
                else if (inst == Instruction.CycleOption1)
                    rplayer.cubeSet.cycleOption1();
                else if (inst == Instruction.CycleOption2)
                    rplayer.cubeSet.cycleOption2();
                else if (inst == Instruction.MainAction)
                {
                    if (player.money >= rplayer.cubeSet.getSelectedNode().cost)
                    {
                        if (rplayer.cubeSet.makeCurrentSelectionPermanent())
                            player.money -= rplayer.cubeSet.getSelectedNode().cost;
                        else
                            failedInstruction = true;
                    }
                    else
                        failedInstruction = true;


                    Console.Write(player.money + "\n");
                }
                else if (inst == Instruction.EndBuild)
                {
                    rplayer.cubeSet.finishedEditing = true;
                    rplayer.cubeSet.deselectAll();
                    failedInstruction = true;
                }


                if (currentRound.recording)
                {
                    if (!failedInstruction) {
                        player.instructionList.Add(inst);
                    }
                }
            }

            if (player1PlaybackComplete && player2PlaybackComplete)
            {
                currentRound.recording = true;
            }

            if (currentRound.p1.cubeSet.finishedEditing && currentRound.p2.cubeSet.finishedEditing)
            {
                currentRound.p1.cubeSet.startActivationCountdowns();
                currentRound.p2.cubeSet.startActivationCountdowns();
                currentApplicationState = GameState.SimPhase;
            }
            float speedAdjust = 1.0f;
            if (keyboardState.IsKeyDown(Keys.P))
                speedAdjust = 0.2f;


            currentRound.p1.cubeSet.Update(gameTime, speedAdjust);
            currentRound.p2.cubeSet.Update(gameTime, speedAdjust);

            currentRound.physicsController.physicsSimulator.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f * speedAdjust);

        }

        public void runSimPhase(GameTime gameTime)
        {
            //Console.Write("runSimPhase\n");

            float speedAdjust = 1.0f;
            sounds.Pitch = 0f;
            if (keyboardState.IsKeyDown(Keys.P))
            {
                speedAdjust = 0.2f;
                sounds.Pitch = -0.5f;
            }

            currentRound.p1.cubeSet.Update(gameTime, speedAdjust);
            currentRound.p2.cubeSet.Update(gameTime, speedAdjust);

            currentRound.physicsController.physicsSimulator.Update(gameTime.ElapsedGameTime.Milliseconds * 0.001f * speedAdjust);

            currentRound.counter -= 1.0f * speedAdjust;

            if (currentRound.counter < 0)
            {
                currentRound.recording = false;

                //reset everything

                currentApplicationState = GameState.EndRound;
            }
        }


        //Detects any collision between two geoms but doesn't have a list of contact points


        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            if ((keyboardState.IsKeyDown(Keys.Escape) && previousState.IsKeyUp(Keys.Escape))&&currentApplicationState!=GameState.MainMenu)
            {
                menuCount = 0;
                if (currentApplicationState == GameState.Pause)
                    currentApplicationState = previousApplicationState;
                else if(currentApplicationState!=GameState.MainMenu)
                {
                    previousApplicationState = currentApplicationState;
                    currentApplicationState = GameState.Pause;
                }
            }



            switch (currentApplicationState)
            {
                case GameState.MainMenu:
                    runMainMenu(gameTime);
                    break;
                case GameState.StartGame:
                    runStartGame(gameTime);
                    break;
                case GameState.StartRound:
                    runStartRound(gameTime);
                    break;
                case GameState.EndGame:
                    runEndGame(gameTime);
                    break;
                case GameState.EndRound:
                    runEndRound(gameTime);
                    break;
                case GameState.BuildPhase:
                    runBuildPhase(gameTime);
                    break;
                case GameState.SimPhase:
                    runSimPhase(gameTime);
                    break;
                case GameState.Pause:
                    runPauseState(gameTime);
                    break;

            }
            //Registers collision events
            //physicsController.physicsSimulator.BroadPhaseCollider.OnBroadPhaseCollision += OnBroadPhaseCollision;
            //floors[0].boxGeom.OnCollision += OnCollision;
            //floors[1].boxGeom.OnCollision += OnCollision;
            //floors[2].boxGeom.OnCollision += OnCollision;
            //floors[3].boxGeom.OnCollision += OnCollision;



            previousState = keyboardState;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);
            spriteBatch.Draw(backgroundTexture, new Vector2(0, 0), Color.White);

            //spriteBatch.DrawString(spriteFont, "" + lastGameTime.TotalGameTime.Seconds + ":" + (lastGameTime.TotalGameTime - lastGameTime.ElapsedGameTime).Seconds, new Vector2(10, 10), Color.White);
            //spriteBatch.DrawString(spriteFont, "" + player1.selectedCube.Value.X + ":" + player1.selectedCube.Value.Y, new Vector2(10, 30), Color.White);
            //spriteBatch.DrawString(spriteFont, "" + lol, new Vector2(10, 50), Color.White);
            
            //cannon.draw(spriteBatch);
            if (currentRound != null)
            {
                currentRound.floors[0].draw(spriteBatch);
                currentRound.floors[1].draw(spriteBatch);
                currentRound.floors[2].draw(spriteBatch);
                currentRound.floors[3].draw(spriteBatch);

                foreach (PhysicsGameObject phy in currentRound.physicsController.physicsObjects)
                {
                    phy.draw(spriteBatch);
                }
            }


            //If the main menu is active, draw the menuObjects
            if (currentApplicationState == GameState.MainMenu)
            {
                menuObjects[0].draw(spriteBatch);
                menuObjects[1].draw(spriteBatch);
                menuObjects[2].draw(spriteBatch);
                //menuObjects[3].draw(spriteBatch);
            }
            else if (currentApplicationState == GameState.Pause)
            {
                menuObjects[0].draw(spriteBatch);
                menuObjects[2].draw(spriteBatch);
            }

            //spriteBatch.Draw(backgroundTexture, new Vector2(0,0), null, Color.White, 0, new Vector2(0,0), 1.0f, SpriteEffects.None, -0.5f);

            /*foreach (Vector2 vertex in cannon.boxGeom.WorldVertices)
            {
                spriteBatch.DrawString(spriteFont, "" + vertex.X + ":" + vertex.Y, vertex, Color.White);
            }*/

            spriteBatch.End();

            

            basicEffect.Begin();

            if (currentRound != null)
            {
                foreach (PhysicsGameJoint phy in currentRound.physicsController.physicsJoints)
                {
                    phy.draw(basicEffect, GraphicsDevice);
                }
            }

            //cannon.draw(basicEffect, GraphicsDevice);
            //floor.draw(basicEffect, GraphicsDevice);

            basicEffect.End();

            //GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, pointList, 0, points);

            base.Draw(gameTime);
        }
    }
}
