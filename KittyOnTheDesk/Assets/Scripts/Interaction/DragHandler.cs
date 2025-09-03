using UnityEngine;

[RequireComponent(typeof(Collider2D))] // 确保对象有碰撞体
public class DragHandler : MonoBehaviour
{
    // 是否正在拖拽
    private bool isDragging = false;
    
    // 拖拽时与相机的距离
    private float distance = 10f;
    
    // 主相机引用
    private Camera mainCamera;

    void Start()
    {
        // 获取主相机
        mainCamera = Camera.main;
        
        // 确保对象有碰撞体
        if (GetComponent<Collider2D>() == null)
        {
            Debug.LogError("拖拽对象需要添加碰撞体组件！");
        }
    }

    void OnMouseDown()
    {
        // 计算物体与相机的距离
        distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        isDragging = true;
    }

    void OnMouseDrag()
    {
        if (isDragging && mainCamera != null)
        {
            // 创建从鼠标位置发出的射线
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            // 计算射线与物体拖拽平面的交点
            Plane plane = new Plane(mainCamera.transform.forward, transform.position);
            float enter;
            
            if (plane.Raycast(ray, out enter))
            {
                // 设置物体新位置
                transform.position = ray.GetPoint(enter);
            }
        }
    }

    void OnMouseUp()
    {
        // 结束拖拽
        isDragging = false;
    }

    // 可选：限制拖拽在特定轴上
    public bool lockX = false;
    public bool lockY = false;
    public bool lockZ = false;

    void LateUpdate()
    {
        if (isDragging)
        {
            // 应用轴锁定
            Vector3 pos = transform.position;
            if (lockX) pos.x = 0;
            if (lockY) pos.y = 0;
            if (lockZ) pos.z = 0;
            transform.position = pos;
        }
    }
}

