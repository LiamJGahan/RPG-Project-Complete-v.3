using GameDevTV.Inventories;
using RPG.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class FilterButtonUI : MonoBehaviour
    {
        [SerializeField] ItemCategory category = ItemCategory.None;

        Button button;
        Shop currentShop;

        void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(SelectFilter);
        }

        public void SetShop(Shop currentShop)
        {
            this.currentShop = currentShop;         
        }

        public void RefreshUI()
        {
            button.interactable = currentShop.GetFilter() != category;
        }

        void SelectFilter()
        {
            currentShop.SelectFilter(category);
        }
    }
}
