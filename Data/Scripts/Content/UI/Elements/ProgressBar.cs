using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Text;

namespace GamJam2k21.Interface
{
    public class ProgressBar : UI_Element
    {
        private static Sprite background;
        private static Sprite fill;
        private static Sprite border;

        private float valueMax;
        public float value;

        public ProgressBar(Vector2 offset, float valueMax) : base(offset)
        {
            this.valueMax = valueMax;
            value = valueMax * 0.5f;
            setSprites();
            collider = new BoxCollider(Transform.Default, ((int)valueMax * 0.0625f, 1.0f));
        }

        private void setSprites()
        {
            if (background != null && fill != null && border != null)
                return;
            background = Sprite.Single(ResourceManager.GetTexture("progressBar_back"), (0.0625f, 1.0f));
            fill = Sprite.Single(ResourceManager.GetTexture("progressBar_fill"), (0.0625f, 1.0f));
            border = Sprite.Single(ResourceManager.GetTexture("progressBar_border"), (0.0625f, 1.0f));
        }

        public override void Update(Vector2 mousePosition)
        {
            base.Update(mousePosition);
        }

        public void SetValue(float newValue)
        {
            if (newValue <= 0.0f)
                value = 0.0f;
            else if (newValue >= valueMax)
                value = valueMax;
            else
                value = newValue;
        }

        public void Colorize(Vector3 color)
        {
            fill.Color = color;
        }

        public override void Render(Vector2 position)
        {
            base.Render(position);
            int barLength = (int)(valueMax / 0.0625f);
            int barFill = (int)(value / 0.0625f);
            Transform barTrasform = new Transform(position + Offset - (valueMax * 0.5f, 0.0f));
            Transform borderTransform = new Transform(barTrasform.Position - (0.0625f, 0.0f));
            border.RenderWithTransform(borderTransform);
            for (int i = 0; i < barLength; i++)
            {
                background.RenderWithTransform(barTrasform);
                if (i < barFill)
                    fill.RenderWithTransform(barTrasform);
                barTrasform.Position += (0.0625f, 0.0f);
            }
            borderTransform.Position = barTrasform.Position;
            border.RenderWithTransform(borderTransform);
        }

        public override void OnHoverOver()
        {
            base.OnHoverOver();
        }
    }
}
