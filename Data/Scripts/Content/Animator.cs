using System.Collections.Generic;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Animator
    {
        //Slownik animacji
        private Dictionary<string, Animation> animations;
        //Rodzic do pobierania pozycji,rozmiaru i rotacji
        public GameObject parent;
        //Renderer unikalny dla kazdego animatora
        //z uwagi na to, ze jest zrobiony pod dany rozmiar
        //SpriteSheeta
        private SpriteRenderer renderer;
        //Rozmiary sprite sheeta, do animacji
        //lepiej, zeby byl poziomy
        //i pod taka wersje jest zaimplementowana reszta
        private Vector2i sheetSize;
        //Flaga odwrocenia sprite'a
        public bool isFlipped = false;
        //Sumowanie uplywu czasu do zmiany klatek
        private float timeSum = 0.0f;
        //Klatki na sekunde
        private float frameRate;
        //Konstruktor
        public Animator(GameObject p, Shader sha, Vector2i size, float rate)
        {
            parent = p;
            sheetSize = size;
            renderer = new SpriteRenderer(sha, sheetSize);
            frameRate = rate;

            animations = new Dictionary<string, Animation>();
        }
        //Update logiki
        public virtual void Update(string state, float deltaTime)
        {
            timeSum += deltaTime;
            if (timeSum >= 1.0f / frameRate)
            {
                animations[state].NextFrame();
                timeSum = 0.0f;
            }
        }
        //Rysowanie
        public virtual void Draw(string state, Vector2 viewPos)
        {
                animations[state].Play(ref renderer, ref parent, viewPos, isFlipped);
        }
        //Dodawanie animacji do slownika
        public void AddAnimation(string name, Texture tex, int frames)
        {
            animations.Add(name, new Animation(tex,frames));
        }
    }
}
