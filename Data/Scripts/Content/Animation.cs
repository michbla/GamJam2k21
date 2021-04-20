using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Animation
    {
        private Texture spriteSheet;
        private readonly int FRAME_COUNT;
        private int currentFrame = 0;
        
        public Animation(Texture tex, int frames = 1)
        {
            spriteSheet = tex;
            FRAME_COUNT = frames;
        }
        
        public void Play(ref SpriteRenderer rend, ref GameObject parent, Vector2 viewPos, bool flipped = false)
        {
            if (flipped)
                rend.DrawSprite((currentFrame, 0), spriteSheet, viewPos, 
                                (parent.position.X + 1.0f, parent.position.Y), 
                                (parent.size.X * -1.0f, parent.size.Y), parent.rotation, parent.color);
            else
                rend.DrawSprite((currentFrame, 0), spriteSheet, viewPos, parent.position, parent.size, parent.rotation, parent.color);
        }
        
        public void NextFrame()
        {
            currentFrame = (currentFrame + 1) % FRAME_COUNT;
        }
    }
}
