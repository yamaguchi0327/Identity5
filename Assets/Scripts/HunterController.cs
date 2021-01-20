using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HunterController : MonoBehaviour
{
    float inputHorizontal;
    float inputVertical;
    Animator animator;
    Rigidbody rb;
    float defaultSpeed = 3.0f;
    public float speed;


    public GameObject myHand;
    public GameObject myLeg;
    public bool nowAttack = false; //攻撃中かどうか
    public bool nowAction = false; //カメラを固定する動作中かどうか
    public bool nowWindow = false; //乗り越え中かどうか（モーションの移動調整用）

    public bool isHit = false; //攻撃が当たったかどうか

    public string item = "non";
    public int item_id = 0;

    public GameObject itemCount;


    public int atkItem_num = 0;
    public int spdItem_num = 0;

    //----------通信------------
    public bool run_check = false;
    public Vector3 moveForward;
    public Vector3 re_move;
    public float[] stop_pos;
    public bool stop_check = false;
    public int player_No = 4;//clientと同じ

    public bool atack_motion = false;
    public bool mado_check = false;
    public bool saku_check = false;
    public bool doya_check = false;

    public string status = "normal";
    public GameObject touch_Image;//乗り越え・柵
    public Client cl;


    void Start()
    {
        moveForward = new Vector3(0, 0, 0);
        stop_pos = new float[] { 0, 0.46f, 0 };

        speed = defaultSpeed;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        if (item_id == 1)
        {
            if (spdItem_num > 0)
            {
                item = "speed";
                if (cl.player_No == 4)
                {
                    itemCount.SetActive(true);
                }
                itemCount.gameObject.transform.root.GetComponent<ItemCount>().time = 6;
                Vector3 offset = new Vector3(0f, 0.2f, 0f);
                GameObject speedFXobj = (GameObject)Instantiate(Resources.Load("SpeedFX"), transform.position + offset, new Quaternion(0f, 90f, 90f, 1f));
                speedFXobj.transform.parent = transform;
                spdItem_num--;
            }
            item_id = 0;
        }
        else if (item_id == 2)
        {
            if (atkItem_num > 0)
            {
                item = "attack";
                if (cl.player_No == 4)
                {
                    itemCount.SetActive(true);
                }
                itemCount.gameObject.transform.root.GetComponent<ItemCount>().time = 6;
                Vector3 offset = new Vector3(0f, 0.2f, 0f);
                GameObject speedFXobj = (GameObject)Instantiate(Resources.Load("AttackFX"), transform.position + offset, new Quaternion(0f, -90f, 90f, 1f));
                speedFXobj.transform.parent = transform;
                atkItem_num--;
            }
            item_id = 0;

        }

    }

    void FixedUpdate()
    {

        animator = GetComponent<Animator>();
        // カメラの方向から、X-Z平面の単位ベクトルを取得
        Vector3 cameraForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;

        // 方向キーの入力値とカメラの向きから、移動方向を決定
        moveForward = cameraForward * inputVertical + Camera.main.transform.right * inputHorizontal;
        moveForward = moveForward.normalized; //斜めが速くなってしまうので正規化 

        if (item == "speed")
        {
            moveForward *= 1.5f;
        }


        // 移動方向にスピードを掛ける。ジャンプや落下がある場合は、別途Y軸方向の速度ベクトルを足す。
        rb.velocity = re_move * speed + new Vector3(0, rb.velocity.y, 0);

        if (moveForward != Vector3.zero)
        {
            run_check = true;
        }
        else
        {
            run_check = false;
        }

        if (stop_check)
        {
            transform.position = new Vector3(stop_pos[0], stop_pos[1], stop_pos[2]);
            stop_check = false;
        }


        // キャラクターの向きを進行方向に
        if (re_move != Vector3.zero && !nowAction && !nowAttack)
        {
            transform.rotation = Quaternion.LookRotation(re_move);
            if (speed > 0)
            {
                animator.SetBool("run", true);
            }

        }
        else
        {
            animator.SetBool("run", false);
        }
        if (atack_motion)
        {
            if (!animator.GetBool("attack") && !nowAction && !nowAttack)
            {
                animator.SetBool("attack", true);
                nowAttack = true;
            }
            atack_motion = false;
        }
        else if (mado_check && !nowAction && !nowAttack)
        {
            animator.SetTrigger("window");
            nowAction = true;
            mado_check = false;
        }
        else if (saku_check && !nowAction && !nowAttack)
        {
            animator.SetTrigger("break");
            nowAction = true;
            saku_check = false;
        }

        if (nowWindow)
        {
            Vector3 playerForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            this.transform.position += playerForward * 0.015f;
        }

        //if(nowAction){
        //    Vector3 playerForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
        //    Camera.main.transform.forward = playerForward;
        //}
    }
    //当たり判定の有効化・無効化
    void vaildAttack()
    {
        myHand.gameObject.GetComponent<CapsuleCollider>().enabled = true;
    }
    void invaildAttack()
    {
        myHand.gameObject.GetComponent<CapsuleCollider>().enabled = false;
    }
    void vaildBreak()
    {
        myLeg.gameObject.GetComponent<SphereCollider>().enabled = true;
    }
    void invaildBreak()
    {
        myLeg.gameObject.GetComponent<SphereCollider>().enabled = false;
    }



    void setNowWindow(int id)
    {
        if (id == 1)
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            nowWindow = true;
        }
        else if (id == -1)
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            nowWindow = false;
        }
    }

    void setZeroSpeed()
    { //硬直
        speed = 0f;
        animator.SetBool("attack", false);
        animator.SetBool("run", false);
    }
    void setDefaultSpeed()
    { //速度再設定
        if (!doya_check)
        {
            speed = defaultSpeed;
            nowAttack = false;
        }
        else
        {
            animator.SetTrigger("DOYA");
            doya_check = false;
        }
    }
    //ドヤ顔モーション終了後呼び出し
    void finishDOYA()
    {
        isHit = false;
        nowAttack = false;
        speed = defaultSpeed;
    }

    void finishAction()
    {
        speed = defaultSpeed;
        nowAction = false;

    }
    void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "Spark")
        {
            GetComponent<Animator>().SetTrigger("stun");

        }
        if (c.gameObject.tag == "SpeedItem") spdItem_num++;
        if (c.gameObject.tag == "AttackItem") atkItem_num++;
    }
    void OnCollisionStay(Collision c)
    {
        if (c.gameObject.tag == "Window")
        {
            if (cl.player_No == player_No)
            {
                touch_Image.SetActive(true);
            }
            status = "window";
        }
        if (c.gameObject.tag == "Fence")
        {
            if (cl.player_No == player_No)
            {
                touch_Image.SetActive(true);
            }
            status = "Fence";
        }

    }
    private void OnCollisionExit(Collision c)
    {
        if (c.gameObject.tag == "Window" || c.gameObject.tag == "Fence")
        {
            touch_Image.SetActive(false);
            status = "normal";
        }
    }


}
