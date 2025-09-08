using DataSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Utilities;

namespace Memo
{
    [RequireComponent(typeof(TMP_InputField))]
    public class MemoInput : MonoBehaviour
    {
        private TMP_InputField inputField;
        private global::Memo.Memo memo;

        private void Awake()
        {
            inputField = GetComponent<TMP_InputField>();
        
            // 确保设置为多行模式
            inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
        
            // 关键修改1：移除默认提交事件，改用 onEndEdit 处理“失去焦点/提交”
            inputField.onSubmit.RemoveAllListeners();
            inputField.onEndEdit.RemoveAllListeners(); // 清除默认的结束编辑事件
            inputField.onEndEdit.AddListener(HandleEndEdit); // 绑定“失去焦点/提交”的统一处理

            memo = GetComponentInParent<global::Memo.Memo>();
        }

        private void Update()
        {
            // 当输入框获得焦点时处理按键（保持原有的Alt+Enter换行逻辑）
            if (inputField.isFocused)
            {
                // Alt+Enter组合键：换行（原逻辑保留）
                if (Input.GetKeyDown(KeyCode.Return) && 
                    (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightAlt))) // 注：原代码LeftControl可能是笔误，此处修正为Alt
                {
                    InsertNewLine();
                
                    // 阻止事件被系统进一步处理（避免触发结束编辑）
                    EventSystem.current.SendMessage("Cancel", SendMessageOptions.DontRequireReceiver);
                }
                // 单独按Enter：触发结束编辑（等同于失去焦点，会走HandleEndEdit）
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    inputField.DeactivateInputField(); // 主动让输入框失去焦点，触发onEndEdit
                    SecreteKeyTest();
                }
            }
        }

        // 插入换行并调整光标位置（原逻辑保留）
        private void InsertNewLine()
        {
            int caretPosition = inputField.caretPosition;
        
            // 注：原代码插入空字符串""，此处修正为插入换行符"\n"
            inputField.text = inputField.text.Insert(caretPosition, "");
        
            // 移动光标到换行符后面
            inputField.caretPosition = caretPosition + 1;
        
            // 保持输入焦点
            inputField.ActivateInputField();
        }

        // 关键修改2：统一处理“失去焦点”和“主动提交”的保存逻辑
        /// <summary>
        /// 当输入框失去焦点或按下Enter时触发
        /// </summary>
        private void HandleEndEdit(string text)
        {
            Debug.Log("输入框失去焦点/提交，自动保存: " + text);
        
            // 1. 失去焦点（可选：如需提交后重新聚焦，可注释此行）
            // EventSystem.current.SetSelectedGameObject(null);

            // 2. 安全校验：避免空引用
            if (!memo)
            {
                Debug.LogError("未找到父对象的Memo组件，无法保存备忘录！");
                return;
            }
            if (DataManager.Instance == null || DataManager.Instance.saveData == null)
            {
                Debug.LogError("DataManager或SaveData未初始化，无法保存！");
                return;
            }
            if (memo.id < 0 || memo.id >= DataManager.Instance.saveData.memoList.Count)
            {
                Debug.LogError($"备忘录ID无效：{memo.id}，列表长度：{DataManager.Instance.saveData.memoList.Count}");
                return;
            }

            // 3. 更新并保存数据
            DataManager.Instance.saveData.memoList[memo.id].memoText = text;
            JsonDataTool.SaveData(DataManager.Instance.saveData); // 调用之前的JSON保存工具
        }

        private void SecreteKeyTest()
        {
            if (inputField.text.Trim() == "Freya")
            {
                Debug.Log("Freya");
            }
        }
    }
}