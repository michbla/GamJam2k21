using OpenTK.Mathematics;
using System;

namespace GamJam2k21.Interface
{
    public class Text : UI_Element
    {
        private static TextRenderer renderer = new TextRenderer();

        private String text;
        private TextType type;
        private float scale;

        public Text(Vector2 offset,
                    String text = "",
                    TextType type = TextType.normal,
                    float scale = 1.0f) 
            : base(offset)
        {
            this.text = text;
            this.type = type;
            this.scale = scale;
            collider = new BoxCollider(Transform.Default,
                                       (text.Length * scale, scale));
        }

        public override void Update(Vector2 mousePosition)
        {
            base.Update(mousePosition);
        }

        public override void Render(Vector2 position)
        {
            base.Render(position);
            renderer.PrintText(text, type, position + Offset, scale);
        }
    }
}
