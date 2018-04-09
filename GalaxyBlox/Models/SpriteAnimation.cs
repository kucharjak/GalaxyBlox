using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GalaxyBlox.Models
{
    public class SpriteAnimation
    {
        public GameObject Parent;

        public Sprite ActiveSprite;
        public int Position;
        public int Rounds;

        private int fps;
        public int FPS
        {
            get { return fps; }
            set { fps = value; timerLimit = 1000 / value; timer = 0; }
        }
        
        public bool Pause = false;
        public bool Loop = true;

        List<Sprite> sprites;
        int timer;
        int timerLimit;
        bool start;

        public event EventHandler AnimationEnd;
        protected virtual void OnAnimationEnd(EventArgs e)
        {
            EventHandler handler = AnimationEnd;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler AnimationLoop;
        protected virtual void OnAnimationLoop(EventArgs e)
        {
            EventHandler handler = AnimationLoop;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event EventHandler AnimationNext;
        protected virtual void OnAnimationNext(EventArgs e)
        {
            EventHandler handler = AnimationNext;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public SpriteAnimation(List<Sprite> sprites, int fps, bool start = true)
        {
            this.sprites = sprites;
            FPS = fps;
            Rounds = 0;
            Position = 0;
            this.start = start;

            if (sprites != null && sprites.Count > 0)
                ActiveSprite = sprites[Position];
        }

        public void Update(GameTime gameTime)
        {
            if (Pause || sprites == null || sprites.Count == 0)
                return;

            if (start)
            {
                start = false;
                return;
            }

            timer += gameTime.ElapsedGameTime.Milliseconds;
            if (timer >= timerLimit)
            {
                timer = 0;
                if (Position + 1 < sprites.Count)
                {
                    Position++;
                    OnAnimationNext(new EventArgs());
                }
                else
                {
                    if (Loop)
                    {
                        Position = 0;
                        OnAnimationLoop(new EventArgs());
                        OnAnimationNext(new EventArgs());
                    }
                    else
                    {
                        Pause = true;
                        OnAnimationEnd(new EventArgs());
                    }
                }

                ActiveSprite = sprites[Position];
            }

        }

        public SpriteAnimation Copy(bool? start = null)
        {
            var copy = new SpriteAnimation(sprites, fps, start.HasValue ? start.Value : Pause)
            {
                Loop = this.Loop
            };
            return copy;
        }
    }
}
