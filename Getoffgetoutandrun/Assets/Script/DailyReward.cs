using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailyReward : MonoBehaviour
{
    public bool initialized;
    //bir sonraki ödülü ne zaman alıcaz
    public long rewardGivingTimeTicks;
    public GameObject RewardMenu;

    public Text remainingTimeText;

    public void InitializeDailyReward()
    {
        //ödülü aldığı son tarihi tutma. Haskey = daha once giriş yapılmış mı demek ?
        if (PlayerPrefs.HasKey("lastDailyReward"))
        {
            //bir sonraki ödül verme tarihi tik verme cinsinden tutucaz
            rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;
            //suanki zaman
            long currentTime = System.DateTime.Now.Ticks;

            //ZAMAN GELDİYSE ODULU VER
            if (currentTime >= rewardGivingTimeTicks)
            {
                GiveReward();
            }
        }
        else
        {

            GiveReward();

        }
        initialized = true;
        

    }

    public void GiveReward()
    {
        //100 prim para ver
        LevelKontrol.Current.GiveMoneyToPlayer(100);
        RewardMenu.SetActive(true);
        //Son alım tarihini guncelle
        PlayerPrefs.SetString("lastDailyReward" ,System.DateTime.Now.Ticks.ToString());
        //bir sonraki ödül alım günü
        rewardGivingTimeTicks = long.Parse(PlayerPrefs.GetString("lastDailyReward")) + 864000000000;


    }
    void Update()
    {
        //geri sayım
        if (initialized)
        {
            if (LevelKontrol.Current.StartMenu.activeInHierarchy)
            {
                long currentTime = System.DateTime.Now.Ticks;
                long remaningTime = rewardGivingTimeTicks - currentTime;

                //geriye kalan sayı 0'a eşit veya küçükse ödülü ver
                if (remaningTime <= 0)
                {
                    GiveReward();
                }
                else
                {

                    System.TimeSpan timeSpan = System.TimeSpan.FromTicks(remaningTime);
                    //format belli değişkenleri belli formatlarda yazma. D2 = 2 basamaktan kucuk bile olsa 0.5 gibi donusturcek
                    remainingTimeText.text = string.Format("{0}:{1}:{2}",timeSpan.Hours.ToString("D2"), timeSpan.Minutes.ToString("D2"), timeSpan.Seconds.ToString("D2"));

                }
            }
        }
    }
    public void TapToReturnButton()
    {
        RewardMenu.SetActive(false);
    }
}
