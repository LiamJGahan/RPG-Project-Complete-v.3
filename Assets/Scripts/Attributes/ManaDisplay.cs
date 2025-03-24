using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using RPG.Stats;

namespace RPG.Attributes
{
    public class ManaDisplay : MonoBehaviour
    {
        [SerializeField] Text manaText;

        Mana mana;

        void Awake()
        {
            mana = GameObject.FindWithTag("Player").GetComponent<Mana>();
        }

        void Update()
        {
            manaText.text = string.Format("Mana: {0:0}/{1:0}", mana.GetMana(), mana.GetMaxMana());
        }
    }
}
