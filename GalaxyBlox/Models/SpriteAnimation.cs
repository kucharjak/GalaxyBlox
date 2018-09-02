using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace GalaxyBlox.Models
{
    /// <summary>
    /// Sprite animation component - list of sprite images, timer and events (AnimationEnd, AnimationLoop, AnimationNext).
    /// To be able to fire animation events correctly there exists a reference to parent GameObject - that is not an ideal way and  it should be done differently. 
    /// </summary>
    public class SpriteAnimation
    {
        /// <summary>
        /// Reference to parent object.
        /// </summary>
        public GameObject Parent;

        /// <summary>
        /// Currently visible frame from animation.
        /// </summary>
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

        /// <summary>
        /// Sprite Animation Constructor
        /// </summary>
        /// <param name="sprites">SpriteImages collection</param>
        /// <param name="fps">Frames per second</param>
        /// <param name="start">True - animation starts immediately / False - you need to start animation yourself</param>
        public SpriteAnimation(List<Sprite> sprites, int fps, bool start = true)
        {
            this.sprites = sprites;
            this.FPS = fps;
            this.Rounds = 0;
            this.Position = 0;
            this.start = start;

            if (sprites != null && sprites.Count > 0)
                this.ActiveSprite = sprites[Position];
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
