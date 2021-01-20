using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HuntersHand : MonoBehaviour
{
    GameObject master;
    

    // Start is called before the first frame update
    void Start()
    {
        master = GameObject.Find("Hunter");

    }

    // Update is called once per frame
    void Update()
    {

        
    }

    private void OnTriggerEnter(Collider c)
    {
        if(c.gameObject.tag == "Survivor"){
            master.gameObject.GetComponent<HunterController>().isHit = true;

        }
    }
}
