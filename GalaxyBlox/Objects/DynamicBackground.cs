using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Objects
{
    class DynamicBackgroundObject : GameObject
    {
        private Texture2D dynamicBackground;
        public Texture2D DynamicBackground
        {
            get { return dynamicBackground; }
            set { dynamicBackground = value; dynamicBackgroundChanged = true; }
        }

        private int dynamicBackgroundScale;
        public int DynamicBackgroundScale
        {
            get { return dynamicBackgroundScale; }
            set { dynamicBackgroundScale = value; dynamicBackgroundChanged = true; }
        }
        private bool dynamicBackgroundChanged;
        
        public DynamicBackgroundObject(Room parentRoom, Texture2D dynamicBackground, int dynamicBackgroundScale = 1) : base(parentRoom)
        {
            DynamicBackground = dynamicBackground;
            DynamicBackgroundScale = dynamicBackgroundScale;
        }

        public override void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (dynamicBackground != null && dynamicBackgroundChanged)
            {
                var backgroundTarget = new RenderTarget2D(graphicsDevice, (int)Size.X, (int)Size.Y);
                var pieceSizeX = dynamicBackground.Width / 3;
                var pieceSizeY = dynamicBackground.Height / 3;
                var realPieceSizeX = pieceSizeX * dynamicBackgroundScale;
                var realPieceSizeY = pieceSizeY * dynamicBackgroundScale;

                graphicsDevice.SetRenderTarget(backgroundTarget);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
                graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                graphicsDevice.Clear(Color.Transparent);

                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        var resultWidth = x != 1 ? realPieceSizeX : (int)(Size.X - 2 * realPieceSizeX);
                        var resultHeigth = y != 1 ? realPieceSizeY : (int)(Size.Y - 2 * realPieceSizeY);
                        var resultX = x != 2 ? realPieceSizeX * x : (int)(Size.X - realPieceSizeX);
                        var resultY = y != 2 ? realPieceSizeY * y : (int)(Size.Y - realPieceSizeY);


                        spriteBatch.Draw(dynamicBackground, new Rectangle(resultX, resultY, resultWidth, resultHeigth), new Rectangle(pieceSizeX * x, pieceSizeY * y, pieceSizeX, pieceSizeY), Color.White);
                    }
                }

                spriteBatch.End();
                graphicsDevice.SetRenderTarget(null);

                BackgroundImage = backgroundTarget;
                dynamicBackgroundChanged = false;
            }

            base.Prepare(spriteBatch, graphicsDevice);
        }
    }
}