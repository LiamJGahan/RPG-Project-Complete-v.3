using RPG.Inventories;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.UI
{
    public class PurseUI : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI balance;

        Purse playerPurse = null;

        void Start()
        {
            playerPurse = GameObject.FindGameObjectWithTag("Player").GetComponent<Purse>();

            if(playerPurse != null)
            {
                playerPurse.balanceChanged += RefreshUI;
            }

            RefreshUI();
        }

        public void RefreshUI()
        {
            balance.text = $"£{playerPurse.GetBalance():N2}";
        }
    }
}
