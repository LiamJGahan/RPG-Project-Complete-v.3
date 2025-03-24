using RPG.Attributes;
using RPG.Control;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig weapon = null;
        [SerializeField] float healthToRestore = 0; // Hack, must remove
        [SerializeField] float respawnTime = 5f;

        void OnTriggerEnter(Collider other)
        {
            if(other.gameObject.tag == "Player")
            {
                Pickup(other.gameObject);
            }
        }

        void Pickup(GameObject subject)
        {
            if(weapon != null)
            {
                subject.GetComponent<Fighter>().EquipWeapon(weapon);
            }
            if(healthToRestore > 0)
            {
                subject.GetComponent<Health>().Heal(healthToRestore);
            }

            StartCoroutine(HideForSeconds(respawnTime));
        }

        IEnumerator HideForSeconds(float seconds)
        {
            ShouldShowPickup(false);
            yield return new WaitForSeconds(seconds);
            ShouldShowPickup(true);

        }

        void ShouldShowPickup(bool shouldShow)
        {
            gameObject.GetComponent<SphereCollider>().enabled = shouldShow;

            foreach(Transform child in transform)
            {
                child.gameObject.SetActive(shouldShow);
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Pickup;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                Pickup(callingController.gameObject);
            }
            return true;
        }
    }

}