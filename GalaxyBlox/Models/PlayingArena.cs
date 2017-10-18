using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GalaxyBlox.Static;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Android.Util;

namespace GalaxyBlox.Models
{
    class PlayingArena : GameObject
    {
        private bool[,] actor;
        private Color actorColor;
        private Point actorPosition;

        private int[,] gamePlayground;
        private Color gamePlaygroundColor;
        private Vector2 gamePlaygroundPosition; // for more precision
        private Vector2 gamePlaygroundSize;
        private float gamePadding;
        private float gameCubeSize;
        private float gameCubePadding;

        private int gameSpeed = 1000; // move actor in 1000 ms = 1 s
        private int gameTimeElapsed = 0; 

        public PlayingArena(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom)
        {
            Size = size;
            Position = position;
            BackgroundImage = Contents.Textures.Pix;
            gamePlayground = new int[Settings.GameArenaSize.Width, Settings.GameArenaSize.Height];
            BackgroundColor = Contents.Colors.GamePlaygroundColor;

            if (Size.X / (float)Settings.GameArenaSize.Width < Size.Y / (float)Settings.GameArenaSize.Height)
            { // WIDTH
                gameCubeSize = (Size.X / (float)Settings.GameArenaSize.Width);
                Size = new Vector2(Size.X, Settings.GameArenaSize.Height * gameCubeSize);
                Position = new Vector2(Position.X, (size.Y - Size.Y) / 2f);
                //gamePlaygroundSize = new Vector2(Size.X, Settings.GameArenaSize.Height * gameCubeSize);
                //gamePlaygroundPosition = new Vector2(Position.X, (Size.Y - gamePlaygroundSize.Y) / 2);
            } else
            { // HEIGHT
                gameCubeSize = (Size.Y / (float)Settings.GameArenaSize.Height);
                Size = new Vector2(Settings.GameArenaSize.Width * gameCubeSize, Size.Y);
                Position = new Vector2((size.X - Size.X) / 2f, Position.Y);
                //gamePlaygroundSize = new Vector2(Settings.GameArenaSize.Width * gameCubeSize, Size.Y);
                //gamePlaygroundPosition = new Vector2((Size.X - gamePlaygroundSize.X) / 2, Position.Y);
            }
            
            gameCubePadding = (int)(gameCubeSize * 0.04f);
            gamePadding = (int)(gameCubeSize * 0.2f);
            gameCubeSize = (((Size.X - (float)((Settings.GameArenaSize.Width - 1) * gameCubePadding) - (float)(2 * gamePadding))) / (float)Settings.GameArenaSize.Width);

            CreateActor();
            Alpha = 0.75f;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (actor != null)
            {
                gameTimeElapsed += gameTime.ElapsedGameTime.Milliseconds;
                if (gameTimeElapsed > gameSpeed)
                {
                    //MoveActorDown();
                    gameTimeElapsed = 0;
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            for (int x = 0; x < gamePlayground.GetLength(0); x++)
            {
                for (int y = 0; y < gamePlayground.GetLength(1); y++)
                {
                    spriteBatch.Draw(BackgroundImage, GetCubeDisplayRectangle(x, y), GetCubeColor(x, y));
                }
            }

            //spriteBatch.Draw(
            //    newTexture,
            //    DisplayRectWithScaleAndRoomPosition(),
            //    Color);
        }

        private void MoveActorDown()
        {
            actorPosition.Y++;
        }

        private void CreateActor()
        {
            actor = new bool[3, 2];
            actor[0, 0] = true;
            actor[1, 0] = true;
            actor[2, 0] = true;
            actor[0, 1] = false;
            actor[1, 1] = true;
            actor[2, 1] = false;

            actorColor = Contents.Colors.GameCubesColors[Game1.Random.Next(1, Contents.Colors.GameCubesColors.Count - 1)];
            actorPosition = new Point(Game1.Random.Next(0, gamePlayground.GetLength(0) - actor.GetLength(0)), 0);
        }

        public Color GetCubeColor(int posX, int posY)
        {
            var result = Contents.Colors.GameCubesColors[gamePlayground[posX, posY]];
            if (gamePlayground[posX, posY] == 0)
                return result * 0.5f;

            return result;
        }

        public Rectangle GetCubeDisplayRectangle(int posX, int posY)
        {
            var offsetX = Origin.X * (Size.X * Scale - Size.X);
            var offsetY = Origin.Y * (Size.Y * Scale - Size.Y);

            var resultRect = new Rectangle(
                (int)((Position.X + gamePadding + (gameCubePadding + gameCubeSize) * posX) * ParentRoom.Scale + ParentRoom.InGameOffsetX - offsetX + ParentRoom.Position.X),
                (int)((Position.Y + gamePadding + (gameCubePadding + gameCubeSize) * posY) * ParentRoom.Scale + ParentRoom.InGameOffsetY - offsetY + ParentRoom.Position.Y),
                (int)(gameCubeSize * ParentRoom.Scale * Scale),
                (int)(gameCubeSize * ParentRoom.Scale * Scale));
            return resultRect;
        }
    }
}