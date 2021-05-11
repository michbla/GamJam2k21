using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GamJam2k21.Interface
{
    public abstract class UI_Element
    {
        private Vector2 offset;

        protected BoxCollider collider;
        protected Vector2 colliderPosition;
        private bool isHoveredOver;
        private bool wasHoveredOver;
        private bool isClicked;
        private bool wasClicked;

        public Vector2 Offset { get => offset; }
        public bool IsHoverOver { get => isHoveredOver; }
        public bool IsClicked { get => isClicked; }

        public UI_Element(Vector2 offset = default)
        {
            isHoveredOver = false;
            wasHoveredOver = false;
            isClicked = false;
            wasClicked = false;
            this.offset = offset;
            collider = new BoxCollider(Transform.Default, Vector2.One);
        }

        public virtual void Update(Vector2 mousePosition)
        {
            collider.SetPosition(colliderPosition);
            collider.Update();
            isHoveredOver = hasCollisionWithMouse(mousePosition);
            isClicked = isHoveredOver && Input.IsMouseButtonDown(MouseButton.Button1);

            if (isClicked)
                OnClick();
            else if (wasClicked)
                OnUnClick();

            if (isHoveredOver)
                OnHoverOver();
            else if (wasHoveredOver)
                OnUnhoverOver();

        }

        private bool hasCollisionWithMouse(Vector2 mousePosition)
        {
            Transform mouseTransform = Transform.Default;
            mouseTransform.Position = mousePosition - (0.05f, 0.05f);
            BoxCollider mouseCollider = new BoxCollider(mouseTransform, (0.1f, 0.1f));
            (bool, Direction, Vector2) result;
            result = collider.CheckCollision(mouseCollider);
            if (result.Item1)
                return true;
            return false;
        }

        public virtual void Render(Vector2 position) {
            colliderPosition = position + Offset;
        }

        public virtual void OnHoverOver()
        {
            wasHoveredOver = true;
        }

        public virtual void OnUnhoverOver()
        {
            wasHoveredOver = false;
        }

        public virtual void OnClick()
        {
            wasClicked = true;
        }

        public virtual void OnUnClick()
        {
            wasClicked = false;
        }
    }
}
