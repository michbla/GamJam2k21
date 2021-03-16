using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace GamJam
{
    public class BallObject : GameObject
    {
        //Promien
        public float radius;
        //Czy przyczepiona do gracza
        public bool stuck;

        public BallObject()
        {
            radius = 12.5f;
            stuck = true;
        }
        public BallObject(Vector3 pos, float r, Vector3 vel, Texture sprite) : base(pos, (r * 2.0f, r * 2f), sprite)
        {
            radius = r;
            Velocity = vel;
            stuck = true;
        }
        //Ruch pilki
        public Vector3 Move(float dt, int windowWidth, int windowHeight)
        {
            if (!stuck)
            {
                Position += Velocity * dt;
                if (Position.X <= -windowWidth / 2)//uderzenie w lewa krawedz
                {
                    Velocity.X *= -1;
                    Position.X = -windowWidth / 2;
                }
                else if (Position.X + Size.X >= windowWidth / 2)//uderzenie w prawa krawedz
                {
                    Velocity.X *= -1;
                    Position.X = windowWidth / 2 - Size.X;
                }
                if (Position.Y + Size.Y >= windowHeight / 2)//uderzenie w gorna krawedz
                {
                    Velocity.Y *= -1;
                    Position.Y = windowHeight / 2 - Size.Y;
                }
            }
            //Obracanie w kierunku ruchu
            double tan = MathHelper.Atan2(Velocity.X, Velocity.Y);
            Rotation = (float)MathHelper.RadiansToDegrees(tan);
            return Position;
        }
        //Resetowanie pozycji i predkosci
        public void Reset(Vector3 pos, Vector3 vel)
        {
            Position = pos;
            Velocity = vel;
            stuck = true;
        }
    }
}
