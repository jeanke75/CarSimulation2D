using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CarSimulation2D.Objects
{
    public class Car
    {
        #region Field Region
        Vector2 carSize;
        Vector2 carLocation;
        float carHeading;
        float carSpeed;
        float steerAngle;
        float wheelBase; // the distance between the two axles

        readonly float axleDistancePerc = 7.0f / 10.0f;
        readonly float maxTurnAngle = 0.35f;
        readonly float maxCarSpeed = 300.0f;
        readonly float maxCarReverseSpeed = 100.0f;
        readonly float carAcceleration = 200.0f;

        Texture2D carTexture;
        Texture2D wheelTexture;
        SpriteFont font;

        readonly float SCALE = 0.50f;
        #endregion

        #region Constructor Region
        public Car(Texture2D carTexture, Texture2D wheelTexture, SpriteFont font)
        {
            carLocation = new Vector2(200, 200);

            carSize = new Vector2(carTexture.Width, carTexture.Height);
            carHeading = 0;
            carSpeed = 0;
            steerAngle = 0;
            wheelBase = carSize.X * axleDistancePerc;

            this.carTexture = carTexture;
            this.wheelTexture = wheelTexture;
            this.font = font;
        }
        #endregion

        #region Method Region
        public void Update(GameTime gameTime, List<Tile> map)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // current wheel positions
            Vector2 frontWheel = carLocation + wheelBase / 2 * new Vector2((float)Math.Cos(carHeading), (float)Math.Sin(carHeading));
            Vector2 backWheel = carLocation - wheelBase / 2 * new Vector2((float)Math.Cos(carHeading), (float)Math.Sin(carHeading));

            // update steerangle
            if (Xin.KeyboardState.IsKeyDown(Keys.Left))
            {
                steerAngle = MathHelper.Clamp(steerAngle - (2 * maxTurnAngle * delta), -maxTurnAngle, maxTurnAngle);
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.Right))
            {
                steerAngle = MathHelper.Clamp(steerAngle + (2 * maxTurnAngle * delta), -maxTurnAngle, maxTurnAngle);
            }
            else if (steerAngle != 0)
            {
                if (steerAngle < 0)
                    steerAngle = MathHelper.Clamp(steerAngle + (maxTurnAngle * delta), -maxTurnAngle, 0);
                else
                    steerAngle = MathHelper.Clamp(steerAngle - (maxTurnAngle * delta), 0, maxTurnAngle);
            }

            // update car speed
            if (Xin.KeyboardState.IsKeyDown(Keys.Up))
            {
                carSpeed = MathHelper.Clamp(carSpeed + (carAcceleration * delta), -maxCarReverseSpeed, maxCarSpeed);
            }
            else if (Xin.KeyboardState.IsKeyDown(Keys.Down))
            {
                carSpeed = MathHelper.Clamp(carSpeed - (carAcceleration * delta), -maxCarReverseSpeed, maxCarSpeed);
            }
            else if (carSpeed != 0)
            {
                if (carSpeed > 0)
                {
                    carSpeed = MathHelper.Clamp(carSpeed - (carAcceleration * delta), 0, maxCarSpeed);
                }
                else
                {
                    carSpeed = MathHelper.Clamp(carSpeed + (carAcceleration * delta), -maxCarReverseSpeed, 0);
                }
                
            }

            // new wheel positions after moving
            backWheel += carSpeed * delta * new Vector2((float)Math.Cos(carHeading), (float)Math.Sin(carHeading));
            frontWheel += carSpeed * delta * new Vector2((float)Math.Cos(carHeading + steerAngle), (float)Math.Sin(carHeading + steerAngle));

            Vector2 tempLoc = (frontWheel + backWheel) / 2.0f;
            Rectangle boundingbox = new Rectangle((int)((tempLoc.X - carSize.X / 2) * SCALE), (int)((tempLoc.Y - carSize.X / 2) * SCALE), (int)(carSize.X * SCALE), (int)(carSize.X * SCALE));
            if (HasCollisions(map, boundingbox))
            {
                carSpeed = 0;
            }
            else
            {
                // car location (center of rectangle)
                carLocation = tempLoc;
                carHeading = (float)Math.Atan2(frontWheel.Y - backWheel.Y, frontWheel.X - backWheel.X);
            }
        }

        private bool HasCollisions(List<Tile> map, Rectangle boundingbox)
        {
            foreach(Tile tile in map.Where(x => x.Solid))
            {
                if (tile.BoundingBox.Intersects(boundingbox))
                {
                    return true;
                }
            }
            return false;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            //draw car
            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateScale(SCALE));
            // draw car body
            spriteBatch.Draw(carTexture, carLocation, null, Color.White, carHeading, new Vector2(carTexture.Width / 2.0f, carTexture.Height / 2.0f), 1.0f, SpriteEffects.None, 1.0f);

            // calculate wheel locations
            Vector2 wheelOrigin = new Vector2(wheelTexture.Width / 2.0f, wheelTexture.Height / 2.0f);
            float a = wheelBase / 2.0f;
            float b = carSize.Y / 2.0f;
            float c = (float)Math.Sqrt(a * a + b * b);
            float angle = (float)Math.Atan(b/a);
            Vector2 wheelVector = c * new Vector2((float)Math.Cos(carHeading + angle), (float)Math.Sin(carHeading + angle));
            Vector2 wheelVector2 = c * new Vector2((float)Math.Cos(carHeading - angle), (float)Math.Sin(carHeading - angle));

            Vector2 frWheel = carLocation + wheelVector;
            Vector2 blWheel = carLocation - wheelVector;
            Vector2 flWheel = carLocation + wheelVector2;
            Vector2 brWheel = carLocation - wheelVector2;

            // draw axles
            spriteBatch.DrawLine(flWheel, frWheel, Color.Green, 2);
            spriteBatch.DrawLine(blWheel, brWheel, Color.Green, 2);

            //draw drivetrain
            spriteBatch.DrawLine((flWheel + frWheel) / 2, (blWheel + brWheel) / 2, Color.Aqua, 2);

            // draw wheels
            spriteBatch.Draw(wheelTexture, frWheel, null, Color.White, carHeading + steerAngle, wheelOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(wheelTexture, blWheel, null, Color.White, carHeading, wheelOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(wheelTexture, flWheel, null, Color.White, carHeading + steerAngle, wheelOrigin, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.Draw(wheelTexture, brWheel, null, Color.White, carHeading, wheelOrigin, 1.0f, SpriteEffects.None, 1.0f);

            // draw boundingbox
            //spriteBatch.DrawRectangle(new Rectangle((int)(carLocation.X - carSize.X / 2), (int)(carLocation.Y - carSize.X / 2), (int)carSize.X, (int)carSize.X), Color.White);
            spriteBatch.End();

            // draw car info
            spriteBatch.Begin();
            spriteBatch.DrawString(font, "Heading: " + carHeading, Vector2.Zero, Color.White);
            spriteBatch.DrawString(font, "Speed: " + carSpeed, new Vector2(0, 20), Color.White);
            spriteBatch.DrawString(font, "Steerangle: " + steerAngle, new Vector2(0, 40), Color.White);
            spriteBatch.DrawString(font, "Wheelbase: " + wheelBase, new Vector2(0, 60), Color.White);
            spriteBatch.End();
        }
        #endregion
    }
}
