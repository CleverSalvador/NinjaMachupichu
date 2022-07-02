using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicaStop : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("musica").GetComponent<MusicaContinua>().StopMusic();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
