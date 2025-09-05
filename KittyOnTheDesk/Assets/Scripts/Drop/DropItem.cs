using System;
using UnityEngine;

namespace Drop
{
    public class DropItem : MonoBehaviour
    {
        private Rigidbody2D rb2d;
        [SerializeField] private float threshold;

        private void Awake()
        {
            rb2d = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (rb2d.velocity.y < threshold) Destroy(gameObject);
        }
    }
}
