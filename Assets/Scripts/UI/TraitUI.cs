using RPG.Stats;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI
{
    public class TraitUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI unusedPoints;
        [SerializeField] Button confirmButton;

        TraitStore playerTraitStore = null;

        void Start()
        {
            playerTraitStore = GameObject.FindGameObjectWithTag("Player").GetComponent<TraitStore>();


            confirmButton.onClick.AddListener(playerTraitStore.Commit);
        }

        void Update()
        {
            unusedPoints.text = playerTraitStore.GetUnassignedPoints().ToString();
        }
    }
}
