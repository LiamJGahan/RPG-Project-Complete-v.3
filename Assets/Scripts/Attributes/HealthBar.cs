using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Attributes
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] Health healthComponent = null;
        [SerializeField] RectTransform foreground = null;
        [SerializeField] Canvas canvas = null;

    void Update()
        {
            if(Mathf.Approximately(healthComponent.GetFraction(), 0)
            || Mathf.Approximately(healthComponent.GetFraction(), 1))
            {
                canvas.enabled = false;
                return;
            }

            canvas.enabled = true;
            foreground.localScale = new Vector3(healthComponent.GetFraction(), 1f, 1f);
        }
    }
}
