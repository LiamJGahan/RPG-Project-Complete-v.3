using GameDevTV.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        InventoryItem inventoryItem;
        int availability;
        float price;
        int quantityInTransaction;

        public ShopItem(InventoryItem inventoryItem, int availability, float price, int quantityInTransaction)
        {
            this.inventoryItem = inventoryItem;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public Sprite GetIcon()
        {
            return inventoryItem.GetIcon();
        }

        public string GetName()
        {
            return inventoryItem.GetDisplayName();
        }

        public int GetAvailability()
        {
            return availability;
        }

        public float GetPrice()
        {
            return price;
        }

        public int GetQuantityInTransaction()
        {
            return quantityInTransaction;
        }

        public InventoryItem GetInventoryItem()
        {
            return inventoryItem;
        }
    }
}

