using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameDevTV.Saving;
using System;

namespace RPG.Stats
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float experiencePoints = 0;

        //public delegate void ExperienceGainedDelegate(); using system means you can use "Action" for void type delegates as a built in template instead of constantly naming delegates
        public event Action onExperienceGained;     //without using "Action" as the delegate you can create custom delegates as seen above a give different retern types

        //Delete This!
        private void Update()
        {
            if (Input.GetKey(KeyCode.E))
            {
                GainExperience(Time.deltaTime * 1000);
            }
        }

        public void GainExperience(float experience)
        {
            experiencePoints += experience;
            onExperienceGained();
        }

        public float GetPoints()
        {
            return experiencePoints;
        }

        public object CaptureState()
        {
            return experiencePoints;
        }


        public void RestoreState(object state)
        {
            experiencePoints = (float)state;
        }
    }
}
