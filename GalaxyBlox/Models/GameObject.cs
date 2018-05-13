using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyBlox.Models
{
    /// <summary>
    /// Basic object that game handles.
    /// All non bassic objects inherit from this.
    /// </summary>
    public class GameObject
    {
        public string Name = "";
        public string Type = "";

        /// <summary>
        /// Room in which object exists.
        /// </summary>
        protected Room ParentRoom;

        /// <summary>
        /// Position relative to room in which object exists.
        /// </summary>
        public Vector2 Position;

        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set { size = value; UpdateTextPosition(); }
        }

        /// <summary>
        /// Origin of object used for scaling and centering object to right position.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// scale for drawing - for example when i need bigger object for a second (animation, or just to stand out like hovered button)
        /// </summary>
        public float Scale = 1f;

        private string text = "";
        public string Text
        {
            get { return text; }
            set { text = value; UpdateTextPosition(); }
        }

        private TextAlignment textAlignment;
        /// <summary>
        /// Text alignment - left, right and center.
        /// </summary>
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set { textAlignment = value; UpdateTextPosition(); }
        }
        public Vector2 TextPosition { get { return new Vector2(Position.X + textOffset.X, Position.Y + textOffset.Y); } }
        private Point textOffset = new Point();

        public bool ShowText = false;
        public Color TextColor = Color.Black;

        private SpriteFont textSpriteFont = null;
        public SpriteFont TextSpriteFont
        {
            get { return textSpriteFont; }
            set { textSpriteFont = value; TextHeight = 10; }
        }

        private int textHeight;
        public int TextHeight
        {
            get { return textHeight; }
            set { textHeight = value; CalculateTextScale(); UpdateTextPosition(); }
        }
        private float textScale = 1f;

        private void CalculateTextScale()
        {
            textScale = textHeight / (float)textSpriteFont.LineSpacing;
        }

        public Sprite SpriteImage = null;
        public SpriteAnimation SpriteAnimation = null;

        public Color Color { get { return BaseColor * Alpha; } }
        public Color BaseColor = Color.White;
        public float Alpha = 1f;
        public float LayerDepth = 0f;

        public bool Visible = true;
        public bool Enabled = true;

        public object Data = null;

        public bool Destroyed = false;

        public Rectangle Rectangle { get { return new Rectangle((int)(Position.X), (int)(Position.Y), (int)Size.X, (int)Size.Y); } }

        public GameObject(Room parentRoom)
        {
            ParentRoom = parentRoom;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (SpriteAnimation != null)
            {
                SpriteAnimation.Update(gameTime);

                if (SpriteAnimation.ActiveSprite != null)
                    SpriteImage = SpriteAnimation.ActiveSprite;
            }
        }

        public virtual void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            if (!Visible)
                return;

            var baseAlpha = Enabled ? 1f : .25f;

            if (SpriteImage != null)
                spriteBatch.Draw(SpriteImage.TextureRef, DisplayRect(), SpriteImage.SourceRectangle, Color * baseAlpha, 0, new Vector2(), SpriteEffects.None, ParentRoom.LayerDepth + LayerDepth);

            if (ShowText && TextSpriteFont != null)
                spriteBatch.DrawString(TextSpriteFont, Text, DisplayTextPosition(), TextColor * Alpha * baseAlpha, 0f, new Vector2(), Scale * textScale * ParentRoom.Scale, SpriteEffects.None, ParentRoom.LayerDepth + LayerDepth + 0.01f);
        }

        /// <summary>
        /// Whenever something related to text is changed this method recalculate right text position.
        /// </summary>
        private void UpdateTextPosition()
        {
            if (string.IsNullOrEmpty(text))
                return;

            var textSize = textSpriteFont.MeasureString(text);
            textSize = new Vector2(textSize.X * textScale, textSize.Y * textScale);

            switch(TextAlignment)
            {
                case TextAlignment.Left: textOffset = new Point(0, (int)((Size.Y - textSize.Y ) / 2)); break;
                case TextAlignment.Right: textOffset = new Point((int)((Size.X - textSize.X)), (int)((Size.Y - textSize.Y) / 2)); break;
                case TextAlignment.Center: textOffset = new Point((int)((Size.X - textSize.X) / 2), (int)((Size.Y - textSize.Y) / 2)); break;
            }
        }

        /// <summary>
        /// Method that returns real position of text on-screen.
        /// </summary>
        /// <returns></returns>
        public Vector2 DisplayTextPosition()
        {
            var textSize = textSpriteFont.MeasureString(text);
            textSize = new Vector2(textSize.X * textScale, textSize.Y * textScale);

            var offsetX = Origin.X * (textSize.X * Scale - textSize.X);
            var offsetY = Origin.Y * (textSize.Y * Scale - textSize.Y);

            var resultVect = new Vector2(
                (TextPosition.X + ParentRoom.Position.X - offsetX) * ParentRoom.Scale + ParentRoom.InGameOffsetX,
                (TextPosition.Y + ParentRoom.Position.Y - offsetY) * ParentRoom.Scale + ParentRoom.InGameOffsetY);
            return resultVect;
        }

        /// <summary>
        /// Method that returns real positon of object on-screen.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle DisplayRect(Rectangle rect)
        {
            var offsetX = Origin.X * (Size.X * Scale - Size.X);
            var offsetY = Origin.Y * (Size.Y * Scale - Size.Y);

            var resultRect = new Rectangle(
                (int)((rect.X + ParentRoom.Position.X - offsetX) * ParentRoom.Scale + ParentRoom.InGameOffsetX),
                (int)((rect.Y + ParentRoom.Position.Y - offsetY) * ParentRoom.Scale + ParentRoom.InGameOffsetY),
                (int)(rect.Size.X * ParentRoom.Scale * Scale),
                (int)(rect.Size.Y * ParentRoom.Scale * Scale));
            return resultRect;
        }

        /// <summary>
        /// Method that returns real positon of object on-screen.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public Rectangle DisplayRect()
        {
            return DisplayRect(new Rectangle((int)Position.X, (int)Position.Y, (int)Size.X, (int)Size.Y));
        }
    }

    /// <summary>
    /// Text alignment setting options.
    /// </summary>
    public enum TextAlignment
    {
        Left,
        Right,
        Center
    }
}