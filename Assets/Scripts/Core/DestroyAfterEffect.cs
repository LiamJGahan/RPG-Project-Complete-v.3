using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterEffect : MonoBehaviour
    {
        [SerializeField] GameObject targetToDestroy = null;

        ParticleSystem isAlive;

        void Start()
        {
            isAlive = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if(!isAlive.IsAlive())
            {
                if(targetToDestroy != null)
                {
                    Destroy(targetToDestroy);
                }
                else
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}
