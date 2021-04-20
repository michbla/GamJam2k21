using System.Collections.Generic;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Animator
    {
        private Dictionary<string, Animation> animations;

        public GameObject parent;
        private SpriteRenderer renderer;

        private Vector2i sheetSize = (1, 1);

        public bool isFlipped = false;
        private float timeSum = 0.0f;
        private readonly float FRAME_RATE;

        public Animator(GameObject p, Shader sha, Vector2i size, float rate)
        {
            parent = p;
            sheetSize = size;
            renderer = new SpriteRenderer(sha, sheetSize);
            FRAME_RATE = rate;

            animations = new Dictionary<string, Animation>();
        }
        
        public virtual void Update(string state, float deltaTime, float animSpeed = 1.0f)
        {
            timeSum += deltaTime * animSpeed;
            if (timeSum >= 1.0f / FRAME_RATE)
            {
                animations[state].NextFrame();
                timeSum = 0.0f;
            }
        }
        
        public virtual void Draw(string state, Vector2 viewPos)
        {
            animations[state].Play(ref renderer, ref parent, viewPos, isFlipped);
        }
        
        public void AddAnimation(string name, Texture tex, int frames)
        {
            animations.Add(name, new Animation(tex, frames));
        }
    }
}
