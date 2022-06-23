using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject vidasUI;
    public PlayerController player;
    public Text textoMonedas;
    public int monedas;
    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelCarga;
    private void Awake()
    {
        if(instance == null)
             instance = this;
        else
            Destroy(this.gameObject);

    }

    public void ActualizarContadorMoneda()
    {
        monedas++;
        textoMonedas.text = monedas.ToString();
    }

    public void PausarJuego()
    {
        Time.timeScale = 0;
        panelPausa.SetActive(true);
    }

    public void DespausarJuego()
    {
        Time.timeScale = 1;
        panelPausa.SetActive(false);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }

    public void CargarSelectorNiveles()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("LevelSelect");
    }
    public void CargarEscena(string escenaCargar )
    {
        SceneManager.LoadScene(escenaCargar);
    }
    public void GameOver()
    {
        panelGameOver.SetActive(true);
    }
    public void SalirDelJuego()
    {
        Application.Quit();
    }

    public void CargarSceneSelect()
    {
        StartCoroutine(CargarScene());
    }
    private IEnumerator CargarScene()
    {
        panelCarga.SetActive(true);

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("LevelSelect");
        while(!asyncLoad.isDone)
        {
            yield return new WaitForSeconds(30);
        }
    }
}
