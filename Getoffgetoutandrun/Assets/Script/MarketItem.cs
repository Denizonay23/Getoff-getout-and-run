using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarketItem : MonoBehaviour
{
    //itemid = eşyalarımızın son durumlarını tutacağız. wearId = eşyamızın hangi kategoriye ait olduğunu tutacak.
    public int itemId, wearId;

    //eşya fiyat bilgisi
    public int price;

    //fiyat yazısı guncelleme
    public Text priceText;

    public Button buyButton, EquipButton, UnequipButton;

    //eşyanın prefabına ulaşma
    public GameObject itemPrefab;

    //eşyayı almış ise true almamış ise false
    public bool HasItem()
    {
        // 0: Daha satın alınmamış.
        // 1: Satın alınmış ama giyilmemiş.
        // 2: Hem satın alınmış hem de giyilmiş.
        
       bool hasıtem = PlayerPrefs.GetInt("item" + itemId.ToString()) != 0;
        return hasıtem;

    }
    //Eşyanın giyilip giyilmediğini belirleme
    public bool IsEquipped()
    {


        bool Equıpıtem = PlayerPrefs.GetInt("item" + itemId.ToString()) == 2;
        return Equıpıtem;

    }
    //fiyat yazısını guncelleme ve eşyaya sahip olup olmadıgını belirtme

    public void InitialiazeItem()
    {
        //fiyat bilgisi güncelleme
        priceText.text = price.ToString();

        //oyuncu bu eşyayı satın almış mı ?
        if (HasItem())
        {
            buyButton.gameObject.SetActive(false);

            //eşyanın giyilip giyilmediği
            if (IsEquipped())
            {
                EquipItem();
            }
            else
            {

                EquipButton.gameObject.SetActive(true);

            }
        }
        else
        {

            buyButton.gameObject.SetActive(true);

        }

    }
    public void BuyItem()
    {
        //Kullanıcıda bu eşya var mı
        if (!HasItem())
        {
            int money = PlayerPrefs.GetInt("money");

            if (money >= price)
            {
                PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.BuyAudioClip, 0.1f);
                LevelKontrol.Current.GiveMoneyToPlayer(-price);
                PlayerPrefs.SetInt("item" + itemId.ToString(), 1);
                buyButton.gameObject.SetActive(false);
                EquipButton.gameObject.SetActive(true);
            }
        }


    }
    public void EquipItem()
    {

        //giyili olan eşyayı çıkarma
        UnequipItem();

        //prefabdan obje yaratma ve  market controllerde giyilen objeyelere atıyoruz
       MarketController.Current.equippedItems[wearId] = Instantiate(itemPrefab, PlayerController.Current.wearSpots[wearId].transform).GetComponent<Item>();
        //giyilmiş olan objenın ıdsını marketitemın ıdsıne eşıtleme
        MarketController.Current.equippedItems[wearId].itemId = itemId;
        EquipButton.gameObject.SetActive(false);
        UnequipButton.gameObject.SetActive(true);

        //Bu eşyanın giyili oldugunu kayıt etme
        PlayerPrefs.SetInt("item" + itemId.ToString(), 2);



    }
    public void UnequipItem()
    {
        //eşya giyilimi değilmi kontrol ediyoruz
        Item equippedItem = MarketController.Current.equippedItems[wearId];
        if (equippedItem != null)
        {
            //giyili olan eşyanın hangi market itemından oluştugunu bulmuş oluyoruz.
            MarketItem marketItem = MarketController.Current.items[equippedItem.itemId];
            PlayerPrefs.SetInt("item" + marketItem.itemId, 1);
            marketItem.EquipButton.gameObject.SetActive(true);
            marketItem.UnequipButton.gameObject.SetActive(false);
            Destroy(equippedItem.gameObject);
        }
    }

    public void EquipItemButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.equipItemAudioClip, 0.1f);
        EquipItem();
    }
    public void UnequipItemButton()
    {
        PlayerController.Current.itemAudioSource.PlayOneShot(PlayerController.Current.unequipItemAudioClip, 0.1f);
        UnequipItem();
    }
}
