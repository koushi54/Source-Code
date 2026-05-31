using UnityEngine.InputSystem;

namespace Ko
{
    public static class SettingPanelInput
    {
        public static bool UpPressed() => Keyboard.current != null && Keyboard.current.upArrowKey.wasPressedThisFrame;
        public static bool DownPressed() => Keyboard.current != null && Keyboard.current.downArrowKey.wasPressedThisFrame;
        public static bool LeftPressed() => Keyboard.current != null && Keyboard.current.leftArrowKey.wasPressedThisFrame;
        public static bool RightPressed() => Keyboard.current != null && Keyboard.current.rightArrowKey.wasPressedThisFrame;
        public static bool EnterPressed() => Keyboard.current != null && Keyboard.current.enterKey.wasPressedThisFrame;
        public static bool BackspacePressed() => Keyboard.current != null && Keyboard.current.backspaceKey.wasPressedThisFrame;
    }
}
