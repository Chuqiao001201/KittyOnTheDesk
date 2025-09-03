using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

[RequireComponent(typeof(TMP_InputField))]
public class TMPInputFieldCustomBehavior : MonoBehaviour
{
    private TMP_InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        
        // 确保设置为多行模式
        inputField.lineType = TMP_InputField.LineType.MultiLineNewline;
        
        // 移除默认的提交事件，使用自定义处理
        inputField.onSubmit.RemoveAllListeners();
        inputField.onSubmit.AddListener(HandleSubmit);
    }

    private void Update()
    {
        // 当输入框获得焦点时处理按键
        if (inputField.isFocused)
        {
            // Alt+Enter组合键：换行
            if (Input.GetKeyDown(KeyCode.Return) && 
               (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightAlt)))
            {
                InsertNewLine();
                
                // 阻止事件被系统进一步处理
                EventSystem.current.SendMessage("Cancel", SendMessageOptions.DontRequireReceiver);
            }
            // 单独按Enter：触发提交
            else if (Input.GetKeyDown(KeyCode.Return))
            {
                // 触发提交事件
                inputField.onSubmit.Invoke(inputField.text);
            }
        }
    }

    // 插入换行并调整光标位置
    private void InsertNewLine()
    {
        int caretPosition = inputField.caretPosition;
        
        // 在当前光标位置插入换行符
        inputField.text = inputField.text.Insert(caretPosition, "\n");
        
        // 移动光标到换行符后面
        inputField.caretPosition = caretPosition + 1;
        
        // 保持输入焦点
        inputField.ActivateInputField();
    }

    // 处理提交逻辑
    private void HandleSubmit(string text)
    {
        // 这里添加你的确认逻辑
        Debug.Log("确认提交: " + text);
        
        // 可选：提交后是否保持焦点
        // inputField.ActivateInputField(); // 保持焦点
        // 或者
        EventSystem.current.SetSelectedGameObject(null); // 失去焦点
    }
}
