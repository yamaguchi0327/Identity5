using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public int player_No = 0;
    float inputHorizontal;
    float inputVertical;
    Animator animator;
    Rigidbody rb;


    float defaultSpeed = 2.8f;
    public float speed;

    public bool nowAction = false;
    public bool nowJump = false; //ジャンプ中かどうか（モーションの移動調整用）


    //-------みのりちゃん--------
    public int hp = 2;
    public string status = "normal";
    PlayerController sv_hurt; //治療対象
    public int therapy_time; //現在の治療進行度
    public int therapy_max; //治療時間
    int start_fast; //進行止める用に必要
    public bool therapy_start = false; //治療中かどうか

    public GameObject timeSlider;
    public GameObject crystal;

    public int down_time = 0;
    PlayerController sv_res;//救助対象


    //-------------------------

    //-------ななちゃん--------
    public GameObject touch_Image;//乗り越え・柵
    public GameObject kaidoku_Image; //解読
    public GameObject rescure_Image; //救助


    public GameObject canvas;

    //-------通信--------
    public bool run_check = false;
    public Vector3 moveForward;
    public Vector3 re_move;
    public bool jump_check = false;
    public bool decord_check = false;
    public bool thera_check = false;
    public bool res_check = false;
    public bool hit_check = false;
    public bool stop_check = false;
    public bool gui_check = false;
    public float[] stop_pos;
    public bool ju_check = false;

    public Client cl;
    public Gate gt;


    void Start()
    {
        moveForward = new Vector3(0, 0, 0);
        speed = defaultSpeed;
        rb = GetComponent<Rigidbody>();
        therapy_time = 0;
        therapy_max = 500;
        timeSlider.SetActive(false);
        stop_pos = new float[] { 0, 0.46f, 0 };
    }

    void Update()
    {
        animator = GetComponent<Animator>();
        //Debug.Log(speed);

        inputHorizontal = Input.GetAxisRaw("Horizontal");
        inputVertical = Input.GetAxisRaw("Vertical");

        if (player_No == 1 && hp == 1 && status == "normal")
        {
            status = "therapy";
            sv_hurt = this;
            timeSlider.GetComponent<SliderValue>().hurt_sv = sv_hurt;
        }
        //治療
        //if (thera_check)
        //{
        //    therapy_start = true;
        //    timeSlider.GetComponent<SliderValue>().hurt_sv = sv_hurt;
        //    thera_check = false;
        //}
        if (therapy_start)
        {
            animator.SetBool("therapy", true);
            sv_hurt.status = "re_therapy";
            start_fast += 1;
            sv_hurt.therapy_time += 1;
            if (player_No == cl.player_No || sv_hurt.player_No == cl.player_No)
            {
                timeSlider.SetActive(true);
            }
            if (sv_hurt.therapy_time >= therapy_max)
            {
                Vector3 offset = new Vector3(0, 1f, 0);
                Instantiate(Resources.Load("Heart"), transform.position + offset, Quaternion.identity);
                animator.SetBool("therapy", false);
                therapy_start = false;
                sv_hurt.hp += 1;
                sv_hurt.therapy_time = 0;
                start_fast = 0;
                timeSlider.SetActive(false);
                status = "normal";
                sv_hurt.status = "normal";
                canvas.GetComponent<StatusGUI>().StatusIconCheck(sv_hurt.player_No, sv_hurt.hp);
                speed = defaultSpeed;
            }
            if (start_fast >= 10 && !thera_check)
            {
                animator.SetBool("therapy", false);
                therapy_start = false;
                timeSlider.SetActive(false);
                start_fast = 0;
                status = "normal";
                sv_hurt.status = "normal";
                speed = defaultSpeed;
            }
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
        // キャラクターの向きを進行方向に
        if (re_move != Vector3.zero && !nowAction)
        {
            transform.rotation = Quaternion.LookRotation(re_move);
            if(hp==2) animator.SetBool("run", true);
            else if(hp==1) animator.SetBool("run2", true);

        }
        else
        {
            animator.SetBool("run", false);
            animator.SetBool("run2", false);
        }
        if (stop_check)
        {
            transform.position = new Vector3(stop_pos[0], stop_pos[1], stop_pos[2]);
            stop_check = false;
        }

        if (jump_check)
        {
            animator.SetTrigger("jump");
            nowAction = true;
            jump_check = false;
        }

        if (nowJump)
        {
            Vector3 playerForward = Vector3.Scale(transform.forward, new Vector3(1, 0, 1)).normalized;
            this.transform.position += playerForward * 0.04f;
        }
        if (res_check)
        {
            nowAction = true;
            animator.SetTrigger("rescue");
            res_check = false;
            sv_res.hp += 1;
            sv_res.speed = defaultSpeed;
            canvas.GetComponent<StatusGUI>().StatusIconCheck(sv_res.player_No, sv_res.hp);
        }

        if (ju_check)
        {
            if(Vector3.Distance(GameObject.Find("Hunter").transform.position,transform.position)<3f){
                transform.LookAt(GameObject.Find("Hunter").transform);
                animator.SetTrigger("item");
                nowAction = true;
                ju_check = false;
            }

        }
        if (decord_check)
        {
            animator.SetBool("dig", true);
            crystal.GetComponent<Crystal>().deco_start = true;
            kaidoku_Image.SetActive(false);
            decord_check = false;
        }
        if (!crystal.GetComponent<Crystal>().deco_start && status == "decoding")
        {
            status = "nomal";
        }
        if(gui_check){
            canvas.GetComponent<StatusGUI>().StatusIconCheck(player_No, hp);
            gui_check = false;
        }

        ////-------みのりちゃん--------
        ////救助
        //if (Input.GetKeyDown(KeyCode.Q) && status == "rescure")
        //{
        //    animator.SetTrigger("rescure");
        //    thrapy_start = true;

        //}
        //if (thrapy_start)
        //{
        //    start_fast += 5;
        //    therapy_time += 5;
        //    timeSlider.SetActive(true);
        //    if (therapy_time >= therapy_max)
        //    {
        //        thrapy_start = false;
        //        GameObject.Find("Survivor" + thrapy_id).GetComponent<PlayerController>().hp += 1;
        //        GameObject.Find("Survivor" + thrapy_id).GetComponent<PlayerController>().speed = defaultSpeed;
        //        therapy_time = 0;
        //        start_fast = 0;
        //        timeSlider.SetActive(false);
        //        status = "nomal";
        //        canvas.GetComponent<StatusGUI>().StatusIconCheck(thrapy_id, GameObject.Find("Survivor" + thrapy_id).GetComponent<PlayerController>().hp);
        //    }
        //    if (start_fast >= 10 && Input.anyKeyDown)
        //    {
        //        thrapy_start = false;
        //        timeSlider.SetActive(false);
        //        start_fast = 0;
        //        status = "nomal";
        //    }
        //}

        //down
        if (hp == 0)
        {
            if(Random.Range(0,30)<1){
                Vector3 offset = new Vector3(Random.Range(-0.5f,0.5f), 1.2f, Random.Range(-0.5f, 0.5f));
                Instantiate(Resources.Load("StunStar"), transform.position + offset, Quaternion.identity);
            }
            speed = 0;
            down_time += 1;
            animator.SetBool("down", true);
        }
        else
        {
            animator.SetBool("down", false);
            //speed = defaultSpeed;　//消してね
        }

        if (down_time >= 1400)
        {
            hp = -1;
            Debug.Log("game over");
            canvas.GetComponent<StatusGUI>().StatusIconCheck(player_No, hp);
            gameObject.SetActive(false);
            gt.dead += 1;

        }
        //-------------------------

        if (Input.GetKey("9")){
            animator.SetTrigger("idle2");
        }
        if (Input.GetKey("0"))
        {
            animator.SetTrigger("idle3");
        }


    }

    void setNowJump(int id)
    {

        if (id == 1)
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = false;
            this.gameObject.GetComponent<Rigidbody>().useGravity = false;
            nowJump = true;
        }
        else if (id == -1)
        {
            this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
            this.gameObject.GetComponent<Rigidbody>().useGravity = true;
            nowJump = false;
        }
    }
    void setZeroSpeed()
    { //硬直
        Debug.Log("aaa");
        speed = 0f;
    }

    void finishAction()
    {
        speed = defaultSpeed;
        nowAction = false;
        status = "normal";
        touch_Image.SetActive(false);

    }
    void initSpark()
    {
        Vector3 target = GameObject.Find("Hunter").transform.position + new Vector3(0,1f,0);
        //Vector3 offset = transform.forward*2f + new Vector3(0, 1f, 0);
        Instantiate(Resources.Load("SparkFX"), target, Quaternion.identity);
    }

    private void OnTriggerEnter(Collider c)
    {
        if (c.gameObject.tag == "HuntersHand" && hp > 0)
        {
            //if(GameObject.Find("Hunter").GetComponent<HunterController>().item=="attack"){
            //    this.GetComponent<Animator>().SetBool("down",true);
            //    //hp -= 2;
            //}
            //else{
            //if(hp==1){
            //    this.GetComponent<Animator>().SetBool("down", true);
            //}
            //else{
            //    this.GetComponent<Animator>().SetTrigger("hit");
            //}
            //hp -= 1;
            //}
            this.GetComponent<Animator>().SetTrigger("hit");
            hit_check = true;
            therapy_time = 0;
            //canvas.GetComponent<StatusGUI>().StatusIconCheck(player_No,hp);
        }
        if(c.gameObject.tag == "Gate"){
            status = "survive";
            gt.survive += 1;
            speed = 0;
        }
    }

    void OnCollisionStay(Collision c)
    {
        if (c.gameObject.tag == "Crystal")
        {
            crystal = c.gameObject;
            //Debug.Log(crystal);
            status = "decoding";
            if (!crystal.GetComponent<Crystal>().deco_start && cl.player_No == player_No)
            {
                kaidoku_Image.SetActive(true);
            }

        }
        if (c.gameObject.tag == "Window" || c.gameObject.tag == "Fence")
        {
            if (cl.player_No == player_No)
            {
                touch_Image.SetActive(true);
            }
            status = "jump";
        }
        if (c.gameObject.tag == "Survivor" && c.gameObject.GetComponent<PlayerController>().hp == 1)
        {
            if (cl.player_No == player_No)
            {
                rescure_Image.SetActive(true);
            }
            sv_hurt = c.gameObject.GetComponent<PlayerController>();
            status = "therapy";
            timeSlider.GetComponent<SliderValue>().hurt_sv = sv_hurt;
        }
        if (c.gameObject.tag == "Survivor" && c.gameObject.GetComponent<PlayerController>().hp == 0)
        {
            if (cl.player_No == player_No)
            {
                rescure_Image.SetActive(true);
            }
            sv_res = c.gameObject.GetComponent<PlayerController>();
            status = "rescue";
        }

    }

    private void OnCollisionExit(Collision c)
    {
        if (c.gameObject.tag == "Crystal")
        {
            status = "normal";
            kaidoku_Image.SetActive(false);
            animator.SetBool("dig", false);
        }
        if (c.gameObject.tag == "Window" || c.gameObject.tag == "Fence")
        {
            touch_Image.SetActive(false);
            status = "normal";
        }
        if (c.gameObject.tag == "Survivor")
        {
            rescure_Image.SetActive(false);
            status = "normal";
        }

    }
}
