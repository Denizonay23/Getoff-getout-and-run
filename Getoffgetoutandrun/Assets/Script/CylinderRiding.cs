using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderRiding : MonoBehaviour
{
    // en büyük haline gelip gelmediği
    private bool _filled;
    //cylindirin sayılsal olarak ne kadar doldugunu
    private float _value;
   
 //silindir boyutu artırma
 public void CylinderBoyutuArtir(float value)
    {

        _value += value;
        if (_value > 1)
        {
            float leftvaue = _value - 1;
            //Silindirin boyutunu tam olarak 1 yap
            int cylinderSayisi = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderSayisi - 1) - 0.25f ,transform.localPosition.z);
            transform.localScale = new Vector3(0.5f , transform.localScale.y, 0.5f);
            //1'den ne kadar büyükse o büyüklükte yeni bir silindir yarat
            PlayerController.Current.CreateCylinder(leftvaue);
        }else if (_value < 0)
        {

            //Karakterimize bu silindiri yok etmesini söyliycez
            PlayerController.Current.DestroyCylinder(this);

        }
        else
        {

            //silindirimizin boyutunu guncelleyeceğiz.
            int cylinderSayisi = PlayerController.Current.cylinders.Count;
            transform.localPosition = new Vector3(transform.localPosition.x, -0.5f * (cylinderSayisi - 1) - 0.25f * _value, transform.localPosition.z);
            transform.localScale = new Vector3(0.5f * _value, transform.localScale.y, 0.5f * _value);
        }
    }
}
