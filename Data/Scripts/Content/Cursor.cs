using OpenTK.Mathematics;
using System;

namespace GamJam2k21
{
    public class Cursor
    {
        private GameObject cursorObject;

        private Vector2 onScreenPosition;
        private Vector2 inWorldPosition;
        private Vector2i onGridPosition;

        private float cursorScale = 0.0f;
        private float cursorSize = 1.0f;
        private float cursorRotation = 0.0f;

        private bool displayPickaxe = false;

        public Vector2 OnScreenPos { get => onScreenPosition; }
        public Vector2 InWorldPos { get => inWorldPosition; }
        public Vector2i OnGridPos { get => onGridPosition; }
        public bool DisplayPickaxe { set { displayPickaxe = value; } }


        public Cursor()
        {
            cursorObject = new GameObject(
                Sprite.Single(ResourceManager.GetTexture("cursor"), Vector2.One),
                Transform.Default);
        }

        public void Update()
        {
            cursorObject.Position = calculateMousePositions();

            cursorScale += 3 * Time.DeltaTime;
            cursorSize = 1.0f + (float)Math.Sin(cursorScale) / 50.0f;
            if (displayPickaxe)
            {
                ChangeTexture("cursorPick");
                cursorRotation = 0.0f - (float)Math.Sin(cursorScale) * 5.0f;
            }
            else
            {
                ChangeTexture("cursor");
                cursorRotation = 0;
            }

            cursorObject.Scale = (cursorSize,cursorSize);
            cursorObject.Rotation = cursorRotation;
        }

        private Vector2 calculateMousePositions()
        {
            Vector2 windowSize = Camera.WindowResolution;
            Vector2 cursorPosition = Input.CursorPosition;
            float mouseScale = Camera.ScreenSize.X / windowSize.X;
            onScreenPosition.X = cursorPosition.X * mouseScale;
            onScreenPosition.Y = -(cursorPosition.Y - windowSize.Y) * mouseScale;
            inWorldPosition =
                onScreenPosition +
                Camera.Position -
                (Camera.ScreenSize.X * 0.5f, Camera.ScreenSize.Y * 0.5f);
            onGridPosition = ((int)Math.Floor(inWorldPosition.X),
                              (int)Math.Floor(inWorldPosition.Y));
            Vector2 newMousePos = Camera.Position
                                  - (0.0f, 1.0f)
                                  + onScreenPosition
                                  - Camera.ScreenSize * 0.5f;
            return newMousePos;
        }

        public void Render()
        {
            cursorObject.Render();
        }

        public void ChangeTexture(string newTextureName)
        {
            cursorObject.Texture = ResourceManager.GetTexture(newTextureName);
        }
    }
}
