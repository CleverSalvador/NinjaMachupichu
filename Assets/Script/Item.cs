using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio; //libreria para los audios
public class Item : MonoBehaviour
{
    public GameObject pickutEfect;
    public AudioSource clip;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            AsignarItem();
        }
    }

    private void AsignarItem()
    {
        if(gameObject.CompareTag("Moneda"))
        {
            GameManager.instance.ActualizarContadorMoneda();
        }else if(gameObject.CompareTag("PowerUp"))
        {
            GameManager.instance.player.DarInmortalidad();
        }
        Instantiate(pickutEfect, transform.position, transform.rotation);
        Destroy(gameObject,0.15f);
        clip.Play();
    }
}
