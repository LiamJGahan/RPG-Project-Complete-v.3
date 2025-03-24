using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class Shopper : MonoBehaviour
    {
        Shop activeShop = null;

        public event Action activeShopChange;

        public void SetActiveShop(Shop shop)
        {
            if(activeShop != null)
            {
                activeShop.SetupShopper(null);
            }

            activeShop = shop;

            if(activeShop != null)
            {
                activeShop.SetupShopper(this);
            }

            if(activeShopChange != null)
            {
                activeShopChange();
            }
        }

        public Shop GetActiveShop()
        {
            return activeShop;
        }
    }
}
