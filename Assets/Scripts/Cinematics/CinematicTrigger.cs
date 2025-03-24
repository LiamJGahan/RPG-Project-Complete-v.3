using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using GameDevTV.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        bool hasTriggered = false;

        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == ("Player") && !hasTriggered)
            {
                GetComponent<PlayableDirector>().Play();
                hasTriggered = true;
            }
            else
            {
                return;
            }
        }

        public object CaptureState()
        {
            return hasTriggered;
        }

        public void RestoreState(object state)
        {
            hasTriggered = (bool)state;
        }
    }
}
