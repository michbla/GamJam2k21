using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Animation
    {
        private Sprite spriteSheet;
        private readonly int FRAME_COUNT;
        private int currentFrame = 0;
        
        public Animation(Sprite spriteSheet, int frameCount = 1)
        {
            this.spriteSheet = spriteSheet;
            FRAME_COUNT = frameCount;
        }
        
        public void Play(Transform parent, bool flipped = false)
        {
            spriteSheet.RenderWithTransform(parent,
                                            (currentFrame, 0),
                                            flipped);
        }
        
        public void NextFrame()
        {
            currentFrame = (currentFrame + 1) % FRAME_COUNT;
        }
    }
}
