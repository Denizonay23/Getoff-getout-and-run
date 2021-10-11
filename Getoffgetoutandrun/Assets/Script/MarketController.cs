using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketController : MonoBehaviour
{
    public static MarketController Current;

    //satın alınmış yada alınmamış listeleri listeleyecek
    public List<MarketItem> items;

    //Giyinmiş Olan Eşyaların Listesi
    public List<Item> equippedItems;

    //Markete erişme
    public GameObject marketMenu;
    public void InıtializeMarketController()
    {

        Current = this;
        foreach (MarketItem item in items)
        {
            item.InitialiazeItem();
                
        }
    }
    public void ActiveMarketMenu(bool active)
    {
        marketMenu.SetActive(active);
    }
}
