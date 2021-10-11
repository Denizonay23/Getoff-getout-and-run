using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelKontrol : MonoBehaviour
{
    public static LevelKontrol Current;
    public bool GameActive = false;

    //ilerleme
    public Slider LevelPrBar;
    //karakterin bitiş çizgisine olan uzaklıgını tutuyoruz
    public float MaxUzaklik;
    //bitiş çizgisini tutacak
    public GameObject BitisCizgisi;


    //Menuye Erisme
    public GameObject StartMenu, GameMenu, GameoverMenu, finisMenu;
    //textlere erişme
    public Text scoreText, FinishScoreText, CurrentLevelText, nextLevelText, StartingMenuMoneyText, gameOverMenuMoneyText, finishGameMenuMoneyText;

    int currentLevelS;

    // oyunumuzun müziğinin ses kaynağı
    public AudioSource GameMusicAudio;

    public AudioClip KazanmaAudio, GameOverAudio;

    //Skorları tutacak değişken
    int Score;

    //gunluk odul sistemi için
    public DailyReward dailyReward;

    void Start()
    {
        //text objelerini bul
        //GameObject[] parentSsInScene = this.gameObject.scene.GetRootGameObjects();
        //foreach (GameObject parent in parentSsInScene)
        //{
        //    TextObject[] textObjectsInParent = parent.GetComponentsInChildren<TextObject>(true);
        //    foreach (TextObject textObject in textObjectsInParent)
        //    {
        //        textObject.InitTextObject();
        //    }
        //}

        Current = this;

         currentLevelS = PlayerPrefs.GetInt("currentLevel");

     
       
        //bulduğu ilk playercontroller sınıfını current değişkenıne eşitleyecek
        PlayerController.Current = GameObject.FindObjectOfType<PlayerController>();
        GameObject.FindObjectOfType<MarketController>().InıtializeMarketController();

        dailyReward.InitializeDailyReward();
        CurrentLevelText.text = (currentLevelS + 1).ToString();
        nextLevelText.text = (currentLevelS + 2).ToString();
        
        //Oyuncunun parasına ulaşma
        UpdateMoney();

        
        //cameradan müziği çekicek
        GameMusicAudio = Camera.main.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameActive)
        {
            //playercontrolleri tutuyoruz
            PlayerController player = PlayerController.Current;
           
            //karakterin çizgiye uzaklığı
            float distance = BitisCizgisi.transform.position.z - PlayerController.Current.transform.position.z;
            
            //bu değer bitis noktasına ne kadar uzaksa maksımum 1 verıcek ne kadar yakınsa minimum 0 vericek 0'dan 1 doğru ilerlemesi için 1 den çıkartıyoruz.
            LevelPrBar.value = 1 - (distance/MaxUzaklik);
        }
    }

    public void StartLevel()
    {
        //bitis çizgisinden karakterimizin z pozisyonunu cıkartıyoruz
        MaxUzaklik = BitisCizgisi.transform.position.z - PlayerController.Current.transform.position.z;
        PlayerController.Current.ChangeSpeed(PlayerController.Current.RunningSpeed);
        StartMenu.SetActive(false);
        GameMenu.SetActive(true);
        PlayerController.Current.anim.SetBool("Running", true);
        GameActive = true;

    }
    public void RestartLevel()
    {

        LevelLoader.Current.ChangeLevel(this.gameObject.scene.name);

    }
    public void LoadNextLevel()
    {

        LevelLoader.Current.ChangeLevel("Level " + (currentLevelS + 1));

    }
    public void GameOver()
    {
        UpdateMoney();
        GameMusicAudio.Stop();
        GameMusicAudio.PlayOneShot(GameOverAudio);
        GameMenu.SetActive(false);
        GameoverMenu.SetActive(true);
        GameActive = false;
        

    }
    public void FinishGame()
    {
        GiveMoneyToPlayer(Score);
        GameMusicAudio.Stop();
        GameMusicAudio.PlayOneShot(KazanmaAudio);
        PlayerPrefs.SetInt("currentLevel", currentLevelS + 1);
        //Bitis menusunun skorunu tutuyor
        FinishScoreText.text = Score.ToString();
        GameMenu.SetActive(false);
        finisMenu.SetActive(true);
        GameActive = false;
        
    }
    //sürekli skorun değişmesi
    public void ChangesScore(int Artis)
    {

        Score += Artis;
        scoreText.text = Score.ToString();

    }
    //o anki anlık paramızı hafıza biriminden çekicek yani para yazmızı guncelleiycez
    public void UpdateMoney()
    {

        int money = PlayerPrefs.GetInt("money");
        StartingMenuMoneyText.text = money.ToString();
        gameOverMenuMoneyText.text = money.ToString();
        finishGameMenuMoneyText.text = money.ToString();

    }
    //finish menusunu coin güncelleme
    public void GiveMoneyToPlayer(int increment)
    {
        // mathf max aldıgı 2 parametrenın arasında en büyüğünü döndürür.
        int money = PlayerPrefs.GetInt("money");
        money = Mathf.Max(0, money + increment);
        PlayerPrefs.SetInt("money", money);
        UpdateMoney();

    }
}
