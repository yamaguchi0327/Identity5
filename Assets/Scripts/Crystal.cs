using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crystal : MonoBehaviour
{
    public Material endMterial;
    GameObject gate;

    public int deco_time;
    public int deco_max;
    public bool deco_start = false;
    public bool deco_finish = false;
    public bool deco_check = false;
    public GameObject timeSlider;
    public GameObject sv;
    public Client cl;
    int start_fast;
    public int id = 0;
    // Start is called before the first frame update
    void Start()
    {
        deco_max = 1500;
        deco_time = 0;
        start_fast = 0;
        gate = GameObject.Find("Gate");
    }

    // Update is called once per frame
    void Update()
    {
        if (deco_start)
        {
            deco_time += 1;
            if (sv.GetComponent<PlayerController>().player_No == 3)
            {
                deco_time += 1;
            }
            start_fast += 1;
            if (sv.GetComponent<PlayerController>().player_No == cl.player_No)
            {
                timeSlider.SetActive(true);
            }
            if (deco_time >= deco_max)
            {
                start_fast = 0;
                deco_start = false;
                deco_finish = true;
                timeSlider.SetActive(false);
            }
            if (start_fast >= 10 && Input.anyKeyDown)
            {
                deco_start = false;
                timeSlider.SetActive(false);
                start_fast = 0;
            }
        }
        if(deco_check){
            deco_check = false;

            sv.gameObject.GetComponent<Animator>().SetBool("dig", false);
            colorChange();
            Vector3 offset = new Vector3(0, -0.5f, 0);
            Instantiate(Resources.Load("CrystalLight"), transform.position + offset, new Quaternion(0f, 90f, 90f, 1f));
            gameObject.tag = "DeadCrystal";
            gate.GetComponent<Gate>().checkCrystalCount();
            sv.gameObject.GetComponent<PlayerController>().kaidoku_Image.SetActive(false);
        }
    }


    public void colorChange()
    {
        this.GetComponent<Renderer>().material = endMterial;
    }

    void OnCollisionEnter(Collision hit)
    {
        if (hit.gameObject.tag == "Survivor")
        {
            sv = hit.gameObject;
        }
    }

}
