using GameDevTV.Inventories;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Shops
{
    public class ShopItem
    {
        private InventoryItem item;
        private int availability;
        private float price;
        private int quantityInTransaction;

        public ShopItem(InventoryItem item, int availability, float price, int quantityInTransaction)
        {
            this.item = item;
            this.availability = availability;
            this.price = price;
            this.quantityInTransaction = quantityInTransaction;
        }

        public float GetPrice()
        {
            return price;
        }

        public int GetAvailability()
        {
            return availability;
        }

        public Sprite GetIcon()
        {
            return item.GetIcon();
        }

        public string GetName()
        {
            return item.GetDisplayName();
        }

        public InventoryItem GetInventoryItem()
        {
            return item;
        }

        public int GetQuantityInTransaction()
        {
            return quantityInTransaction;
        }
    }
}