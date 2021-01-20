using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemCount : MonoBehaviour
{
    public float time;
    public HunterController hc;

    string sTex;
    public Text countText;//スタート文
    public Text atk_num_Text;
    public Text spd_num_Text;

    // Use this for initialization
    void Start()
    {
        countText.text = "00:00";

    }

    // Update is called once per frame
    void Update()
    {
        spd_num_Text.text = hc.spdItem_num.ToString();
        atk_num_Text.text = hc.atkItem_num.ToString();

        if (time >= 0 && hc.item != "non")
        {
            countText.text = "";
            time -= Time.deltaTime;//毎フレームの時間を加算.
            if (time <= 0)
            {
                hc.item = "non";
                countText.gameObject.SetActive(false);
            }
            int minute = (int)time / 60;//分.timeを60で割った値.
            int second = (int)time % 60;//秒.timeを60で割った余り.
            string minText, secText;//テキスト形式の分・秒を用意.
            if (minute < 10)
                minText = "0" + minute.ToString();//ToStringでint→stringに変換.
            else
                minText = minute.ToString();
            if (second < 10)
                secText = "0" + (second + 1).ToString();//上に同じく.
            else
                secText = second.ToString();
            countText.text = minText + ":" + secText;

        }
    }
}
