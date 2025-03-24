using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.UI.DamagePopupText
{
    public class DamagePopupSpawner : MonoBehaviour
    {
        [SerializeField] DamagePopupText damagePopupText = null;

        public void spawn(float damageAmount)
        {
            DamagePopupText instance = Instantiate<DamagePopupText>(damagePopupText, transform);
            instance.SetValue(damageAmount);
        }
    }
}
