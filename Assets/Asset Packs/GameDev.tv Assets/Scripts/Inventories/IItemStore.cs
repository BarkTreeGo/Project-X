using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameDevTV.Inventories
{
    public interface IItemStore
    {
        int AddItems(InventoryItem item, int number);
    }
}
