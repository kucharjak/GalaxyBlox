using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyBlox.Models
{
    public class GameObject
    {
        public string Name = "";
        public string Type = "";
        protected Room ParentRoom;

        private Vector2 size;
        public Vector2 Size
        {
            get { return size; }
            set { size = value; UpdateTextPosition(); }
        }
        public Vector2 Position;
        public Vector2 Origin;
        public float Scale = 1f; // scale for drawing - for example when i need bigger object for a second (animation, or just to stand out like hovered button)

        private string text = "";
        public string Text
        {
            get { return text; }
            set { text = value; UpdateTextPosition(); }
        }
        public Vector2 TextPosition { get { return new Vector2(Position.X + textOffset.X, Position.Y + textOffset.Y); } }
        private TextAlignment textAlignment;
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set { textAlignment = value; UpdateTextPosition(); }
        } 
        public bool ShowText = false;
        public Color TextColor = Color.Black;
        private Point textOffset = new Point();

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

        public Texture2D BackgroundImage = null;
        public Color Color { get { return BaseColor * Alpha; } }
        public Color BaseColor = Color.White;
        public float Alpha = 1f;
        public float LayerDepth = 0f;

        public bool Enabled = true;

        public Rectangle Rectangle { get { return new Rectangle((int)(Position.X), (int)(Position.Y), (int)Size.X, (int)Size.Y); } }

        public GameObject(Room parentRoom)
        {
            ParentRoom = parentRoom;
        }

        public virtual void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;
        }

        public virtual void Prepare(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            var baseAlpha = Enabled ? 1f : .25f;

            if (BackgroundImage != null)
                spriteBatch.Draw(BackgroundImage, DisplayRect(), null, Color * baseAlpha, 0, new Vector2(), SpriteEffects.None, ParentRoom.LayerDepth + LayerDepth);

            if (ShowText && TextSpriteFont != null)
                spriteBatch.DrawString(TextSpriteFont, Text, DisplayTextPosition(), TextColor * baseAlpha, 0f, new Vector2(), Scale * textScale * ParentRoom.Scale, SpriteEffects.None, ParentRoom.LayerDepth + LayerDepth + 0.01f);
        }

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

        public Rectangle DisplayRect()
        {
            var offsetX = Origin.X * (Size.X * Scale - Size.X);
            var offsetY = Origin.Y * (Size.Y * Scale - Size.Y);

            var resultRect = new Rectangle(
                (int)((Position.X + ParentRoom.Position.X - offsetX) * ParentRoom.Scale + ParentRoom.InGameOffsetX),
                (int)((Position.Y + ParentRoom.Position.Y - offsetY) * ParentRoom.Scale + ParentRoom.InGameOffsetY),
                (int)(Size.X * ParentRoom.Scale * Scale),
                (int)(Size.Y * ParentRoom.Scale * Scale));
            return resultRect;
        }
    }

    public enum TextAlignment
    {
        Left,
        Right,
        Center
    }
}