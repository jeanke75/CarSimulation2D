using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace CarSimulation2D.Objects
{
    public class Tile
    {
        #region Field Region
        int size = 32;
        Vector2 position;

        Texture2D texture;

        bool solid;
        #endregion

        #region Property Region
        public int Width
        {
            get { return size; }
        }

        public int Height
        {
            get { return size; }
        }

        public Rectangle BoundingBox
        {
            get { return new Rectangle((int)position.X, (int)position.Y, size, size); }
        } 

        public bool Solid
        {
            get { return solid; }
        }
        #endregion

        #region Constructor Region
        public Tile(int size, Vector2 position, Texture2D texture, bool solid)
        {
            this.size = size;
            this.position = position;
            this.texture = texture;
            this.solid = solid;
        }
        #endregion

        #region Method Region
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, BoundingBox, Color.White);
        }
        #endregion
    }
}
