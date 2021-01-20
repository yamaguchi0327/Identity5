using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntersLeg : MonoBehaviour
{
    GameObject master;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.Find("Hunter");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
