using System;
using UnityEditor;
using UnityEngine;

namespace Interaction
{
    public class CursorController : MonoBehaviour
    {
        // 在Inspector中拖拽你的光标图像到这里
        public Texture2D cursorOpenHand;
        public Texture2D cursorCloseHand;
        // 光标热点位置（通常是光标的尖端）
        public Vector2 hotSpot = Vector2.zero;
        private bool canSetCursor;

        private void Update()
        {
            if (!canSetCursor) return;
            if (Input.GetMouseButton(0)) SetCustomCursor(1);
            if (Input.GetMouseButtonUp(0)) SetCustomCursor(0);
            if (Input.GetMouseButtonUp(1)) ResetCursor();
        }

        public void SetCustomCursor(int index)
        {
            canSetCursor = true;
            var cursorTexture = index == (int)CursorState.OpenHand ? cursorOpenHand : cursorCloseHand;
            // 设置自定义光标
            Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
        }

        // 可选：如果需要在某些条件下恢复默认光标
        public void ResetCursor()
        {
            canSetCursor = false;
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
    }

    public enum CursorState
    {
        OpenHand, CloseHand
    }
}
