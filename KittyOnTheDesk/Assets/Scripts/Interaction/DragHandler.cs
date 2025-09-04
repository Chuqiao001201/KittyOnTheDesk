using System.Collections;
using System.Collections.Generic;
using DataSystem;
using UnityEngine;
using Utilities;

namespace Interaction
{
    [RequireComponent(typeof(Collider2D))] // 确保对象有碰撞体
    public class DragHandler : MonoBehaviour
    {
        // 是否正在拖拽
        private bool isDragging = false;
    
        // 拖拽时与相机的距离
        private float distance = 10f;
    
        // 主相机引用
        private Camera mainCamera;

        [SerializeField] private GameObject functionPanel;
        [SerializeField] private float duration;
        private Coroutine coroutine;

        [SerializeField] private GameObject feedSystem;

        private void Start()
        {
            // 获取主相机
            mainCamera = Camera.main;
        
            // 确保对象有碰撞体
            if (!GetComponent<Collider2D>())
            {
                Debug.LogError("拖拽对象需要添加碰撞体组件！");
            }

            transform.position = DataManager.Instance.saveData.mainViewPos;
        }

        private void OnMouseOver()
        {
            // 检测左键点击
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("左键点击");
                // 计算物体与相机的距离
                distance = Vector3.Distance(transform.position, mainCamera.transform.position);
                isDragging = true;
            }
            // 检测右键点击
            else if (Input.GetMouseButtonDown(1))
            {
                Debug.Log("右键点击");
                // 右键点击逻辑
                if (!functionPanel.activeInHierarchy)
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                        coroutine = null;
                    }
                    coroutine = StartCoroutine(PanelDisplayCoroutine());
                }
                else
                {
                    if (coroutine != null)
                    {
                        StopCoroutine(coroutine);
                        coroutine = null;
                    }
                    functionPanel.SetActive(false);
                }
            }
        }

        private void OnMouseDrag()
        {
            if (!isDragging || !mainCamera) return;
            // 创建从鼠标位置发出的射线
            var ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            
            // 计算射线与物体拖拽平面的交点
            var plane = new Plane(mainCamera.transform.forward, transform.position);

            if (plane.Raycast(ray, out var enter))
            {
                // 设置物体新位置
                transform.position = ray.GetPoint(enter);
            }
        }

        private void OnMouseUp()
        {
            // 结束拖拽
            isDragging = false;
            DataManager.Instance.saveData.mainViewPos = transform.position;
            JsonDataTool.SaveData(DataManager.Instance.saveData);
        }

        // 可选：限制拖拽在特定轴上
        public bool lockX = false;
        public bool lockY = false;
        public bool lockZ = false;

        private void LateUpdate()
        {
            if (!isDragging) return;
            // 应用轴锁定
            var pos = transform.position;
            if (lockX) pos.x = 0;
            if (lockY) pos.y = 0;
            if (lockZ) pos.z = 0;
            transform.position = pos;
        }

        private IEnumerator PanelDisplayCoroutine()
        {
            functionPanel.SetActive(true);
            yield return new WaitForSeconds(duration);
            functionPanel.SetActive(false);
        }

        public void FeedSystemControl()
        {
            feedSystem.SetActive(!feedSystem.activeInHierarchy);
        }
    }
}

