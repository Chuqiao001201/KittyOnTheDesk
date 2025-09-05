using System;
using DataSystem;
using TMPro;
using UnityEngine;
using Utilities;

namespace MemoSystem
{
    public class MemoCreator : MonoBehaviour
    {
        [SerializeField] private GameObject memoPrefab;

        private void Start()
        {
            // foreach (var memoData in DataManager.Instance.saveData.memoList)
            // {
            //     var memo = Instantiate(memoPrefab, transform.position, transform.rotation, transform);
            //     memo.transform.localPosition = memoData.memoPos;
            //     memo.GetComponentInChildren<TMP_InputField>().text = memoData.memoText;
            // }
            for (var i = 0; i < DataManager.Instance.saveData.memoList.Count; i++)
            {
                var memoData = DataManager.Instance.saveData.memoList[i];
                var memo = Instantiate(memoPrefab, transform.position, transform.rotation, transform);
                memo.transform.localPosition = memoData.memoPos;
                memo.GetComponentInChildren<TMP_InputField>().text = memoData.memoText;
                memo.GetComponent<Memo>().id = i;
            }
        }

        public void CreateMemo()
        {
            var memo = Instantiate(memoPrefab, transform.position, transform.rotation, transform);
            DataManager.Instance.saveData.memoList.Add(
                new MemoData(memo.transform.localPosition, ""));
            memo.GetComponent<Memo>().id = DataManager.Instance.saveData.memoList.Count-1;
            JsonDataTool.SaveData(DataManager.Instance.saveData);
        }
    }
}
