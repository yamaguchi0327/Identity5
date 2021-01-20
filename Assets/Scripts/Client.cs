using UnityEngine;
using System.Collections;
using WebSocketSharp;
using WebSocketSharp.Net;

using System;

public class Client : MonoBehaviour
{
    WebSocket ws;

    public int player_No = 0; //playerNamber(0:空軍,1:医師,2:調香,3:心眼,4:ハンター)

    public HunterController ht;
    public PlayerController sv1;
    public PlayerController sv2;
    public PlayerController sv3;
    public PlayerController sv4;

    PlayerController[] svs;

    public PlayerController me;//自分
    Vector3 pos; //reposition用
    bool stop_check = true; //stopを送ったか
    bool send_check = false;

    public Crystal cr1;
    public Crystal cr2;
    public Crystal cr3;
    public Crystal cr4;
    public Crystal cr5;
    public Crystal cr6;
    public Crystal cr7;

    Crystal[] crs;

    Crystal d_cr;

    public GameObject spdImg;
    public GameObject atkImg;
    public GameObject gun;
    public GameObject chusya;

    public string ip = "localhost";



    void Start()
    {
        svs = new PlayerController[] { sv1, sv2, sv3, sv4 };
        crs = new Crystal[] { cr1, cr2, cr3, cr4, cr5, cr6, cr7 };
        d_cr = cr1;
        if (player_No <= 3)
        {
            me = svs[player_No];
            spdImg.SetActive(false);
            atkImg.SetActive(false);
        }
        if(player_No != 0){
            gun.SetActive(false);
        }
        if(player_No != 1){
            chusya.SetActive(false);
        }
        pos = new Vector3(0, 0, 0);
        ws = new WebSocket("ws://" + ip + ":3000/");

        //接続
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("WebSocket Open");
        };
        //受信
        ws.OnMessage += (sender, e) =>
        {
            String[] data = e.Data.Split(',');
            //Debug.Log(data[0]);
            if (int.Parse(data[1]) <= 3)//sv
            {
                PlayerController sv = svs[int.Parse(data[1])];
                if (data[0] == "run")
                {
                    Vector3 re_move = new Vector3(float.Parse(data[2]), float.Parse(data[3]), float.Parse(data[4]));
                    sv.re_move = re_move;
                }
                if (data[0] == "stop")
                {
                    sv.stop_pos[0] = float.Parse(data[2]);
                    sv.stop_pos[1] = float.Parse(data[3]);
                    sv.stop_pos[2] = float.Parse(data[4]);
                    sv.stop_check = true;
                    sv.re_move = Vector3.zero;
                }
                if (data[0] == "jump")
                {
                    sv.jump_check = true;
                }
                if (data[0] == "decoding")
                {
                    sv.stop_pos[0] = float.Parse(data[2]);
                    sv.stop_pos[1] = float.Parse(data[3]);
                    sv.stop_pos[2] = float.Parse(data[4]);
                    sv.stop_check = true;
                    sv.decord_check = true;
                }
                if(data[0] == "deco_finish"){
                    crs[int.Parse(data[2])].deco_check = true;
                }
                if (data[0] == "therapy")
                {
                    sv.therapy_start = true;
                    sv.thera_check = true;
                }
                if (data[0] == "thera_stop")
                {
                    sv.thera_check = false;
                }
                if (data[0] == "rescue")
                {
                    sv.res_check = true;
                }
                if (data[0] == "hit")
                {
                    sv.hit_check = false;
                    send_check = false;
                    if (ht.item == "attack")
                    {
                        sv.hp -= 1;
                    }
                    sv.hp -= 1;
                    sv.gui_check = true;
                    ht.doya_check = true;
                    
                }
                if(data[0] == "ju"){
                    sv.ju_check = true;
                }
            }
            else
            {//ht
                if (data[0] == "run")
                {
                    Vector3 re_move = new Vector3(float.Parse(data[2]), float.Parse(data[3]), float.Parse(data[4]));
                    ht.re_move = re_move;
                }
                if (data[0] == "stop")
                {
                    ht.stop_pos[0] = float.Parse(data[2]);
                    ht.stop_pos[1] = float.Parse(data[3]);
                    ht.stop_pos[2] = float.Parse(data[4]);
                    ht.stop_check = true;
                    ht.re_move = Vector3.zero;
                }
                if (data[0] == "atack")
                {
                    ht.atack_motion = true;
                }
                if (data[0] == "mado")
                {
                    ht.mado_check = true;
                }
                if (data[0] == "saku")
                {
                    ht.saku_check = true;
                }
                if (data[0] == "item")
                {
                    ht.item_id = int.Parse(data[2]);
                }
            }
        };
        //エラー
        ws.OnError += (sender, e) =>
        {
            Debug.Log("WebSocket Error: " + e.Message);
        };
        //通信終了
        ws.OnClose += (sender, e) =>
        {
            Debug.Log("WebSocket Close");
        };

        ws.Connect();

    }


    void Update()
    {
        if (player_No <= 3)
        {
            if (me.run_check)
            {
                pos = me.moveForward;

                ws.Send("run," + player_No + "," + pos.x + "," + pos.y + "," + pos.z);
                stop_check = false;
            }
            else if (!stop_check)
            {
                pos = me.transform.position;
                ws.Send("stop," + player_No + "," + pos.x + "," + pos.y + "," + pos.z);
                stop_check = true;
            }

            if (Input.GetKeyDown(KeyCode.Space) && me.status == "jump")
            {
                ws.Send("jump," + player_No);
            }

            if (Input.GetKeyDown(KeyCode.Q) && me.status == "decoding")
            {
                pos = me.transform.position;
                ws.Send("decoding," + player_No + "," + pos.x + "," + pos.y + "," + pos.z);
                d_cr = crs[me.crystal.GetComponent<Crystal>().id];
            }
            if(d_cr.deco_finish){
                ws.Send("deco_finish," + player_No + "," + d_cr.id);
                d_cr.deco_finish = false;
            }
            if (Input.GetKeyDown(KeyCode.Q) && me.status == "therapy")
            {
                ws.Send("therapy," + player_No);
            }
            if ((me.therapy_start || me.status == "re_therapy")&& Input.anyKeyDown)
            {
                ws.Send("thera_stop," + player_No);
            }
            if (Input.GetKeyDown(KeyCode.Q) && me.status == "rescue")
            {
                ws.Send("rescue," + player_No);
            }
            if (me.hit_check && !send_check)
            {
                ws.Send("hit," + player_No);
                send_check = true;
            }
            if(Input.GetKeyDown("1") && me.player_No == 0){
                ws.Send("ju,0");
            }
            if(me.status == "therapy" && Input.GetKeyDown("1") && me.player_No == 1){
                ws.Send("therapy," + player_No);
            }
        }
        else
        {
            if (ht.run_check)
            {
                pos = ht.moveForward;

                ws.Send("run," + player_No + "," + pos.x + "," + pos.y + "," + pos.z);
                stop_check = false;
            }
            else if (!stop_check || me.hp == 0)
            {
                pos = ht.transform.position;
                ws.Send("stop," + player_No + "," + pos.x + "," + pos.y + "," + pos.z);
                stop_check = true;
            }

            if (Input.GetMouseButton(0))
            {
                ws.Send("atack," + player_No);
            }
            if (Input.GetKeyDown(KeyCode.Space) && ht.status == "window")
            {
                ws.Send("mado," + player_No);
            }
            if (Input.GetKeyDown(KeyCode.Space) && ht.status == "Fence")
            {
                ws.Send("saku," + player_No);
            }
            if (Input.GetKeyDown("1"))
            {
                ws.Send("item," + player_No + "," + "1");
            }
            if (Input.GetKeyDown("2"))
            {
                ws.Send("item," + player_No + "," + "2");
            }
        }

    }

    void OnDestroy()
    {
        ws.Close();
        ws = null;
    }
}
