using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    /// <summary>
    /// Klasa odpowiadajaca za wszelkie obiekty bedace czescia swiata gry
    /// </summary>
    public class GameObject
    {
        //Wlasciwosci
        public Vector2 position;
        public Vector2 size;
        public Vector2 velocity;

        public Vector3 color;

        public float rotation;
        //Flagi
        public bool isSolid;
        public bool isDestroyed;

        //Tekstura
        Texture sprite;

        //Konstruktor bezparametrowy
        //Ustawia wartosci domyslne
        public GameObject() : this((0.0f, 0.0f), (1.0f, 1.0f), null, (1.0f, 1.0f, 1.0f), (0.0f, 0.0f)) { }
        //Konstruktor zawierajacy jedynie pozycje, skale i teksture
        public GameObject(Vector2 pos, Vector2 size, Texture sprite) : this(pos, size, sprite, (1.0f, 1.0f, 1.0f), (0.0f, 0.0f)) { }
        //Konstruktor zawierajacy pozycje, skale, teksture i kolor
        public GameObject(Vector2 pos, Vector2 size, Texture sprite, Vector3 col) : this(pos, size, sprite, col, (0.0f, 0.0f)) { }
        //Pelny konstruktor
        public GameObject(Vector2 pos, Vector2 size, Texture sprite, Vector3 col, Vector2 vel)
        {
            this.position = pos;
            this.size = size;
            this.sprite = sprite;
            this.color = col;
            this.velocity = vel;
            isSolid = false;
            isDestroyed = false;
        }
        //Wirtualna funkcja rysujaca
        public virtual void Draw(SpriteRenderer rend, Vector2 viewPos)
        {
            //OPTYMALIZACJA - nie renderuj obiektow poza ekranem
            if (position.Y < -viewPos.Y -4.5f - size.Y || position.Y > -viewPos.Y + 4.5f)
                return;
            rend.DrawSprite(sprite, viewPos, position, size, rotation, color);
        }
        //Metoda obslugujaca logike
        public virtual void Update(KeyboardState input, float deltaTime)
        {

        }
    }
}
