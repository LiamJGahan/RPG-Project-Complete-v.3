using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamagePopupText
{
    public class DamagePopupText : MonoBehaviour
    {
        [SerializeField] Text damageText = null;

        public void SetValue(float amount)
        {
            damageText.text = string.Format("{0:0}", amount);
        }
    }
}
