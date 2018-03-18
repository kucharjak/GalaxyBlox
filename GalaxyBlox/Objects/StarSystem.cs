using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalaxyBlox.Models;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace GalaxyBlox.Objects
{
    class StarSystem : GameObject
    {
        private Dictionary<Star.StarType, int> starSizes;
        private Dictionary<Star.StarType, Texture2D> starTextures;

        private List<Star> stars;

        public int MaxTimer;
        public float MaxStarScale;

        private int seed;

        public StarSystem(Room parentRoom, Vector2 size, Vector2 position) : base(parentRoom)
        {
            MaxTimer = 5000;
            MaxStarScale = 1.55f;

            Position = position;
            Size = size;

            starSizes = new Dictionary<Star.StarType, int>();
            starSizes.Add(Star.StarType.tiny, 4);
            starSizes.Add(Star.StarType.small, 12);
            starSizes.Add(Star.StarType.medium_01, 36);
            starSizes.Add(Star.StarType.medium_02, 28);
            starSizes.Add(Star.StarType.big, 44);

            starTextures = new Dictionary<Star.StarType, Texture2D>();
            starTextures.Add(Star.StarType.tiny, Static.Contents.Textures.Pix);
            starTextures.Add(Star.StarType.small, Static.Contents.Textures.Star_small);
            starTextures.Add(Star.StarType.medium_01, Static.Contents.Textures.Star_medium_01);
            starTextures.Add(Star.StarType.medium_02, Static.Contents.Textures.Star_medium_02);
            starTextures.Add(Star.StarType.big, Static.Contents.Textures.Star_big);
        }

        public void Start(int rndSeed, int bigStarCount, int medium01StarCount, int medium02StarCount, int smallStarCount, int tinyStarCount)
        {
            seed = rndSeed;
            var random = new Random(rndSeed);
            stars = new List<Star>();

            var starCounts = new[]
            {
                new {Type = Star.StarType.big,          Count = bigStarCount},
                new {Type = Star.StarType.medium_01,    Count = medium01StarCount},
                new {Type = Star.StarType.medium_02,    Count = medium02StarCount},
                new {Type = Star.StarType.small,        Count = smallStarCount},
                new {Type = Star.StarType.tiny,         Count = tinyStarCount}
            };

            foreach (var star in starCounts)
            {
                var size = starSizes[star.Type];
                for (int i = 0; i < star.Count; i++)
                    stars.Add(new Star()
                    {
                        Type = star.Type,
                        Position = new Vector2(random.Next(0, (int)Size.X - size), random.Next(0, (int)Size.Y - size)),
                        Limit = MaxTimer,
                        Timer = random.Next(0, MaxTimer)
                    });
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            foreach (var star in stars)
            {
                if (star.ScalingUp)
                {
                    star.Timer += gameTime.ElapsedGameTime.Milliseconds;
                    if (star.Timer > MaxTimer)
                        star.ScalingUp = false;
                }
                else
                {
                    star.Timer -= gameTime.ElapsedGameTime.Milliseconds;
                    if (star.Timer <= 0)
                        star.ScalingUp = true;
                }

                star.Scale = 1 + star.Timer / (float)MaxTimer * (MaxStarScale - 1);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            foreach (var star in stars)
            {
                var size = (int)(starSizes[star.Type] * star.Scale);
                var starRect = new Rectangle(
                    (int)(Position.X + star.Position.X - (size / 2)),
                    (int)(Position.Y + star.Position.Y - (size / 2)),
                    size,
                    size);

                spriteBatch.Draw(
                    starTextures[star.Type],
                    DisplayRect(starRect),
                    null,
                    Color.White,
                    0f,
                    new Vector2(),
                    SpriteEffects.None,
                    ParentRoom.LayerDepth + LayerDepth
                    );
            }
        }
    }

    public class Star
    {
        public Vector2 Position;
        public StarType Type;
        public int Timer;
        public int Limit;
        public float Scale = 1f;
        public bool ScalingUp = true;

        public enum StarType
        {
            tiny,
            small,
            medium_01,
            medium_02,
            big
        };
    }
}