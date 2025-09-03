using UnityEngine;

namespace MemoSystem
{
    public class MemoCreator : MonoBehaviour
    {
        [SerializeField] private GameObject memoPrefab;

        public void CreateMemo()
        {
            Instantiate(memoPrefab, transform.position, transform.rotation, transform);
        }
    }
}
