using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    //baslangiç ve bitiş referansları
    public GameObject startReference, endReference;
   
    //box collideri konumlandırmak ve boyultlandırmak için tanımlıyoruz
    public BoxCollider hiddenPlatform;

    
    void Start()
    {

        Vector3 direction = endReference.transform.position - startReference.transform.position;
        //magnitu yon vektörünün ağırlığı
        float distance = direction.magnitude;
        //birim vektore donusturdum
        direction = direction.normalized;
        // iki referans noktanın yerı degısırse colliderinde degismesi gerekiyor.
        hiddenPlatform.transform.forward = direction;
        hiddenPlatform.size = new Vector3(hiddenPlatform.size.x,hiddenPlatform.size.y,distance);

        // Collideri iki referans noktasının arasına getirdik.
        hiddenPlatform.transform.position = startReference.transform.position + (direction * distance / 2) + (new Vector3(0,-direction.z,direction.y) * hiddenPlatform.size.y / 2);

    }

}
