using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyBlox.Models
{
    /// <summary>
    /// Sprite class - consists of Texture reference and rectangle that points to coordinates in given texture.
    /// Added to support multiple images in one texture and animation with sprite sheets.
    /// </summary>
    public class Sprite
    {
        public Texture2D TextureRef;
        public Rectangle SourceRectangle;

        public Sprite(Texture2D textureRef, Rectangle resourceRectangle)
        {
            TextureRef = textureRef;
            SourceRectangle = resourceRectangle;
        }
    }
}
