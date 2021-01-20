using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Gate : MonoBehaviour
{
    public GameObject[] crystal;
    public static int crystalCount = 5;
    public GameObject crystalText;

    public int dead = 0;//脱落者
    public int survive = 0;//生存者
    public bool game_end = false;
    public string result = "duce";
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(crystalCount>0){
            crystalText.GetComponent<Text>().text = crystalCount + "個のクリスタルが発掘されていません。";
        }
        if(dead + survive == 4){
            game_end = true;
            if(dead>survive){
                result = "hunter";
            }else if(dead<survive){
                result = "surviver";
            }
        }

    }
    public void  checkCrystalCount(){
        crystalCount--;
        if(crystalCount==0){
            // ゲームオブジェクトの子のTransformを列挙
            foreach (Transform transform in gameObject.transform)
            {
                // Transformからゲームオブジェクト取得・削除
                GameObject door = transform.GetChild(0).gameObject;
                Destroy(door);
                crystalText.GetComponent<Text>().text = "ゲートが開放されました";
            }
            foreach (GameObject obj in crystal)
            {
                obj.gameObject.GetComponent<Crystal>().colorChange();
            }
        }

    }
}
