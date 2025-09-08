using System;
using Character;
using UnityEngine;

namespace Food
{
    public class FoodButton : MonoBehaviour
    {
        private void OnMouseDown()
        {
            PetController.Instance.Eat(FoodSpawner.Instance.DeliverFood());
        }
    }
}
