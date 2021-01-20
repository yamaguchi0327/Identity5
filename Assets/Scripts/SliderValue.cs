using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    Slider timeSlider;
    PlayerController pc;
    public PlayerController hurt_sv;
    public Client cl;

    float max;
    float now;

    // Start is called before the first frame update
    void Start()
    {
        pc = cl.me;
        if (pc.status == "re_therapy" || pc.status == "therapy")
        {
            max = hurt_sv.therapy_max;
            now = hurt_sv.therapy_time;
        }
        else if (pc.status == "decoding")
        {
            max = pc.crystal.GetComponent<Crystal>().deco_max;
            now = pc.crystal.GetComponent<Crystal>().deco_time;
        }
        timeSlider = GetComponent<Slider>();
        timeSlider.maxValue = max;
    }

    // Update is called once per frame
    void Update()
    {
        if (pc.status == "re_therapy" || pc.status == "therapy")
        {
            now = hurt_sv.therapy_time;
        }
        else if (pc.status == "decoding")
        {
            now = pc.crystal.GetComponent<Crystal>().deco_time;
        }
        timeSlider.value = now;
    }
}
