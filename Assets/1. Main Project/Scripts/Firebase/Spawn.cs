using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawn : MonoBehaviour
{

    [SerializeField]  public  GameObject Scene;

    public void Spawnear()
    {
        Scene.SetActive(true);
    }

   
}
