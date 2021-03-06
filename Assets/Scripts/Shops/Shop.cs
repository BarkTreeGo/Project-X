using GameDevTV.Inventories;
using GameDevTV.Saving;
using RPG.Control;
using RPG.Inventories;
using RPG.Stats;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace RPG.Shops
{
    public class Shop : MonoBehaviour, IRaycastable, ISaveable
    {
        [SerializeField] string shopName;
        [Range(0,100)] [SerializeField] float sellingPercentage = 70f;
        [SerializeField] float maximumBarterDiscount = 80f;

        //Stock Config
        [SerializeField] StockItemConfig[] stockConfig;              

        [System.Serializable]
        class StockItemConfig
        {
            public InventoryItem item;
            public int initialStock;
            [Range(0,100)] public float buyingDiscountPercentage;
            public int levelToUnlock = 0;
        }

        Dictionary<InventoryItem, int> transaction = new Dictionary<InventoryItem, int>();
        Dictionary<InventoryItem, int> stockSold = new Dictionary<InventoryItem, int>();
        
        bool isBuyingMode = true;
        Shopper currentShopper = null;        
        ItemCategory filter = ItemCategory.None;

        public event Action onChange;       

        public void SetShopper(Shopper shopper)
        {
            currentShopper = shopper;
        }

        public string GetShopName()
        {
            return shopName;
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

            foreach (InventoryItem item in availabilities.Keys)
            {               
                if (availabilities[item] <= 0) { continue; }

                float price = prices[item];
                int quantityInTransaction = 0;
                transaction.TryGetValue(item, out quantityInTransaction); //if item is in the transactiondictionary it will give the quantity, if not in there it returns 0 by default
                int availability = availabilities[item];         
              
                yield return new ShopItem(item, availability, price, quantityInTransaction);
            }
        }
        private Dictionary<InventoryItem, float> GetPrices()
        {
            Dictionary<InventoryItem, float> prices = new Dictionary<InventoryItem, float>();            

            foreach (var config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!prices.ContainsKey(config.item))
                    {
                        prices[config.item] = config.item.GetPrice() * GetBarterDiscount();
                    }                
                    prices[config.item] *= (1 - config.buyingDiscountPercentage / 100);
                }                              
                else
                {
                    prices[config.item] = config.item.GetPrice() * (sellingPercentage / 100);
                }
            }
            return prices;
        }

        private float GetBarterDiscount()
        {
            BaseStats baseStats = currentShopper.GetComponent<BaseStats>();
            float discount = baseStats.GetStat(Stat.BuyingDiscountPercentage);
            return 1 - (Mathf.Min(discount, maximumBarterDiscount) / 100);
        }

        private Dictionary<InventoryItem, int> GetAvailabilities()
        {
            Dictionary<InventoryItem, int> availabilities = new Dictionary<InventoryItem, int>();

            foreach (var config in GetAvailableConfigs())
            {
                if (isBuyingMode)
                {
                    if (!availabilities.ContainsKey(config.item))
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

        private IEnumerable<StockItemConfig> GetAvailableConfigs()
        {
            int shopperLevel = GetShopperLevel();
            foreach (var config in stockConfig)
            {
                if (config.levelToUnlock > shopperLevel) continue;
                {
                    yield return config;
                }
            }
        }

        public void ConfirmTransaction()
        {
            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();                               
            Purse shopperPurse = currentShopper.GetComponent<Purse>();

            if (shopperInventory == null || shopperPurse == null) { return; }

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();
                float price = shopItem.GetPrice();

                for (int i = 0; i < quantity; i++)
                {
                    if (isBuyingMode)
                    {
                        BuyItem(shopperInventory, shopperPurse, item, price);
                    }
                    else
                    {
                        SellItem(shopperInventory, shopperPurse, item, price);
                    }
                }                
            }
            if (onChange != null)
            {
                onChange();
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

        public bool canTransact()
        {
            if (IsTransactionEmpty()) return false;
            if (!HasSufficientFunds()) return false;
            if (!HasSufficientInventorySpace()) return false;

            return true;
        }
        public bool IsTransactionEmpty()
        {
            return transaction.Count == 0;
        }
        public bool HasSufficientFunds()
        {
            if (!isBuyingMode) { return true; }
            
            Purse purse = currentShopper.GetComponent<Purse>();
            if (purse == null) { return false; }

            return purse.GetBalance() >= TransactionTotal();
        }

        public bool HasSufficientInventorySpace()
        {
            if (!isBuyingMode) { return true; }

            Inventory shopperInventory = currentShopper.GetComponent<Inventory>();
            if (shopperInventory == null) { return false; }

            List<InventoryItem> flatItems = new List<InventoryItem>();

            flatItems.Clear();

            foreach (ShopItem shopItem in GetAllItems())
            {
                InventoryItem item = shopItem.GetInventoryItem();
                int quantity = shopItem.GetQuantityInTransaction();                

                for (int i = 0; i < quantity; i++)
                {
                    flatItems.Add(item);
                }
            }

            return shopperInventory.HasSpaceFor(flatItems);
        }       

        public float TransactionTotal()
        {
            float total = 0f;
            foreach (ShopItem item in GetAllItems())
            {
                total += item.GetPrice() * item.GetQuantityInTransaction();
            }            
            return total;
        }

        public void AddToTransaction(InventoryItem item, int quantity)
        {
            if (!transaction.ContainsKey(item))
            {
                transaction[item] = 0; //adds the item as a key in the transaction dictionary
            }

            var availabilities = GetAvailabilities();
            int availability = availabilities[item];
            if (transaction[item] + quantity > availability)
            {
                transaction[item] = availability;
            }
            else
            {
                transaction[item] += quantity;
            }            

            if (transaction[item] <= 0)
            {
                transaction.Remove(item);
            }

            if (onChange != null)
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
            if (Input.GetMouseButton(0))
            {
                callingController.GetComponent<Shopper>().SetActiveShop(this);
            }

            return true;
        }        

        private int CountItemsInInventory(InventoryItem item)
        {
            Inventory inventory = currentShopper.GetComponent<Inventory>();

            if (inventory == null) { return 0; }
            int total = 0;

            for (int i = 0; i < inventory.GetSize(); i++)
            {
                if (item == inventory.GetItemInSlot(i))
                {
                    total += inventory.GetNumberInSlot(i);
                }
            }
            return total;
        }

        
        private void BuyItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            if (shopperPurse.GetBalance() < price) { return; }

            bool success = shopperInventory.AddToFirstEmptySlot(item, 1);
            if (success)
            {
                AddToTransaction(item, -1);
                if (!stockSold.ContainsKey(item))
                {
                    stockSold[item] = 0;
                }
                stockSold[item]++;
                shopperPurse.UpdateBalance(-price);
            }
        }
        private void SellItem(Inventory shopperInventory, Purse shopperPurse, InventoryItem item, float price)
        {
            int slot = FindFirstItemSlot(shopperInventory, item);
            if (slot == -1) { return; }
            AddToTransaction(item, -1);
            shopperInventory.RemoveFromSlot(slot, 1);
            if (!stockSold.ContainsKey(item))
            {
                stockSold[item] = 0;
            }
            stockSold[item]--;
            shopperPurse.UpdateBalance(price);
        }

        private int FindFirstItemSlot(Inventory shopperInventory, InventoryItem item)
        {         
            int slot = 0;

            for (int i = 0; i < shopperInventory.GetSize(); i++)
            {
                if (item == shopperInventory.GetItemInSlot(i))
                {
                    return i;
                }
            }
            return -1;
        }        

        private int GetShopperLevel()
        {
            BaseStats stats = currentShopper.GetComponent<BaseStats>();

            if (stats == null) { return 0; }

            return stats.GetLevel();
        }

        public object CaptureState()
        {
            Dictionary<string, int> saveObject = new Dictionary<string, int>();

            foreach (var pair in stockSold)
            {
                saveObject[pair.Key.GetItemID()] = pair.Value;
            }

            return saveObject;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, int> saveObject = (Dictionary<string, int>)state;

            stockSold.Clear();

            foreach (var pair in saveObject)
            {
                stockSold[InventoryItem.GetFromID(pair.Key)] = pair.Value;
            }            
        }
    }

}
