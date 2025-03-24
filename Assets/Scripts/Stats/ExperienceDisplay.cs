using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace RPG.Stats
{
    public class ExperienceDisplay : MonoBehaviour
    {
        [SerializeField] Text scoreText;

        Experience experience;

        void Awake()
        {
            experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        void Update()
        {
            scoreText.text = string.Format("XP: {0:0}", experience.GetPoints());
        }
    }
}
