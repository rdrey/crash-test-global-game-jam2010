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
using FarseerGames.FarseerPhysics.Dynamics.Joints;

namespace PhysicsGame.GameObjects
{
    class PhysicsGameObject : GameObject, Drawable
    {
        // TODO abstract Texture stuff into a contained textureController class

        public enum PhysicsMapID
        {
            anything = 0,
            player1 = 1,
            player2 = 2,
            wall = 3
        }

        public class TextureSet
        {
            public float currentTextureListIndex;
            public List<Texture2D> textureList;

            public Vector2 scale, offset;

            PhysicsGameObject pgo;

            public Nullable<Rectangle> sourceRect, sourceRectAdjustedForScale;

            public TextureSet(PhysicsGameObject pgo)
            {
                this.pgo = pgo;
                currentTextureListIndex = -1;
                textureList = new List<Texture2D>();
                scale = new Vector2(1.0f, 1.0f);
                offset = new Vector2(0.0f, 0.0f);


                sourceRect = new Rectangle(0, 0, (int)pgo.width, (int)pgo.height);
                sourceRectAdjustedForScale = new Rectangle(0, 0, (int)pgo.width, (int)pgo.height);
                calculateSourceRectAdjustedForScale();
            }

            public void addTexture(Texture2D sprite)
            {
                if (currentTextureListIndex == -1)
                    currentTextureListIndex = 0;
                textureList.Add(sprite);

                setScaleToFit();
            }

            public void setScale(float scaleX, float scaleY)
            {
                this.scale.X = scaleX;
                this.scale.Y = scaleY;

                calculateSourceRectAdjustedForScale();
            }

            public void setScale(float scale)
            {
                setScale(scale, scale);
            }

            public void setScaleToFit()
            {
                setScale( pgo.width / textureList[(int)currentTextureListIndex].Width,
                          pgo.height / textureList[(int)currentTextureListIndex].Height);
            }

            public void setScaleByNumberOfPixels(int numberOfPixelsX, int numberOfPixelsY)
            {
                setScale(pgo.width / (float)numberOfPixelsX,
                          pgo.height / (float)numberOfPixelsY);
            }



            private void calculateSourceRectAdjustedForScale()
            {
                if (sourceRect != null)
                {
                    Rectangle adjSrc = new Rectangle();
                    adjSrc.X = sourceRect.Value.X;
                    adjSrc.Y = sourceRect.Value.Y;
                    adjSrc.Width = (int)(sourceRect.Value.Width / scale.X);
                    adjSrc.Height = (int)(sourceRect.Value.Height / scale.Y);
                    sourceRectAdjustedForScale = adjSrc;
                }
            }

            public void setTexturePosition(int x, int y)
            {
                sourceRect = new Rectangle(x, y, (int)pgo.width, (int)pgo.height);
                calculateSourceRectAdjustedForScale();
            }

            public void incrementIndex(float i)
            {
                currentTextureListIndex += i;
                while (currentTextureListIndex >= textureList.Count())
                    currentTextureListIndex -= textureList.Count();
            }

        };

        public List<string> textureNames = new List<string>();
        public Dictionary<string, TextureSet> textures = new Dictionary<string, TextureSet>();

        public List<PhysicsGameJoint> joints = new List<PhysicsGameJoint>();
        public Body boxBody;
        public Geom boxGeom;
        private float width, height;
        public Color colorValue = Color.White;
        public PhysicsMapID ID = PhysicsMapID.anything;

        //private int changeImageCounter, changeImageFrequency;

        public PhysicsGameObject(PhysicsSimulator physicsSimulator, float width, float height, bool isStatic)
        {
            //changeImageCounter = changeImageFrequency = 100;

            addTextureSet("Default");

            this.width = width;
            this.height = height;


            boxBody = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, width, height, 1);
            boxBody.Position = new Vector2(0,0);
            boxBody.IsStatic = isStatic;
            //boxBody.AngularVelocity = 0;

            boxGeom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, boxBody, width, height);
            //boxGeom.
        }


        public override void Update(GameTime gameTime, float speedAdjust)
        {
        }

        public bool addTextureSet(string textureListName)
        {
            if (textureListName == "")
                return false;

            if (!textureNames.Contains(textureListName))
                textureNames.Add(textureListName);
             
            return true;
            
        }
        public bool removeTextureSet(string textureListName)
        {
            if (textureListName == "")
                return false;

            if (textureNames.Contains(textureListName))
                textureNames.Remove(textureListName);

            return true;
        }

        public TextureSet getTextureSet(string textureListName)
        {
            if (!textures.ContainsKey(textureListName))
                textures[textureListName] = new TextureSet(this);

            return textures[textureListName];
        }

        /*public bool addTexture(Texture2D sprite)
        {
            return addTexture(sprite, "Default");
        }*/


        public void draw(SpriteBatch spriteBatch)
        {
            foreach (string name in textureNames)
            {
                spriteBatch.Draw(textures[name].textureList[(int)(textures[name].currentTextureListIndex)], boxGeom.Position, textures[name].sourceRectAdjustedForScale, colorValue, boxGeom.Rotation, new Vector2(((width / 2) + textures[name].offset.X) / textures[name].scale.X, ((height / 2) + textures[name].offset.Y) / textures[name].scale.Y), textures[name].scale, SpriteEffects.None, 0.0f);
            }
        }


        public void draw(BasicEffect basicEffect, GraphicsDevice graphicsDevice)
        {

            Vector2[] pointsList = boxGeom.WorldVertices.ToArray();

            VertexPositionColor[] verticies = new VertexPositionColor[pointsList.Length + 1];

            for (int i = 0; i < pointsList.Length; i++)
            {
                verticies[i] = new VertexPositionColor(new Vector3(pointsList[i], 0), Color.Black);
            }
            verticies[verticies.Length - 1] = new VertexPositionColor(new Vector3(pointsList[0], 0), Color.Black);

            foreach (EffectPass pass in basicEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                graphicsDevice.DrawUserPrimitives<VertexPositionColor>(
                    PrimitiveType.LineStrip,
                    verticies, 0, verticies.Length - 1
                );
                pass.End();
            }
        }
    }
}
