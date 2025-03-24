using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] Text healthText;

        Health health;

        void Awake()
        {
            health = GameObject.FindWithTag("Player").GetComponent<Health>();
        }

        void Update()
        {
            healthText.text = string.Format("Health: {0:0}/{1:0}", health.GetHealthPoints(), health.GetMaxHealthPoints());
        }
    }
}
