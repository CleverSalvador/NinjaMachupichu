using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class GameManager : MonoBehaviour
{
    private bool ejecutando;
    public static GameManager instance;
    public GameObject vidasUI;
    public PlayerController player;
    public Text textoMonedas;
    public int monedas;
    public Text guardarPartida;//monedas, posicion y vidas

    public GameObject panelPausa;
    public GameObject panelGameOver;
    public GameObject panelCarga;
    public GameObject panelTransicion;

    public bool avanzandoNivel;
    public int nivelActual;
    public List<Transform> posicionesAvance = new List<Transform>();
    public List<Transform> posicionesRetroceder = new List<Transform>();

    private void Awake()
    {
        if(instance == null)
             instance = this;
        else{
             Destroy(this.gameObject);
        }
        if(PlayerPrefs.GetInt("vidas") !=0)   
        CargarPartida();

    }
    public void ActivarPanelTransicion()
    {
        panelTransicion.GetComponent<Animator>().SetTrigger("ocultar");
    }
    public void CambiarPosicionJugador()//Este metodo permite cambiar la posicion del jugador al terminar el mapa
    {
        if(avanzandoNivel)
        {
            if(nivelActual +1 < posicionesAvance.Count)
            {
                //validamos que no sobrepase de los niveles que hay
                player.transform.position = posicionesAvance[nivelActual+1].transform.position;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("caminar",false);
                player.terminandoMapa = false;
            }
        }else 
        {
            if(posicionesRetroceder.Count < nivelActual - 1)
            {
                player.transform.position = posicionesRetroceder[nivelActual-1].transform.position;
                player.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
                player.GetComponent<Animator>().SetBool("caminar",false);
                player.terminandoMapa = false;
            }
                 
        }
    }
    public void GuardarPartida()
    {
        float x,y;
        x = player.transform.position.x;
        y = player.transform.position.y;

        int vidas = player.vidas;
        /*PlayerPrefs es un sistema de guardado simple, lo incorpora Unity ver documentacion*/
        PlayerPrefs.SetInt("monedas",monedas); //key/value
        PlayerPrefs.SetFloat("x",x);
        PlayerPrefs.SetFloat("y",y);
        PlayerPrefs.SetInt("vidas",vidas);
        if(!ejecutando)
        {
            StartCoroutine(MostrarTextoGuardado());
        }
    }
    public void CargarPartida()
    {
        monedas = PlayerPrefs.GetInt("monedas");
        player.transform.position = new Vector2(PlayerPrefs.GetFloat("x"), PlayerPrefs.GetFloat("y"));
        player.vidas = PlayerPrefs.GetInt("vidas");
        textoMonedas.text = monedas.ToString();
        int vidasDescontar = 3 - player.vidas;
        player.ActualizarVidasUI(vidasDescontar);
    }
    private IEnumerator MostrarTextoGuardado()
    {
        ejecutando = true;
        guardarPartida.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        guardarPartida.gameObject.SetActive(false);
        ejecutando = false;
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
