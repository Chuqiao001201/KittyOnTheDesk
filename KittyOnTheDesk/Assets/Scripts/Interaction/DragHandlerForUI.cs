using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class DragHandlerForUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    // 拖拽时的偏移量，使拖拽点位于鼠标位置
    private Vector2 offset;
    
    // 原始位置，用于可能的复位操作
    private Vector2 originalPosition;
    
    // 引用RectTransform组件
    private RectTransform rectTransform;

    void Awake()
    {
        // 获取RectTransform组件
        rectTransform = GetComponent<RectTransform>();
        // 记录初始位置
        originalPosition = rectTransform.anchoredPosition;
    }

    // 开始拖拽时调用
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 计算鼠标位置与UI元素中心的偏移
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rectTransform, 
            eventData.position, 
            eventData.pressEventCamera, 
            out offset);
    }

    // 拖拽过程中调用
    public void OnDrag(PointerEventData eventData)
    {
        if (rectTransform == null) return;
        
        Vector2 localPointerPosition;
        
        // 将屏幕坐标转换为父对象的本地坐标
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)rectTransform.parent, 
            eventData.position, 
            eventData.pressEventCamera, 
            out localPointerPosition))
        {
            // 设置UI元素的位置，减去偏移量使拖拽更自然
            rectTransform.localPosition = localPointerPosition - offset;
        }
    }

    // 结束拖拽时调用
    public void OnEndDrag(PointerEventData eventData)
    {
        // 可以在这里添加拖拽结束后的逻辑
        // 例如：限制拖拽范围、吸附到特定位置等
    }
    
    // 可选：添加复位功能
    public void ResetPosition()
    {
        rectTransform.anchoredPosition = originalPosition;
    }

    public void DestroySelf()
    {
        Destroy(gameObject);
    }
}

