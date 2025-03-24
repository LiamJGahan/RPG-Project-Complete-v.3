using GameDevTV.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99f)]
        [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression;
        [SerializeField] GameObject levelUpParticleEffect = null;
        [SerializeField] bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;

        Experience experience;

        void Awake()
        {
            experience = GetComponent<Experience>();

            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        void OnEnable()
        {
            if (experience != null)
            {
                experience.onExperienceGained += UpdateLevel;
            }
        }

        void OnDisable()
        {
            if (experience != null)
            {
                experience.onExperienceGained -= UpdateLevel;
            }
        }

        void Start()
        {
            currentLevel.ForceInit();
        }

        void UpdateLevel()
        {
            int newLevel = CalculateLevel(); 

            if(newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                levelUpEffect();
                onLevelUp();
            }
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAddativeModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }

        public int GetLevel()
        {
            return currentLevel.value;
        }

        int CalculateLevel()
        {
            Experience experience = GetComponent<Experience>();

            if(experience == null) { return startingLevel; }

            float currentXP = experience.GetPoints();

            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);

            for(int level = 1; level <= penultimateLevel; level++)
            {
                float xpToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                
                if(xpToLevelUp > currentXP)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }

        void levelUpEffect()
        {
            Instantiate(levelUpParticleEffect, gameObject.transform);
        }

        float GetAddativeModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float sumTotal = 0f;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    sumTotal += modifier;
                }
            }
            return sumTotal;
        }

        float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float sumTotal = 0f;

            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    sumTotal += modifier;
                }
            }
            return sumTotal;
        }
    }
}
