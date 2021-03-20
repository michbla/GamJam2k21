using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Animation
    {

        private Texture sheet;

        private int frameCount;

        private int currentFrame = 0;

        public Animation(Texture tex, int frames)
        {
            sheet = tex;
            frameCount = frames;
        }

        public void Play(ref SpriteRenderer rend, ref GameObject obj, Vector2 viewPos, bool flipped)
        {
            if (flipped)
            {
                rend.DrawSprite((currentFrame, 0), sheet, viewPos, (obj.position.X + 1.0f, obj.position.Y), (obj.size.X * -1.0f, obj.size.Y), obj.rotation, obj.color);
            }
            else
            {
                rend.DrawSprite((currentFrame, 0), sheet, viewPos, obj.position, obj.size, obj.rotation, obj.color);
            }
        }

        public void NextFrame()
        {
            currentFrame = (currentFrame + 1) % frameCount;
        }
    }
}
