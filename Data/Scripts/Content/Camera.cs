using System;
using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace GamJam2k21
{
    public class Camera
    {
        private static Vector2 position;
        private static Matrix4 projection;

        private static float FOLLOW_SPEED = 5.0f;

        private static Transform target = null;

        private static Vector2 viewBounds;
        private static Vector2 screenSize;
        private static Vector2 windowResolution;

        private static bool lookAroundEnabled = true;

        private static GameObject background;

        #region Singleton
        private Camera() { }

        private static Camera instance;
        public static Camera GetInstance()
        {
            if (instance == null)
                instance = new Camera();
            return instance;
        }
        #endregion

        public static Vector2 Position
        {
            get => position;
            set { position = value; }
        }
        public static Vector2 ScreenSize
        {
            get => screenSize;
            set { screenSize = value; }
        }
        public static Vector2 WindowResolution
        {
            get => windowResolution;
            set { windowResolution = value; }
        }
        public static bool CanLookAround { set { lookAroundEnabled = value; } }

        public static void Update()
        {
            followTarget();
            if (background != null)
                background.Position = GetLeftLowerCorner();
        }

        public static void Initiate()
        {
            GetInstance();
            screenSize = (24.0f, 13.5f);
            position = (0.0f, 0.0f);
            projection = Matrix4.CreateOrthographic(screenSize.X,
                                                    screenSize.Y,
                                                    -1.0f,
                                                    1.0f);
            viewBounds = (0.0f + screenSize.X * 0.5f,
                          128.0f - screenSize.X * 0.5f);
        }
        public static void RenderBackground()
        {
            if (background == null)
                makeBackground();
            background.Render();
        }

        private static void makeBackground()
        {
            Sprite backgroundSprite = Sprite.Single(
                ResourceManager.GetTexture("sky"),
                screenSize);
            background = new GameObject(
                backgroundSprite,
                Transform.Default);
        }


        public static Vector2 GetScreenCenter(Vector2 offset = default)
        {
            return new Vector2(position.X + offset.X, position.Y + offset.Y);
        }

        public static Vector2 GetLeftLowerCorner(Vector2 offset = default)
        {
            return new Vector2(position.X - screenSize.X * 0.5f + offset.X,
                               position.Y - screenSize.Y * 0.5f + offset.Y);
        }

        public static Vector2 GetLeftUpperCorner(Vector2 offset = default)
        {
            return new Vector2(position.X - screenSize.X * 0.5f + offset.X,
                               position.Y + screenSize.Y * 0.5f + offset.Y);
        }

        public static Vector2 GetRightLowerCorner(Vector2 offset = default)
        {
            return new Vector2(position.X + screenSize.X * 0.5f + offset.X,
                               position.Y - screenSize.Y * 0.5f + offset.Y);
        }

        public static Vector2 GetRightUpperCorner(Vector2 offset = default)
        {
            return new Vector2(position.X + screenSize.X * 0.5f + offset.X,
                               position.Y + screenSize.Y * 0.5f + offset.Y);
        }

        public static void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }

        private static void followTarget()
        {
            Vector2 desiredView;

            desiredView.X = MathHelper.Lerp(
                position.X,
                target.Position.X,
                Time.DeltaTime * FOLLOW_SPEED);
            desiredView.Y = MathHelper.Lerp(
                position.Y,
                target.Position.Y,
                Time.DeltaTime * FOLLOW_SPEED);
            if (lookAroundEnabled)
                desiredView = LookAroundWithKeys(desiredView);
            position.Y = desiredView.Y;
            position.X = Math.Clamp(desiredView.X,
                                    viewBounds.X,
                                    viewBounds.Y);
        }

        public static Vector2 LookAroundWithKeys(Vector2 desiredView)
        {
            if (Input.IsKeyDown(Keys.S))
                desiredView.Y -= Time.DeltaTime * 10;
            if (Input.IsKeyDown(Keys.W))
                desiredView.Y += Time.DeltaTime * 10;
            if (Input.IsKeyDown(Keys.A))
                desiredView.X -= Time.DeltaTime * 10;
            if (Input.IsKeyDown(Keys.D))
                desiredView.X += Time.DeltaTime * 10;
            return desiredView;
        }
        public static Matrix4 GetProjection()
        {
            return projection;
        }

        public static void SetProjection()
        {
            projection = Matrix4.CreateOrthographic(
                screenSize.X,
                screenSize.Y,
                -1.0f,
                1.0f);
        }
    }
}
