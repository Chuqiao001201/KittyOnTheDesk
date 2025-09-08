using System;
using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace DataSystem
{
    public class DataManager : Singleton<DataManager>
    {
        // 保存的数据对象
        public SaveData saveData;

        protected override void Awake()
        {
            base.Awake();
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
