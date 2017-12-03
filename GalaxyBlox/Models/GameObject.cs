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
            set
            {
                size = value;
                if (text != "")
                    UpdateTextPosition();
            }
        }
        public Vector2 Position;
        public Vector2 Origin;
        public float Scale = 1f; // scale for drawing - for example when i need bigger object for a second (animation, or just to stand out like hovered button)

        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                textSize = TextSpriteFont.MeasureString(text);
                UpdateTextPosition();
            }
        }
        private string text;
        private TextAlignment textAlignment;
        public TextAlignment TextAlignment
        {
            get { return textAlignment; }
            set
            {
                textAlignment = value;
                UpdateTextPosition();
            }
        } 
        public bool ShowText = false;
        public Color TextColor = Color.Black;
        public Point TextOffset = new Point();
        protected Vector2 textSize = new Vector2();
        public SpriteFont TextSpriteFont = null;
        public Vector2 TextPosition { get { return new Vector2(Position.X + TextOffset.X, Position.Y + TextOffset.Y); } }
        public float TextScale = 1f;

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
                spriteBatch.DrawString(TextSpriteFont, Text, DisplayText(), TextColor * baseAlpha, 0f, new Vector2(), Scale, SpriteEffects.None, ParentRoom.LayerDepth + LayerDepth + 0.01f);
        }

        private void UpdateTextPosition()
        {
            switch(TextAlignment)
            {
                case TextAlignment.Left: TextOffset = new Point(0, (int)((Size.Y - textSize.Y / ParentRoom.Scale) / 2)); break;
                case TextAlignment.Right: TextOffset = new Point((int)((Size.X - textSize.X / ParentRoom.Scale)), (int)((Size.Y - textSize.Y / ParentRoom.Scale) / 2)); break;
                case TextAlignment.Center: TextOffset = new Point((int)((Size.X - textSize.X / ParentRoom.Scale) / 2), (int)((Size.Y - textSize.Y / ParentRoom.Scale) / 2)); break;
            }
        }

        public Vector2 DisplayText()
        {
            var offsetX = Origin.X * (textSize.X * Scale - textSize.X);
            var offsetY = Origin.Y * (textSize.Y * Scale - textSize.Y);

            var resultVect = new Vector2(
                (TextPosition.X + ParentRoom.Position.X) * ParentRoom.Scale + ParentRoom.InGameOffsetX - offsetX,
                (TextPosition.Y + ParentRoom.Position.Y) * ParentRoom.Scale + ParentRoom.InGameOffsetY - offsetY);
            return resultVect;
        }

        public Rectangle DisplayRect()
        {
            var offsetX = Origin.X * (Size.X * Scale - Size.X);
            var offsetY = Origin.Y * (Size.Y * Scale - Size.Y);

            var resultRect = new Rectangle(
                (int)((Position.X + ParentRoom.Position.X) * ParentRoom.Scale + ParentRoom.InGameOffsetX - offsetX),
                (int)((Position.Y + ParentRoom.Position.Y) * ParentRoom.Scale + ParentRoom.InGameOffsetY - offsetY),
                (int)(Size.X * ParentRoom.Scale * Scale),
                (int)(Size.Y * ParentRoom.Scale * Scale)
                );
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