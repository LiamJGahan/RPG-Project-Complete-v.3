using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Combat
{
    public class EnemyHealthDisplay : MonoBehaviour
    {
        [SerializeField] Text text;
        Fighter fighter;

        void Awake()
        {
            fighter = GameObject.FindWithTag("Player").GetComponent<Fighter>();
        }

        void Update()
        {
            if(fighter.GetTarget() == null) { return; }

            Health health = fighter.GetTarget();
            text.text = string.Format("Enemy: {0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
