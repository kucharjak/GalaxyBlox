using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GalaxyBlox.Static;
using GalaxyBlox.Models;

namespace GalaxyBlox.Objects
{
    class ActorViewer : GameObject
    {
        private bool[,] actor;
        private Color actorColor;
        private Color backgroundColor;

        private Point cubesOffset;
        private int cubeSize;
        private int cubeBorderSize;

        private bool actorDrawn;

        private RenderTarget2D renderTarget;
        public ActorViewer(Room parentRoom, Vector2 size, Color backgroundColor, int cubeSize = 25, int cubeBorderSize = 2) : base(parentRoom)
        {
            this.backgroundColor = backgroundColor;
            this.cubeSize = cubeSize;
            this.cubeBorderSize = cubeBorderSize;

            Size = size;
            renderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, 1, 1);
            BackgroundImage = renderTarget;
        }

        public void ResetActor()
        {
            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            }

            renderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, 1, 1);
            BackgroundImage = renderTarget;
            actor = null;
            actorDrawn = false;
        }

        public void SetActor(bool[,] actor, Color actorColor)
        {
            this.actor = actor; 
            this.actorColor = actorColor;

            // calculate size of renderTarget
            var cubeCount = actor.GetLength(0) > actor.GetLength(1) ? actor.GetLength(0) : actor.GetLength(1);
            cubeCount += 2; // padding
            var renderTargetSize = (cubeCount * cubeSize) + ((cubeCount - 1) * cubeBorderSize);

            if (renderTarget != null)
            {
                renderTarget.Dispose();
                renderTarget = null;
            } 

            renderTarget = new RenderTarget2D(Game1.ActiveGame.GraphicsDevice, renderTargetSize, renderTargetSize);
            BackgroundImage = renderTarget;
            cubesOffset = new Point(
                (int)(renderTargetSize - cubeSize * actor.GetLength(0) - actor.GetLength(0) * cubeBorderSize) / 2,
                (int)(renderTargetSize - cubeSize * actor.GetLength(1) - actor.GetLength(1) * cubeBorderSize) / 2);

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
                            new Rectangle((int)cubesOffset.X + x * (cubeSize + cubeBorderSize), (int)cubesOffset.Y + y * (cubeSize + cubeBorderSize), cubeSize, cubeSize),
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