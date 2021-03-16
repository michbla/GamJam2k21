using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace GamJam
{
    /// <summary>
    /// Klasa odpowiadajaca za wszelkie obiekty bedace czescia swiata gry
    /// </summary>
    public class GameObject
    {
        //Wlasciwosci
        public Vector3 Position;
        public Vector2 Size;
        public Vector3 Velocity;

        public Vector3 Color;

        public float Rotation;
        //Flagi
        public bool IsSolid;
        public bool Destroyed;

        //Tekstura
        Texture Sprite;

        //Konstruktor bezparametrowy
        //Ustawia wartosci domyslne
        public GameObject() : this((0.0f, 0.0f,0.0f), (1.0f, 1.0f), null, (1.0f, 1.0f, 1.0f), (0.0f, 0.0f,0.0f)) { }
        //Konstruktor zawierajacy jedynie pozycje, skale i teksture
        public GameObject(Vector3 pos, Vector2 size, Texture sprite) : this(pos, size, sprite, (1.0f, 1.0f, 1.0f), (0.0f, 0.0f, 0.0f)) { }
        //Konstruktor zawierajacy pozycje, skale, teksture i kolor
        public GameObject(Vector3 pos, Vector2 size, Texture sprite, Vector3 col) : this(pos, size, sprite, col, (0.0f, 0.0f, 0.0f)) { }
        //Pelny konstruktor
        public GameObject(Vector3 pos, Vector2 size, Texture sprite, Vector3 col, Vector3 vel) {
            Position = pos;
            Size = size;
            Sprite = sprite;
            Color = col;
            Velocity = vel;
            IsSolid = false;
            Destroyed = false;
        }
        //Wirtualna funkcja rysujaca
        public virtual void Draw(SpriteRenderer rend) {
            rend.DrawSprite(Sprite, Position, Size, Rotation, Color);
        }

    }
}
