using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Stats
{
    public class LevelDisplay : MonoBehaviour
    {
        [SerializeField] Text levelText;

        BaseStats baseStats;

        void Awake()
        {
            baseStats = GameObject.FindWithTag("Player").GetComponent<BaseStats>();
        }

        void Update()
        {
            levelText.text = string.Format("Level: {0:0}", baseStats.GetLevel());
        }
    }
}
