using System.Collections.Generic;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Animator
    {
        private readonly Dictionary<string, Animation> ANIMATIONS;

        public readonly Transform PARENT;
        private readonly float FRAME_RATE;

        private bool isFlipped = false;
        private float timeSum = 0.0f;

        public bool IsFlipped
        {
            get => isFlipped;
            set { isFlipped = value; }
        }

        public Animator(Transform parent, float frameRate)
        {
            PARENT = parent;
            FRAME_RATE = frameRate;
            ANIMATIONS = new Dictionary<string, Animation>();
        }
        
        public void Update(string state, float animSpeed = 1.0f)
        {
            timeSum += Time.DeltaTime * animSpeed;
            if (timeSum >= 1.0f / FRAME_RATE)
            {
                ANIMATIONS[state].NextFrame();
                timeSum = 0.0f;
            }
        }
        
        public void Render(string state)
        {
            ANIMATIONS[state].Play(PARENT, isFlipped);
        }
        
        public void AddAnimation(string name, Sprite spriteSheet, int frameCount = 1)
        {
            ANIMATIONS.Add(name, new Animation(spriteSheet, frameCount));
        }
    }
}
