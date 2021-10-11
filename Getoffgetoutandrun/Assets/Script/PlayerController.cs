using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // belirli bir süre beklemek için
    private float _KopruYaratmaZamanlayicisi;

    public static PlayerController Current;

    public float limitX;
    public float RunningSpeed;

    public float xSpeed;

    private float _curentRunningSpeed;
    //Cylinder prefabı tutacak
    public GameObject ridingCylinderPrfab;

    //ayagımın altındaki silindiri tutacak
    public List<CylinderRiding> cylinders;

    //köprü oluşturma veya oluşturmama true false
    private bool _spawnBridge;
    // köprü parçalarını yaratma ve değişkende tutma
    public GameObject bridgePrefab;
    // baslangıc ve bitis spawner
    private BridgeSpawner _bridgeSpawner;

    public Animator anim;

    //Skor Zamanlayici
    private float _TimerScore = 0;

    //bitiscizgisine gelip gelmediğini tutacak
    private bool _finished;

    //Oyuncunun ekrana dokundugu son yatay pozisyonu tutacağız
    private float _lastTouchX;

    //Ses
    public AudioSource cylinderAudioS,TriggerAudioSound, itemAudioSource;

    //silindir büyürken ve küçülürken çıkarılan sesler
    public AudioClip gatherAudioClip, dropAudioClip, coinAudio, BuyAudioClip,equipItemAudioClip, unequipItemAudioClip;

    //cilindir azalma sesi
    private float _dropSoundTi;

    //karakterimize giyilme alanları oluşturuyoruz
    public List<GameObject> wearSpots;

    // Update is called once per frame
    void Update()
    {
        if (LevelKontrol.Current == null || !LevelKontrol.Current.GameActive)
        {
            return;
        }
        float newX = 0;
        float touchXdelta = 0;
        //telefon ayarları
        //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved)
        if (Input.touchCount > 0)
        {
            //Began ekrana ilk dokunus demek
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                _lastTouchX = Input.GetTouch(0).position.x;
            }

            //Eğer ilk defa dokunuyorsa parmağını hareket ettiriyorsa
            else if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {

                touchXdelta = 5 * (Input.GetTouch(0).position.x - _lastTouchX) / Screen.width;
                //lasttouchX guncelleme
                _lastTouchX = Input.GetTouch(0).position.x;
            }

            //touchXdelta = Input.GetTouch(0).deltaPosition.x / Screen.width;
        }
        // pc ayarları
        else if(Input.GetMouseButton(0))
        {

            touchXdelta = Input.GetAxis("Mouse X");

        }
        //kaydırma hızı
        newX = transform.position.x + xSpeed * touchXdelta * Time.deltaTime;
        //sag ve sol limitlerde sınırlandırma
        newX = Mathf.Clamp(newX,-limitX,limitX);

        //Z ekseni yonunde ilerlemesini sağlıyor
        Vector3 newPosition = new Vector3(newX,transform.position.y,transform.position.z + _curentRunningSpeed * Time.deltaTime);
        //karakterin pozisyonunu newPositiona eşitliyoruz
        transform.position = newPosition;

        //Kopru yaratıp yaratmadıgını kontrol etme
        if (_spawnBridge)
        {
            DropSound();
            //kopru yarattıgımız her surede zamanı cıkartıcaz yani saniye sayıyormus gibi olacak
            _KopruYaratmaZamanlayicisi -= Time.deltaTime;

            //kopru parçası yaratma ce 
            if (_KopruYaratmaZamanlayicisi < 0)
            {
                //kopruyaratma zamanlayıcı guncelleme
                _KopruYaratmaZamanlayicisi = 0.01f;
                //silindir hacmi küçültme
                CylinderHacmiArtir(-0.01f);

                // yenı kopru parçası yaratma ve prefabı karakterin childı yapıyoruz.
                GameObject creatingBridge = Instantiate(bridgePrefab,this.transform);

                //sonra parentını null yapıp objeyi karakterin childı olmaktan kurtarıyoruz.
                creatingBridge.transform.SetParent(null);

                //dogru konuma getirmek için
                Vector3 direction = _bridgeSpawner.endReference.transform.position - _bridgeSpawner.startReference.transform.position;
                
                //magnitu yon vektörünün ağırlığı
                float distance = direction.magnitude;
                
                //birim vektore donusturdum
                direction = direction.normalized;

                //yarattıgımız obejenın yonunu dogru yone çeviriyoruz

                creatingBridge.transform.forward = direction;

                // Karakterimiz baslangıc referans noktasından ne kadar ilerde
                float characterDistance = transform.position.z - _bridgeSpawner.startReference.transform.position.z;

                //çıkardıgımız değeri 0 ve maksimum uzaklıkla sınırlandırıyoruz
                characterDistance = Mathf.Clamp(characterDistance,0,distance);

                //yarattıgımız objenın yenı pozisyonunu tutma
                Vector3 NewPiecePosition = _bridgeSpawner.startReference.transform.position + direction * characterDistance;
                
                // yeni parça pozisyonunu NewPiece pozisyonu ile değiştirme
                NewPiecePosition.x = transform.position.x;

                //parçanın nereden çıkacağını belirliyoruz nowposition dersek karakterin altından çıkar.
                creatingBridge.transform.position = NewPiecePosition;

                //Bitis çizgisine gelmiş ise belli süreler içerisinde skor kazan
                if (_finished)
                {
                    _TimerScore -= Time.deltaTime;
                    //SKOR ZAMANLAYICIM 0 İSE YADA 0'DAN KUCUK ISE guncelliyoruz
                    if (_TimerScore < 0)
                    {
                        _TimerScore = 0.3f;
                        //skoru 1 artırma
                        LevelKontrol.Current.ChangesScore(1);
                    }
                }
            }
        }

    }

    //level Controlun player controlun hızını değiştiriyoruz
    public void ChangeSpeed(float value)
    {

        _curentRunningSpeed = value;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "AddCylinder")
        {
            cylinderAudioS.PlayOneShot(gatherAudioClip, 0.1f);
            CylinderHacmiArtir(0.1f);
            Destroy(other.gameObject);

        }else if(other.tag == "SpawnBridge")
        {
            // carptıgı nesne kopru yaratmaya basladıgı nesne 
            StartSpawnBridge(other.transform.parent.GetComponent<BridgeSpawner>());

        }else if(other.tag == "StopSpawnBridge")
        {

            StopBridge();

            //Oyunu Bitirdin
            if (_finished)
            {
                LevelKontrol.Current.FinishGame();
            }

        }else if (other.tag == "Finish")
        {

            _finished = true;
            StartSpawnBridge(other.transform.parent.GetComponent<BridgeSpawner>());
        }else if (other.tag == "Coin")
        {
            TriggerAudioSound.PlayOneShot(coinAudio, 0.1f);
            //cilindirin altına değüp fazla coin almasını engelleme
            other.tag = "Untagged";
            LevelKontrol.Current.ChangesScore(10);
            Destroy(other.gameObject);

        }
    }

    private void OnTriggerStay(Collider other)
    {

      //karakter oldukten sonra silindir sesini kapatma
      if (LevelKontrol.Current.GameActive)
    {
       
          //trap tagına basınca canı gitsin
          if (other.tag == "Trap")
        {
            DropSound();
            CylinderHacmiArtir(-Time.fixedDeltaTime);
        }

     }
        
    }

    public void CylinderHacmiArtir(float value)
    {
        // Count eleman demek
        if (cylinders.Count == 0)
        {
            if (value > 0)
            {
                CreateCylinder(value);
            }
            else
            {
                if (_finished)
                {
                //bitis çizgisine ulasmısa leveli bitir
                LevelKontrol.Current.FinishGame();
                }
                // eğer çizgiye ulaşmadan silindirleri bitti ise ve eksiye düştü ise gameover
                else
                {

                    Die();

                }
                
               

            }
        }
        else
        {

            cylinders[cylinders.Count - 1].CylinderBoyutuArtir(value);

        }

    }

    public void Die()
    {
        anim.SetBool("Dead", true);
        //karakter layerini 6 yapma
        gameObject.layer = 6;
        //karakter olurken kamera takip etmesın diye kameranın parenteni null yapıyoruz.
        Camera.main.transform.SetParent(null);
        LevelKontrol.Current.GameOver();

    }
    public void CreateCylinder(float value)
    {
        //cilindir obejesini karakterimizin childı yapıyoruz
        CylinderRiding createdCylinder = Instantiate(ridingCylinderPrfab, transform).GetComponent<CylinderRiding>();
        cylinders.Add(createdCylinder);
        createdCylinder.CylinderBoyutuArtir(value);

    }
    public void DestroyCylinder(CylinderRiding cylinder)
    {
        // cylinderi yok etmek için cylinder listesinden çıkartıyoruz.
        cylinders.Remove(cylinder);
        Destroy(cylinder.gameObject);

    }
    //kopru yaratmaya baslama
    public void StartSpawnBridge(BridgeSpawner spawner)
    {
        //refenras noktalarına eşitleme
        _bridgeSpawner = spawner;
        //kopru yaratıp yaratmayan değişkeni true yapıyoruz
        _spawnBridge = true;

    }

    //kopru yapmayı durdurma

    public void StopBridge()
    {

        _spawnBridge = false;

    }
    //silindir azalma sesi
    public void DropSound()
    {

        _dropSoundTi -= Time.deltaTime;
        if (_dropSoundTi < 0)
        {
            _dropSoundTi = 0.16f;
            cylinderAudioS.PlayOneShot(dropAudioClip, 0.2f);
        }

    }

}
