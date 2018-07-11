using System;
using System.Collections.Generic;
using CarSimulation2D.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CarSimulation2D
{
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // fonts
        SpriteFont font;

        // tiles
        readonly int TILE_SIZE = 32;
        Texture2D grasTexture;
        Texture2D roadTexture;
        Texture2D wallCornerTexture;
        Texture2D wallNorthTexture;
        Texture2D wallEastTexture;
        Texture2D wallSouthTexture;
        Texture2D wallWestTexture;
        Texture2D grasToRoadTexture;

        // map
        List<Tile> map;

        // car
        Car car1;
        Texture2D carTexture;
        Texture2D wheelTexture;


        public GraphicsDeviceManager Graphics
        {
            get { return graphics; }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            Components.Add(new Xin(this));

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            // load fonts
            font = Content.Load<SpriteFont>("Font");

            // load tile textures
            grasTexture = Content.Load<Texture2D>("Tiles/Tile_gras");
            roadTexture = Content.Load<Texture2D>("Tiles/Tile_road");
            wallCornerTexture = Content.Load<Texture2D>("Tiles/Tile_wall_corner");
            wallNorthTexture = Content.Load<Texture2D>("Tiles/Tile_wall_north");
            wallEastTexture = Content.Load<Texture2D>("Tiles/Tile_wall_east");
            wallSouthTexture = Content.Load<Texture2D>("Tiles/Tile_wall_south");
            wallWestTexture = Content.Load<Texture2D>("Tiles/Tile_wall_west");
            grasToRoadTexture = Content.Load<Texture2D>("Tiles/Tile_gras_to_road");

            // create map
            int maxHeight = graphics.PreferredBackBufferHeight - TILE_SIZE;
            int maxWidth = graphics.PreferredBackBufferWidth - TILE_SIZE;
            Random rnd = new Random();
            map = new List<Tile>();
            for (int row = 0; row < graphics.PreferredBackBufferHeight; row += TILE_SIZE)
            {
                for (int col = 0; col < graphics.PreferredBackBufferWidth; col += TILE_SIZE)
                {
                    if ((row == 0 || row == maxHeight) && (col == 0 || col == maxWidth))
                        map.Add(new Tile(TILE_SIZE, new Vector2(col, row), wallCornerTexture, true));
                    else if (row == 0)
                        map.Add(new Tile(TILE_SIZE, new Vector2(col, row), wallNorthTexture, true));
                    else if (row == maxHeight)
                        map.Add(new Tile(TILE_SIZE, new Vector2(col, row), wallSouthTexture, true));
                    else if (col == 0)
                        map.Add(new Tile(TILE_SIZE, new Vector2(col, row), wallWestTexture, true));
                    else if (col == maxWidth)
                        map.Add(new Tile(TILE_SIZE, new Vector2(col, row), wallEastTexture, true));
                    else
                    {
                        if (rnd.Next(100) < 5)
                            map.Add(new Tile(TILE_SIZE, new Vector2(col, row), grasTexture, false));
                        else
                            map.Add(new Tile(TILE_SIZE, new Vector2(col, row), roadTexture, false));
                    }  
                }
            }

            // load car textures
            carTexture = Content.Load<Texture2D>("Car");
            wheelTexture = Content.Load<Texture2D>("Wheel");

            // create car object
            car1 = new Car(carTexture, wheelTexture, font);
        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            car1.Update(gameTime, map);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
            foreach (Tile tile in map)
            {
                tile.Draw(spriteBatch);
            }
            spriteBatch.End();

            car1.Draw(gameTime, spriteBatch);

            base.Draw(gameTime);
        }
    }
}
