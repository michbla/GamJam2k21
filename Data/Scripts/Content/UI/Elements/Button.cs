using OpenTK.Mathematics;
using System.Collections.Generic;

namespace GamJam2k21.Interface
{
    public enum ButtonState
    {
        normal,
        hovered,
        clicked
    }
    public class Button : UI_Element
    {
        private static Dictionary<Vector2i, Sprite> buttons
            = new Dictionary<Vector2i, Sprite>();

        private Vector2i size;
        private Vector2i objSize;

        private Text text;

        private ButtonState state;

        private bool performedAction;

        public Button(Vector2 offset,
                      Vector2i size,
                      string text = "",
                      TextType type = TextType.bold)
            : base(offset)
        {
            objSize = (size.X, size.Y % 10);
            this.size = size;
            makeButtonOfSize(size);
            collider = new BoxCollider(Transform.Default, objSize);

            state = ButtonState.normal;
            performedAction = false;

            if (text.Length >= 1)
            {
                float charSize = (objSize.X - 0.1f) / text.Length;
                if (charSize >= objSize.Y)
                    charSize = objSize.Y - 0.05f;
                Vector2 textOffset = calculateTextOffset(text, objSize, charSize);
                this.text = new Text(textOffset, text, type, charSize);
            }
        }

        private Vector2 calculateTextOffset(string text, Vector2i size, float charSize)
        {
            float offsetX = size.X * 0.5f - (text.Length * 0.5f) * charSize;
            float offsetY = (size.Y - charSize) * 0.55f;
            return (offsetX, offsetY);
        }

        private void makeButtonOfSize(Vector2i size)
        {
            if (!buttons.ContainsKey(size))
            {
                string buttonSize = size.X + "x" + size.Y;
                Texture buttonTexture = ResourceManager.GetTexture("button_" + buttonSize);
                Sprite buttonSprite = Sprite.Sheet(buttonTexture, (size.X,size.Y % 10), (1, 3));

                buttons.Add(size, buttonSprite);
            }
        }

        private Sprite getSprite()
        {
            return buttons[size];
        }

        public override void Update(Vector2 mousePosition)
        {
            base.Update(mousePosition);
        }

        public override void Render(Vector2 position)
        {
            base.Render(position);
            int yOffset = 0;
            switch (state)
            {
                case ButtonState.hovered:
                    yOffset = 1;
                    break;
                case ButtonState.clicked:
                    yOffset = 2;
                    break;
            }
            getSprite().RenderWithTransform(new Transform(position + Offset),
                                            (0, yOffset));
            if (state == ButtonState.clicked)
                text.Render(position + Offset - (0.0f, 0.1f));
            else
                text.Render(position + Offset);
        }

        public void UpdateText(string newText)
        {
            text.UpdateText(newText);
        }

        public override void OnHoverOver()
        {
            base.OnHoverOver();
            state = ButtonState.hovered;
            if (IsClicked)
                state = ButtonState.clicked;
        }

        public override void OnUnhoverOver()
        {
            base.OnUnhoverOver();
            state = IsClicked ? ButtonState.clicked : ButtonState.normal;
        }

        public override void OnUnClick()
        {
            base.OnUnClick();
            reset();
        }

        public bool CanPerformAction()
        {
            if (Input.IsClickingAButton)
                return false;
            if (IsClicked && !performedAction)
            {
                Input.IsClickingAButton = true;
                performedAction = true;
                return true;
            }
            return false;
        }

        private void reset()
        {
            performedAction = false;
        }
    }
}
