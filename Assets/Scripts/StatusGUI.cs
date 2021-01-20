using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusGUI : MonoBehaviour
{
    public GameObject[] icon_well;
    public GameObject[] icon_hurt;
    public GameObject[] icon_down;
    public GameObject[] icon_dead;
    public GameObject[] fill;

    public GameObject player;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 4;i++){
            fill[i].GetComponent<Image>().fillAmount = player.GetComponent<PlayerController>().down_time / 1300f;

        }


    }

    public void StatusIconCheck(int _id,int _hp){
        if(_hp==2){
            icon_well[_id].SetActive(true);
            icon_hurt[_id].SetActive(false);
        }
        else if(_hp==1){
            icon_well[_id].SetActive(false);
            icon_hurt[_id].SetActive(true);
            icon_down[_id].SetActive(false);
            fill[_id].SetActive(false);
        }
        else if(_hp==0){
            icon_well[_id].SetActive(false);
            icon_hurt[_id].SetActive(false);
            icon_down[_id].SetActive(true);
            fill[_id].SetActive(true);
        }
        else if (_hp == -1)
        {
            icon_well[_id].SetActive(false);
            icon_hurt[_id].SetActive(false);
            icon_down[_id].SetActive(false);
            fill[_id].SetActive(false);
            icon_dead[_id].SetActive(true);

        }
    }
}
