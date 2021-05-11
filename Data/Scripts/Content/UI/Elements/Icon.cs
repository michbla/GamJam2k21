using OpenTK.Mathematics;

namespace GamJam2k21.Interface
{
   public class Icon : UI_Element
    {
        private Sprite visual;

        public Icon(Vector2 offset, Sprite visual) : base(offset)
        {
            this.visual = visual;
            collider = new BoxCollider(Transform.Default, visual.Size);
        }

        public override void Update(Vector2 mousePosition)
        {
            base.Update(mousePosition);
        }

        public override void Render(Vector2 position)
        {
            base.Render(position);
            Transform visualTransform = new Transform(position + Offset);
            visual.RenderWithTransform(visualTransform);
        }

        public void ChangeTexture(string newTextureName)
        {
            visual.Texture = ResourceManager.GetTexture(newTextureName);
        }
    }
}
