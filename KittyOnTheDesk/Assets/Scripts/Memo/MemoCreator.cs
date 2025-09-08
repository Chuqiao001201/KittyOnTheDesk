using DataSystem;
using TMPro;
using UnityEngine;
using Utilities;

namespace Memo
{
    public class MemoCreator : MonoBehaviour
    {
        [SerializeField] private GameObject memoPrefab;

        private void Start()
        {
            for (var i = 0; i < DataManager.Instance.saveData.memoList.Count; i++)
            {
                var memoData = DataManager.Instance.saveData.memoList[i];
                var memo = Instantiate(memoPrefab, transform.position, transform.rotation, transform);
                memo.transform.localPosition = memoData.memoPos;
                memo.GetComponentInChildren<TMP_InputField>().text = memoData.memoText;
                memo.GetComponent<global::Memo.Memo>().id = i;
            }
        }

        public void CreateMemo()
        {
            var memo = Instantiate(memoPrefab, transform.position, transform.rotation, transform);
            DataManager.Instance.saveData.memoList.Add(
                new MemoData(memo.transform.localPosition, ""));
            memo.GetComponent<global::Memo.Memo>().id = DataManager.Instance.saveData.memoList.Count-1;
            JsonDataTool.SaveData(DataManager.Instance.saveData);
        }
    }
}
