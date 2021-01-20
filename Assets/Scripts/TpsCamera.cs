using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TpsCamera : MonoBehaviour
{

    public Transform Player;
    public Client cl;
    public float RotateSpeed;

    float yaw, pitch;

    public Vector3 offset = new Vector3(0, 10f, 0);

    private void Start()
    {
        if (cl.player_No <= 3)
        {
            Player = cl.me.GetComponent<Transform>();
        }
        else
        {
            Player = cl.ht.GetComponent<Transform>();
        }
        RotateSpeed = 5;

    }

    void Update()
    {

        //プイレヤー位置を追従する
        transform.position = new Vector3(Player.position.x, Player.position.y, Player.position.z);


        yaw += Input.GetAxis("Mouse X") * RotateSpeed; //横回転入力
        pitch -= Input.GetAxis("Mouse Y") * RotateSpeed; //縦回転入力

        pitch = Mathf.Clamp(pitch, -10, 20); //縦回転角度制限する

        transform.eulerAngles = new Vector3(pitch, yaw, 0f); //回転の実行

    }
}
