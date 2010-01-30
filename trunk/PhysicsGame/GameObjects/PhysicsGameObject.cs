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

namespace PhysicsGame.GameObjects
{
    class PhysicsGameObject : GameObject, Drawable
    {
        private List<string> textureNames;
        private Dictionary<string, int> currentTextureListIndex;
        private Dictionary<string, List<Texture2D>> textures;


        private Nullable<Rectangle> sourceRect, sourceRectAdjustedForScale;
        
        public Body boxBody;
        public Geom boxGeom;
        private float width, height;
        private Vector2 scale;

        //private int changeImageCounter, changeImageFrequency;

        public PhysicsGameObject(PhysicsSimulator physicsSimulator, float width, float height, bool isStatic)
        {
            //changeImageCounter = changeImageFrequency = 100;

            textureNames = new List<string>();
            currentTextureListIndex = new Dictionary<string,int>();
            textures = new Dictionary<string, List<Texture2D>>();

            addTextureSet("Default");

            this.width = width;
            this.height = height;

            scale = new Vector2(1.0f, 1.0f);
            sourceRect = new Rectangle(0, 0, (int)width, (int)height);
            sourceRectAdjustedForScale = new Rectangle(0, 0, (int)width, (int)height);
            calculateSourceRectAdjustedForScale();


            boxBody = BodyFactory.Instance.CreateRectangleBody(physicsSimulator, width, height, 1);
            boxBody.Position = new Vector2(0,0);
            boxBody.IsStatic = isStatic;
            //boxBody.AngularVelocity = 0;

            boxGeom = GeomFactory.Instance.CreateRectangleGeom(physicsSimulator, boxBody, width, height);
        }


        public void Update()
        {
        }

        public void setIndexOfTexture(string textureListName, int index)
        {
            currentTextureListIndex[textureListName] = index;
        }

        public void addTextureSet(string textureListName)
        {
            textureNames.Add(textureListName);
        }
        public void removeTextureSet(string textureListName)
        {
            textureNames.Remove(textureListName);
        }

        public bool addTexture(Texture2D sprite)
        {
            return addTexture(sprite, "Default");
        }
        public bool addTexture (Texture2D sprite, string textureListName)
        {
            if (textureListName == "")
                return false;

            if (!textures.ContainsKey(textureListName))
            {
                textures[textureListName] = new List<Texture2D>();
                currentTextureListIndex[textureListName] = 0;
            }

            textures[textureListName].Add(sprite);


            return true;
        }

        public void setTexturePosition(int x, int y)
        {
            sourceRect = new Rectangle(x, y, (int)width, (int)height);
            calculateSourceRectAdjustedForScale();
        }

        private void calculateSourceRectAdjustedForScale()
        {
            if (sourceRect != null)
            {
                Rectangle adjSrc = new Rectangle();
                adjSrc.X = sourceRect.Value.X;
                adjSrc.Y = sourceRect.Value.Y;
                adjSrc.Width  = (int)(sourceRect.Value.Width / scale.X);
                adjSrc.Height = (int)(sourceRect.Value.Height / scale.Y);
                sourceRectAdjustedForScale = adjSrc;
            }
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
            setScale(width / textures["Default"][currentTextureListIndex["Default"]].Width,
                      height / textures["Default"][currentTextureListIndex["Default"]].Height);
        }

        public void setScaleByNumberOfPixels(int numberOfPixelsX, int numberOfPixelsY)
        {
            setScale( width  / (float)numberOfPixelsX,
                      height / (float)numberOfPixelsY);
        }

        public void draw(SpriteBatch spriteBatch)
        {
            foreach (string name in textureNames)
            {
                spriteBatch.Draw(textures[name][currentTextureListIndex[name]], boxGeom.Position, sourceRectAdjustedForScale, Color.White, boxGeom.Rotation, new Vector2(width / 2 / scale.X, height / 2 / scale.Y), scale, SpriteEffects.None, 1.0f);
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
