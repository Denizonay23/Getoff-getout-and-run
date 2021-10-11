using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{
    //her sınıftan erişilebilmesi için
    public static LevelLoader Current;

    //önceden başka bir level var ise başka bir değişkende tutacağız
    private Scene _LastLoadedScene;

    void Start()
    {
        Current = this;
        ChangeLevel("level " + PlayerPrefs.GetInt("currentLevel"));
    }

    //Level Sahnelerini değiştirme
    public void ChangeLevel(string sceneName)
    {
        StartCoroutine(ChangeScene(sceneName));

    }

    IEnumerator ChangeScene(string sceneName)
    {
        //bu sahne şuan yuklu bir sahne mi kontrol et ?
        if (_LastLoadedScene.IsValid())
        {
            //sahneyi sil
            SceneManager.UnloadSceneAsync(_LastLoadedScene);

            //sahne silinip silinmediğini kontrol eden değişkeni tanımla
            bool sceneUnloaded = false;

            //dongu true olana kadar tekrar et
            while (!sceneUnloaded)
            {
                //sahne silinip silinmediğini kontrol et
                sceneUnloaded = !_LastLoadedScene.IsValid();

                //Oyunun bir dongusunun tamamlanmasını bekle sonra aşağıdaki işlemleri yap.
                yield return new WaitForEndOfFrame();
        
            }
            
        }

        //yeni sahneyi yükle
        SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);

        //sahnenın yuklemesının bitmesini sağlıyoruz
        bool sceneLoaded = false;

        while (!sceneLoaded)
        {
            //son yuklenen sahneyi şimdiki sahneye eşitleme
            _LastLoadedScene = SceneManager.GetSceneByName(sceneName);

            //eğer çektiğimiz sahne boş ise kontrolünü sağlıyoruz.
            sceneLoaded = _LastLoadedScene != null && _LastLoadedScene.isLoaded;

            //Oyunun bir dongusunun tamamlanmasını bekle sonra aşağıdaki işlemleri yap.
            yield return new WaitForEndOfFrame();

        }

    }
   
}
