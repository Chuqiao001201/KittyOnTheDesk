using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace DataSystem
{
    public class DataManager : MonoBehaviour
    {
        // 静态实例，全局唯一
        public static DataManager Instance { get; private set; }

        // 保存的数据对象
        public SaveData saveData;

        private void Awake()
        {
            // 单例模式实现
            if (!Instance)
            {
                // 如果实例不存在，设置为当前实例
                Instance = this;
                // 防止场景切换时被销毁
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                // 如果实例已存在，销毁当前重复对象
                Destroy(gameObject);
            }
            
            saveData = JsonDataTool.LoadData();
        }
    }
    
    [Serializable]
    public class SaveData
    {
        public Vector3 mainViewPos;
        public List<MemoData> memoList;
    }

    [Serializable]
    public class MemoData
    {
        public Vector3 memoPos;
        public string memoText;

        public MemoData(Vector3 pos, string txt)
        {
            memoPos = pos;
            memoText = txt;
        }
    }
}
