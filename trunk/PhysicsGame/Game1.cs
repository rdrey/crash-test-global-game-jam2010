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

        public List<Texture2D> plainTextures = new List<Texture2D>();
        public List<Texture2D> unknownTextures = new List<Texture2D>();

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

            plainTextures.Add(Content.Load<Texture2D>("Sprites/plain_block"));
            unknownTextures.Add(Content.Load<Texture2D>("Sprites/plain_block"));

            bulletTexture.Add(Content.Load<Texture2D>("Sprites/Bullet"));
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
        public class GameSpecific
        {
            public List<Instruction> player1Instructions = new List<Instruction>();
            public List<Instruction> player2Instructions = new List<Instruction>();
            public Dictionary<Keys, Instruction> player1KeyMap = new Dictionary<Keys, Instruction>();
            public Dictionary<Keys, Instruction> player2KeyMap = new Dictionary<Keys, Instruction>();

            public int completedRounds = 0;

            public GameSpecific()
            {
                setUpKeyboardMap();
            }

            public void setUpKeyboardMap()
            {
                player1KeyMap[Keys.W] = Instruction.Up;
                player1KeyMap[Keys.S] = Instruction.Down;
                player1KeyMap[Keys.A] = Instruction.Left;
                player1KeyMap[Keys.D] = Instruction.Right;


                player1KeyMap[Keys.Q] = Instruction.CycleType;
                player1KeyMap[Keys.E] = Instruction.CycleOption1;
                player1KeyMap[Keys.Space] = Instruction.MainAction;
                player1KeyMap[Keys.Tab] = Instruction.EndBuild;


                player2KeyMap[Keys.Up] = Instruction.Up;
                player2KeyMap[Keys.Down] = Instruction.Down;
                player2KeyMap[Keys.Left] = Instruction.Left;
                player2KeyMap[Keys.Right] = Instruction.Right;


                player2KeyMap[Keys.LeftShift] = Instruction.CycleType;
                player2KeyMap[Keys.LeftControl] = Instruction.CycleOption1;
                player2KeyMap[Keys.Enter] = Instruction.MainAction;
                player2KeyMap[Keys.Insert] = Instruction.EndBuild;

            }
        }

        public class RoundSpecific
        {
            public int player1InstructionsPos = 0;
            public int player2InstructionsPos = 0;

            public float counter = 2000;

            public bool recording = false;
            public CubeSet player1, player2;

            public PhysicsController physicsController;

            public PhysicsGameObject[] floors = new PhysicsGameObject[4];

            Game1 _parent;

            public RoundSpecific(Game1 parent)
            {
                _parent = parent;
                physicsController = new PhysicsController();

                player1 = new CubeSet(physicsController, parent.textureStore, new Vector2(300, 300), 1, parent.sounds);

                player2 = new CubeSet(physicsController, parent.textureStore, new Vector2(900, 300), 2, parent.sounds);


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
            menuObjects[0] = new PhysicsGameObject(applicationPhysicsSimHach, 100, 50, false);
            menuObjects[0].getTextureSet("Default").addTexture(backgroundTexture);
            menuObjects[0].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[0].getWidth() / 2), 200);
            applicationPhysicsSimHach.Remove(menuObjects[0].boxGeom);

            menuObjects[1] = new PhysicsGameObject(applicationPhysicsSimHach, 100, 50, false);
            menuObjects[1].getTextureSet("Default").addTexture(backgroundTexture);
            menuObjects[1].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[1].getWidth() / 2), 400);
            applicationPhysicsSimHach.Remove(menuObjects[1].boxGeom);

            menuObjects[2] = new PhysicsGameObject(applicationPhysicsSimHach, 100, 50, false);
            menuObjects[2].getTextureSet("Default").addTexture(backgroundTexture);
            menuObjects[2].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[2].getWidth() / 2), 600);
            applicationPhysicsSimHach.Remove(menuObjects[2].boxGeom);

            menuObjects[3] = new PhysicsGameObject(applicationPhysicsSimHach, 20, 20, false);
            menuObjects[3].getTextureSet("Default").addTexture(selector);
            menuObjects[3].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[0].getWidth() / 2) - 30, 200);
            applicationPhysicsSimHach.Remove(menuObjects[3].boxGeom);
            //
            pauseObjects[0] = new PhysicsGameObject(applicationPhysicsSimHach, 100, 50, false);
            pauseObjects[0].getTextureSet("Default").addTexture(backgroundTexture);
            pauseObjects[0].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (pauseObjects[0].getWidth() / 2), 200);
            applicationPhysicsSimHach.Remove(pauseObjects[0].boxGeom);

            pauseObjects[1] = new PhysicsGameObject(applicationPhysicsSimHach, 100, 50, false);
            pauseObjects[1].getTextureSet("Default").addTexture(backgroundTexture);
            pauseObjects[1].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (pauseObjects[1].getWidth() / 2), 500);
            applicationPhysicsSimHach.Remove(pauseObjects[1].boxGeom);

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
            Console.Write("runMainMenu\n");

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

            if (menuChange == true)
            {
                menuObjects[3].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[0].getWidth() / 2) - 30, menuCount * 200 + 200);
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
                menuObjects[3].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[0].getWidth() / 2) - 30, menuCount * 300 + 200);
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
                        menuObjects[3].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[0].getWidth() / 2) - 30, menuCount * 200 + 200);
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


            Console.Write("runStartRound\n");
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
                CubeSet player;
                Dictionary<Keys, Instruction> keyMap;
                List<Instruction> instructionList;
                int instructionsPos;

                if (i == 0)
                {
                    player = currentRound.player1;
                    keyMap = currentGame.player1KeyMap;
                    instructionList = currentGame.player1Instructions;
                    instructionsPos = currentRound.player1InstructionsPos;
                }
                else
                {
                    player = currentRound.player2;
                    keyMap = currentGame.player2KeyMap;
                    instructionList = currentGame.player2Instructions;
                    instructionsPos = currentRound.player2InstructionsPos; // TODO save these values back afterwards
                }

                Instruction inst = Instruction.None;
                if (currentRound.recording)
                {
                    foreach (Keys k in keyMap.Keys)
                    {
                        if (keyboardState.IsKeyDown(k) && previousState.IsKeyUp(k))
                        {
                            inst = keyMap[k];
                            break;
                        }

                    }
                    if (inst != Instruction.None && inst != Instruction.EndBuild)
                        instructionList.Add(inst);
                }
                else // playback
                {
                    Console.Write("" + instructionList + " - " + instructionsPos + "\n");
                    if (instructionsPos < instructionList.Count())
                    {
                        inst = instructionList[instructionsPos];
                        instructionsPos++;
                        if (i == 0) currentRound.player1InstructionsPos = instructionsPos;
                        else currentRound.player2InstructionsPos = instructionsPos;
                    }
                    else
                    {
                        if (i == 0) player1PlaybackComplete = true;
                        else player2PlaybackComplete = true;
                    }
                }

                if (inst == Instruction.Right)
                    player.changeSelectedNode(Direction.East);
                else if (inst == Instruction.Left)
                    player.changeSelectedNode(Direction.West);
                else if (inst == Instruction.Up)
                    player.changeSelectedNode(Direction.North);
                else if (inst == Instruction.Down)
                    player.changeSelectedNode(Direction.South);
                else if (inst == Instruction.CycleType)
                    player.cycleSelectedNode();
                else if (inst == Instruction.CycleOption1)
                    player.cycleOption1();
                else if (inst == Instruction.MainAction)
                    player.makeCurrentSelectionPermanent();
                else if (inst == Instruction.EndBuild)
                {
                    player.finishedEditing = true;
                    player.deselectAll();
                }
            }

            if (player1PlaybackComplete && player2PlaybackComplete)
            {
                currentRound.recording = true;
            }

            if (currentRound.player1.finishedEditing && currentRound.player2.finishedEditing)
            {
                currentRound.player1.startActivationCountdowns();
                currentRound.player2.startActivationCountdowns();
                currentApplicationState = GameState.SimPhase;
            }
            float speedAdjust = 1.0f;
            if (keyboardState.IsKeyDown(Keys.P))
                speedAdjust = 0.2f;


            currentRound.player1.Update(gameTime, speedAdjust);
            currentRound.player2.Update(gameTime, speedAdjust);

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

            currentRound.player1.Update(gameTime, speedAdjust);
            currentRound.player2.Update(gameTime, speedAdjust);

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
                menuObjects[3].boxBody.Position = new Vector2(graphics.PreferredBackBufferWidth / 2 - (menuObjects[0].getWidth() / 2) - 30, menuCount * 300 + 200);
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
                menuObjects[3].draw(spriteBatch);
            }
            else if (currentApplicationState == GameState.Pause)
            {
                pauseObjects[0].draw(spriteBatch);
                pauseObjects[1].draw(spriteBatch);
                menuObjects[3].draw(spriteBatch);
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
