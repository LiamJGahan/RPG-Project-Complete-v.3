using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] string shopName = null;
        [SerializeField] [Range(0, 100)] float sellingPenaltyPercentage = 50f;
        [SerializeField] float maximumBarterDiscount = 80f;
        [SerializeField] StockItemConfig[] stockItemConfig;


        [System.Serializable]
        class StockItemConfig
        {
            [Tooltip("This will set the item to be stocked in the shop, duplicates of the item can be set to unlock with leveling.")]
            public InventoryItem item;
            [Tooltip("This will set the amount of inital stock for the item, this will stack against items of same type.")]
            public int initialStock;
            [Tooltip("Set the percentage to be discounted from the item price, this will stack against items of same type.")]
            [Range(0, 100)] public float buyingDiscountPercentage;
            [Tooltip("This item will not be unlocked in the shop until this level is reached.")]
            public int levelToUnlock = 0;
        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();

        Shopper currentShopper = null;
        bool isBuyingMode = true;
        ItemCategory filter = ItemCategory.None;

        public event Action onChange;

        public void SetupShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public IEnumerable<ShopItem> GetFilteredItems()
        {
            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                if (filter == ItemCategory.None || item.GetCategory() == filter)
                {
                    yield return shopItem;
                }
            }
        }

        public IEnumerable<ShopItem> GetAllItems()
        {
            Dictionary<InventoryItem, float> prices = GetPrices();
            Dictionary<InventoryItem, int> availabilities = GetAvailabilities();

            foreach(InventoryItem item in availabilities.Keys)
            {
                if (availabilities[item] <= 0) { continue; }

                float price = prices[item];
                int quantityInTransaction = 0;
                transaction.TryGetValue(item, out quantityInTransaction);
                int availability = availabilities[item];
                yield return new ShopItem(item, availability, price, quantityInTransaction);
            }
        }

        public void SelectFilter(ItemCategory category)
        {
            filter = category;

            if (onChange != null)
            {
                onChange();
            }
        }

        public ItemCategory GetFilter() 
        { 
            return filter; 
        }

        public string GetShopName() 
        { 
            return shopName;
        }

        public void SelectMode(bool isBuying)
        {
            isBuyingMode = isBuying;

            if (onChange != null)
            {
                onChange();
            }
        }

        public bool IsBuyingMode() 
        { 
            return isBuyingMode;
        }

        public bool CanTransact()
        {
            if(IsTrancsactionEmpty()) { return false; }
            if(!HasSufficientFunds()) { return false; }
            if(!HasInventorySpace()) { return false; }
            return true; 
        }

        public bool IsTrancsactionEmpty()
        {
            return transaction.Count == 0;
        }

        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) { return true; }

            Purse purse = currentShopper.GetComponent<Purse>();
            if(purse == null) { return false; }

            return purse.GetBalance() >= TransactionTotal();
        }

        public bool HasInventorySpace()
        {
            if(!isBuyingMode) { return true; }

            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if(shopperInventory == null) { return false; }

            List<InventoryItem> flatItems = new List<InventoryItem>();
            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();            

                for(int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }
            
            return shopperInventory.HasSpaceFor(flatItems);
        }

        public void ConfirmTransaction() 
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            Purse shopperPurse = currentShopper.GetComponent<Purse>();
            if(shopperInventory == null || shopperPurse == null) { return; }

            foreach(ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();

                for(int i = 0; i < quantity; i++)
                {
                    if(isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }
            }

            if(onChange != null)
            {
                onChange();
            }
        }

        public float TransactionTotal()
        {
            float total = 0;

            foreach(ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }
            return total;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if(!transaction.ContainsKey(item))
            {
                transaction[item] = 0;
            }

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];

            if(transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }

            if(transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            if(onChange != null)
            {
                onChange();
            }
        }

        public CursorType GetCursorType()
        {
            return CursorType.Shop;
        }

        public bool HandleRaycast(PlayerController callingController)
        {
            if(Input.GetMouseButtonDown(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }

            return true;
        }

        int CountItemsInInventory(InventoryItem item)
        {
            Inventory shopperInventroy = currentShopper.GetComponent<Inventory>();



            int itemtotal = 0;

            for(int i = 0; i < shopperInventroy.GetSize(); i++)
            {
                if(item == shopperInventroy.GetItemInSlot(i))
                {
                    itemtotal += shopperInventroy.GetNumberInSlot(i);
                }
            }

            return itemtotal;
        }

        Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();

            foreach(var config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if(!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice() * GetBarterDiscount();
                    }

                    prices[config.item] *= (1 - config.buyingDiscountPercentage / 100);
                }
                else
                {
                    //This is not optimal (Needs Work) must get the selling price to be half of the current buying price.

                    //float discountedPrice = config.item.GetPrice() * (1 - config.buyingDiscountPercentage / 100);
                    //prices[config.item] = discountedPrice - (discountedPrice / 100 * sellingPenaltyPercentage); - My ideas

                    prices[config.item] = config.item.GetPrice() * (sellingPenaltyPercentage / 100);
                }
            }

            return prices;
        }

        float GetBarterDiscount()
        {
            BaseStats baseStats = currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);

            return (1 - Mathf.Min(discount, maximumBarterDiscount) / 100);
        }

        Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

            foreach(var config in GetAvailableConfigs())
            {
                if(isBuyingMode)
                {
                    if(!availabilities.ContainsKey(config.item))
                    {
                        int sold = 0;
                        stockSold.TryGetValue(config.item, out sold);
                        availabilities[config.item] = -sold;
                    }
                    availabilities[config.item] += config.initialStock;
                }
                else
                {
                    availabilities[config.item] = CountItemsInInventory(config.item);
                }
            }

            return availabilities;
        }

        IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach(var config in stockItemConfig)
            {
                if(config.levelToUnlock > shopperLevel) continue;
                yield return config;
            }
        }

        void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if(shopperPurse.GetBalance() < price) { return; }

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);

            if(success)
            {              
                AddToTransaction(item, -1);

                if(!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;
                }

                stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }

        void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if(slot == -1) { return; }

            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);

            if (!stockSold.ContainsKey(item))
            {
                stockSold[item] = 0;
            }

            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {
            for(int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (shopperInventory.GetItemInSlot(i) == item)
                {
                    return i;
                }
            }
            return -1;
        }

        int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();
            if(stats == null) { return 0; }

            return stats.GetLevel();
        }

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();

            foreach(var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }

            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;
            stockSold.Clear();
            foreach(var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }
        }
    }
}