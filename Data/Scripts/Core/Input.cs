using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Mathematics;

namespace GamJam2k21
{
    public class Input
    {
        private static MouseState mouseState;
        private static KeyboardState keyboardState;

        public static bool IsClickingAButton = false;

        public static MouseState MouseState
        {
            get => mouseState;
        }

        public static KeyboardState KeyboardState
        {
            get => keyboardState;
        }

        public static Vector2 CursorPosition
        {
            get => mouseState.Position;
        }

        #region Singleton
        private Input() { }
        private static Input instance;
        public static Input GetInstance()
        {
            if (instance == null)
                instance = new Input();
            return instance;
        }
        #endregion

        public static void SetInputs(MouseState _mouseState,
                                     KeyboardState _keyboardState)
        {
            mouseState = _mouseState;
            keyboardState = _keyboardState;
        }

        public static bool IsAnyKeyDown()
        {
            return keyboardState.IsAnyKeyDown;
        }

        public static bool IsKeyDown(Keys key)
        {
            return keyboardState.IsKeyDown(key);
        }

        public static bool IsKeyPressed(Keys key)
        {
            return keyboardState.IsKeyPressed(key);
        }

        public static bool IsKeyReleased(Keys key)
        {
            return keyboardState.IsKeyReleased(key);
        }

        public static bool IsMouseButtonDown(MouseButton button)
        {
            return mouseState.IsButtonDown(button);
        }

        public static bool IsAnyMouseButtonDown()
        {
            return mouseState.IsAnyButtonDown;
        }
    }
}
