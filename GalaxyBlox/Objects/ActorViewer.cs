using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using GalaxyBlox.Static;
using GalaxyBlox.Models;
using GalaxyBlox.Utils;

namespace GalaxyBlox.Objects
{
    class ActorViewer : GameObject
    {
        private bool[,] actor;
        private Color actorColor;
        private Color backgroundColor;

        private Point cubesOffset;
        private int cubeSize;
        private int cubesPadding;

        private bool actorDrawn;
        
        private Vector2 backgroundSize;
        private RenderTarget2D renderTarget;

        public ActorViewer(Room parentRoom, Vector2 size, Color backgroundColor) : base(parentRoom)
        {
            this.backgroundColor = backgroundColor;

            Size = size;
            backgroundSize = new Vector2(DisplayRect().Size.X, DisplayRect().Size.Y);
            renderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)backgroundSize.X, (int)backgroundSize.Y);
            SpriteImage = new Sprite(renderTarget, renderTarget.GetRectangle());

            cubesPadding = 1;
        }

        public void ResetActor()
        {
            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }

            renderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, (int)backgroundSize.X, (int)backgroundSize.Y);
            SpriteImage = new Sprite(renderTarget, renderTarget.GetRectangle());
            actor = null;
            actorDrawn = false;
        }

        public void SetActor(bool[,] actor, Color actorColor)
        {
            this.actor = actor; 
            this.actorColor = actorColor;

            // calculate size of renderTarget
            var actorWidth = actor.GetLength(0);
            var actorHeight = actor.GetLength(1);
            var cubeCount = actorWidth > actorHeight ? actorWidth : actorHeight;

            cubeCount += 2; // padding

            if (backgroundSize.X > backgroundSize.Y)
            { // HEIGTH is smaller
                cubeSize = (int)((backgroundSize.Y - ((cubeCount - 1) * cubesPadding)) / cubeCount);
            }
            else
            { // WIDTH is smaller
                cubeSize = (int)((backgroundSize.X - ((cubeCount - 1) * cubesPadding)) / cubeCount);
            }

            cubesOffset = new Point(
                (int)(backgroundSize.X - cubeSize * actorWidth - (actorWidth - 1) * cubesPadding) / 2,
                (int)(backgroundSize.Y - cubeSize * actorHeight - (actorHeight - 1 )* cubesPadding) / 2);

            actorDrawn = false;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        public override void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            base.Prepare(spriteBatch, graphicsDevice);

            if (actorDrawn)
                return;

            graphicsDevice.SetRenderTarget(renderTarget);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque);

            graphicsDevice.DepthStencilState = new DepthStencilState() { DepthBufferEnable = true };
            graphicsDevice.Clear(backgroundColor);

            if (actor != null)
            {
                for (int x = 0; x < actor.GetLength(0); x++)
                {
                    for (int y = 0; y < actor.GetLength(1); y++)
                    {
                        if (!actor[x, y])
                            continue;

                        spriteBatch.Draw(
                            Contents.Textures.Pix,
                            new Rectangle((int)cubesOffset.X + x * (cubeSize + cubesPadding), (int)cubesOffset.Y + y * (cubeSize + cubesPadding), cubeSize, cubeSize),
                            actorColor);
                    }
                }
            }

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
            actorDrawn = true;
        }
    }
}