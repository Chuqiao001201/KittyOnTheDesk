using DataSystem;
using UnityEngine;
using Utilities;

namespace Memo
{
    public class Memo : MonoBehaviour
    {
        public int id;
        
        public void DestroySelf()
        {
            DataManager.Instance.saveData.memoList.RemoveAt(id);
            JsonDataTool.SaveData(DataManager.Instance.saveData);
            Destroy(gameObject);
        }
    }
}
