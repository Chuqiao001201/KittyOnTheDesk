using UnityEngine;

namespace Interaction
{
    public class CharacterInteraction : MonoBehaviour
    {
        private void Start()
        {
            // 确保对象有碰撞体
            if (GetComponent<Collider2D>() == null)
            {
                Debug.LogError("拖拽对象需要添加碰撞体组件！");
            }
        }

        private void OnMouseOver()
        {
            if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("右键点击");
                // 右键点击逻辑
            }
        }
    }
}
