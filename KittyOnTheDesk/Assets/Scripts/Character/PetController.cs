using System;
using System.Collections;
using Food;
using UnityEngine;
using Utilities;

namespace Character
{
    public class PetController : Singleton<PetController>
    {
        [Header("基础属性")]
        public float hungry;
        public float happy;
        public float sleepy;
        [Header("状态")]
        public PetState state;
        public float hungryThreshold = 30;
        
        private Coroutine eatCoroutine;
        private Coroutine hungryCoroutine;
        private Animator animator;

        private void OnEnable()
        {
            if (hungryCoroutine == null)
                hungryCoroutine = StartCoroutine(HungryCoroutine());
        }

        private void OnDisable()
        {
            if (hungryCoroutine != null)
            {
                StopCoroutine(hungryCoroutine);
                hungryCoroutine = null;
            }
        }

        private void Start()
        {
            animator = GetComponent<Animator>();
        }

        // private void Update()
        // {
        //     if (hungry < hungryThreshold && state != PetState.Eating)
        //     {
        //         Eat(FoodSpawner.Instance.DeliverFood());
        //     }
        // }

        public void Eat(int num)
        {
            if (hungry >= hungryThreshold)
            {
                Debug.Log("没有饿");
                return;
            }
            if (num <= 0)
            {
                Debug.Log("没有食物");
                return;
            }
            if (eatCoroutine != null || state == PetState.Eating)
            {
                Debug.Log("正在进食");
                return;
            }

            eatCoroutine = StartCoroutine(EatCoroutine(num));
        }

        private IEnumerator EatCoroutine(int num)
        {
            Debug.Log("开始进食");
            state = PetState.Eating;
            SwitchAnimation();
            for (var i = 0; i < num; i++)
            {
                if (!(hungry < 100)) continue;
                hungry += 0.5f;
                yield return new WaitForSeconds(1);
            }
            state = PetState.Idle;
            SwitchAnimation();
            eatCoroutine = null;
            Debug.Log("结束进食");
        }

        private IEnumerator HungryCoroutine()
        {
            while (true)
            {
                if (hungry > 0) hungry--;
                yield return new WaitForSeconds(60);
            }
        }

        private void SwitchAnimation()
        {
            animator.SetInteger("State", (int)state);
        }
    }

    public enum PetState
    {
        Idle, Eating
    }
}
