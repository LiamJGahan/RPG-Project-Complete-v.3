using RPG.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.Shops
{
    public class RowUI : MonoBehaviour
    {
        [SerializeField] Image icon;
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI availability;
        [SerializeField] TextMeshProUGUI price;
        [SerializeField] TextMeshProUGUI quantity;

        ShopItem currentItem = null;
        Shop currentShop = null;

        public void Setup(ShopItem item, Shop shop)
        {
            currentItem = item;
            currentShop = shop;

            icon.sprite = item.GetIcon();
            itemName.text = item.GetName();
            availability.text = $"{item.GetAvailability()}";
            price.text = $"£{item.GetPrice():N2}";
            quantity.text = $"{item.GetQuantityInTransaction()}";
        }

        public void Add()
        {
            currentShop.AddToTransaction(currentItem.GetInventoryItem(), 1);
        }

        public void Remove()
        {
            currentShop.AddToTransaction(currentItem.GetInventoryItem(), -1);
        }
    }

}