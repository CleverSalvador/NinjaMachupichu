using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;//Cambio de escena probar
public class DeteccionFinal : MonoBehaviour
{
    public bool avanzando;
    public GameObject textLevelCompletado;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("detectando al jugador");
            GameManager.instance.ActivarPanelTransicion();
            GameManager.instance.avanzandoNivel = avanzando;
            StartCoroutine(EsperarCambioPosicion());
            //SceneManager.LoadScene("LevelSelect");//probando
        }
    }

    private IEnumerator EsperarCambioPosicion()
    {
        textLevelCompletado.SetActive(true);
        yield return new WaitForSeconds(0.15f);
        GameManager.instance.CambiarPosicionJugador();
        if(avanzando)
            GameManager.instance.nivelActual++;
        else
            GameManager.instance.nivelActual--;
    }
}
