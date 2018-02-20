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
        private bool dynamicBackgroundChanged;

        public DynamicBackgroundObject(Room parentRoom, Texture2D dynamicBackground) : base(parentRoom)
        {
            DynamicBackground = dynamicBackground;
        }

        public override void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            if (dynamicBackground != null && dynamicBackgroundChanged)
            {
                var backgroundTarget = new RenderTarget2D(graphicsDevice, (int)Size.X, (int)Size.Y);
                var pieceSizeX = dynamicBackground.Width / 3;
                var pieceSizeY = dynamicBackground.Height / 3;

                graphicsDevice.SetRenderTarget(backgroundTarget);
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp);
                graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
                graphicsDevice.Clear(Color.Transparent);

                for (int x = 0; x < 3; x++)
                {
                    for (int y = 0; y < 3; y++)
                    {
                        var resultWidth = x != 1 ? pieceSizeX : (int)(Size.X - 2 * pieceSizeX);
                        var resultHeigth = y != 1 ? pieceSizeY : (int)(Size.Y - 2 * pieceSizeY);
                        var resultX = x != 2 ? pieceSizeX * x : (int)(Size.X - pieceSizeX);
                        var resultY = y != 2 ? pieceSizeY * y : (int)(Size.Y - pieceSizeY);

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