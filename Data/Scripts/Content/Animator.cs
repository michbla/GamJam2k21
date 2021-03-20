using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Animator
    {
        private Dictionary<string, Animation> animations;

        public GameObject parent;

        private SpriteRenderer renderer;

        private Vector2i sheetSize;

        public bool isFlipped = false;

        private float timeSum = 0.0f;

        private float frameRate;

        public Animator(GameObject p, Shader sha, Vector2i size, float rate)
        {
            parent = p;
            sheetSize = size;
            renderer = new SpriteRenderer(sha, sheetSize);
            frameRate = rate;

            animations = new Dictionary<string, Animation>();
        }

        public void Update(string state, float deltaTime)
        {
            timeSum += deltaTime;
            if (timeSum >= 1.0f / frameRate)
            {
                animations[state].NextFrame();
                timeSum = 0.0f;
            }
        }

        public void Draw(string state, Vector2 viewPos)
        {
                animations[state].Play(ref renderer, ref parent, viewPos, isFlipped);
        }

        public void AddAnimation(string name, Animation anim)
        {
            animations.Add(name, anim);
        }
    }
}
