using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GalaxyBlox.Models
{
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
