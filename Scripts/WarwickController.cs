using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class WarwickAnim
{
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip walkback;
    public AnimationClip run;
    public AnimationClip attack;
    public AnimationClip trapped;
    public AnimationClip joke;
    public AnimationClip boost;
}

enum wwAnim
{
    idle,
    walk,
    walkback,
    run,
    attack,
    trapped,
    joke,
    boost
}

public class WarwickController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector] public PhotonView PV = null;
    public static WarwickController Inst = null;

    float h, v, accel = 1;
    float moveSpeed = 17.0f;
    private float CamRotSpeed = 2.5f;
    public WarwickAnim anim;
    [HideInInspector] public Animation _animation;
    wwAnim CurwwAnim = wwAnim.idle;
    wwAnim ExwwAnim = wwAnim.idle;
    Rigidbody rb;
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;
    [HideInInspector] public List<GameObject> TriggeredTeemo = new List<GameObject>();

    [HideInInspector] public bool Stunned = false;
    [HideInInspector] public bool Trapped = false;
    [HideInInspector] public float timer = 0.0f;

    RaycastHit hit;
    float MaxDistance = 150.0f;

    bool Attackable = false;
    bool isAttacking = false;
    bool isJoking = false;

    float SoundTimer = 0.0f;
    bool StopSound = false;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        PV.ObservedComponents[0] = this;
        Inst = this;

        if (PV.IsMine)
        {
            GameObject CameraCon = GameObject.Find("CameraContainer");
            CameraController CameraContrl = CameraCon.GetComponent<CameraController>();
            CameraContrl.InitCamera(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        _animation = GetComponentInChildren<Animation>();
        _animation.clip = anim.idle;
        _animation.Play();
        rb = this.gameObject.GetComponent<Rigidbody>();
        InGameMgr.Inst.heroCount++;
        if (PV.IsMine)
            SoundManager.Instance.PlayEffSound("Warwick_Start", Vector3.zero);
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        if (PV.IsMine)
        {
            if (Trapped)
            {
                timer -= Time.deltaTime;
                CurwwAnim = wwAnim.trapped;
                if (timer <= 0)
                {
                    timer = 0;
                    Trapped = false;
                }
            }
            else if (Stunned)
            {
                if (!StopSound)
                {
                    SoundManager.Instance.PlayEffSound("Warwick_Hurt", Vector3.zero);
                    if (PV != null)
                        PV.RPC("DelieverSound", RpcTarget.Others, "Warwick_Hurt");
                }
                StopSound = true;
                timer -= Time.deltaTime;
                CurwwAnim = wwAnim.trapped;
                if (timer <= 0)
                {
                    timer = 0;
                    Stunned = false;
                    StopSound = false;
                }
            }
            else
            {
                if(Physics.Raycast(transform.position,transform.forward,out hit, MaxDistance))
                {
                    if (hit.transform.tag == "TEEMO" && !isAttacking && !isJoking)
                    {
                        if (TriggeredTeemo.Contains(hit.transform.gameObject))
                        {
                            if (!hit.transform.gameObject.GetComponent<TeemoController>().isDead)
                            {
                                Attackable = true;
                                InGameMgr.Inst.AttackTxt.gameObject.SetActive(true);
                            }
                            else
                            {
                                Attackable = false;
                                InGameMgr.Inst.AttackTxt.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            Attackable = false;
                            InGameMgr.Inst.AttackTxt.gameObject.SetActive(false);
                        }
                    }
                    else
                    {
                        Attackable = false;
                        InGameMgr.Inst.AttackTxt.gameObject.SetActive(false);
                    }

                }

                if (Attackable && Input.GetMouseButtonDown(0) && !isJoking)
                {
                    hit.transform.gameObject.GetComponent<TeemoController>().DeadFunc();
                    isAttacking = true;
                    timer = 0.5f;
                    JailDoorMgr.Inst.DoorSystem(false);
                }
                else if(isAttacking)
                {
                    if (!StopSound)
                    {
                        SoundManager.Instance.PlayEffSound("Warwick_Attack", Vector3.zero);
                        if (PV != null)
                            PV.RPC("DelieverSound", RpcTarget.Others, "Warwick_Attack1");
                    }
                    StopSound = true;
                    timer -= Time.deltaTime;
                    CurwwAnim = wwAnim.attack;
                    if (timer <= 0)
                    {
                        isAttacking = false;
                        StopSound = false;
                    }                        
                }
                else if (!isJoking && Input.GetKeyDown(KeyCode.T))
                {
                    isJoking = true;
                    timer = 4.0f;
                }
                else if (isJoking)
                {
                    if (!StopSound)
                    {
                        SoundManager.Instance.PlayEffSound("Warwick_Joke", Vector3.zero);
                        if (PV != null)
                            PV.RPC("DelieverSound", RpcTarget.Others, "Warwick_Joke");
                    }
                    StopSound = true;    
                    timer -= Time.deltaTime;
                    CurwwAnim = wwAnim.joke;
                    if (timer <= 0)
                    {
                        isJoking = false;
                        StopSound = false;
                    }                        
                }
                else
                {
                    SoundTimer += Time.deltaTime;
                    //----- 키보드 이동 구현
                    if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
                        v = 1;
                    else if (!Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
                        v = -1;
                    else
                        v = 0;

                    if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
                        h = -1;
                    else if (!Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
                        h = 1;
                    else
                        h = 0;

                    if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W))
                        accel = 3;
                    else
                        accel = 1;
                    //----- 키보드 이동 구현

                    //----- 애니메이션 구현
                    if (accel > 1 && v == 1)
                        CurwwAnim = wwAnim.run;
                    else if (v == -1)
                        CurwwAnim = wwAnim.walkback;
                    else if (v == 0 && h == 0)
                        CurwwAnim = wwAnim.idle;
                    else
                        CurwwAnim = wwAnim.walk;
                    //----- 애니메이션 구현

                    Vector3 MoveStep = (transform.forward * v + transform.right * h).normalized * moveSpeed * accel + new Vector3(0, -9.81f, 0);
                    //transform.position += MoveStep;
                    rb.velocity = MoveStep;

                    if (!Input.GetKey(KeyCode.LeftAlt))
                    {
                        Vector3 PlayerAngle = this.transform.rotation.eulerAngles;
                        float Y_Change = PlayerAngle.y + Input.GetAxis("Mouse X") * CamRotSpeed;

                        transform.rotation = Quaternion.Euler(PlayerAngle.x, Y_Change, PlayerAngle.z);
                    }

                    if ((int)SoundTimer % 5 == 0 && (int)SoundTimer != 0)
                    {
                        if (!StopSound)
                        {
                            string _SoundName = "Warwick_Idle" + Random.Range(1, 5).ToString();
                            SoundManager.Instance.PlayEffSound(_SoundName, Vector3.zero);
                        }
                        StopSound = true;
                    }
                    else
                        StopSound = false;
                }                
            }
        }
        else
        {
            if (10.0f < (transform.position - currPos).magnitude)
                transform.position = currPos;
            else
                transform.position = Vector3.Lerp(transform.position, currPos, Time.deltaTime * 10.0f);
            transform.rotation = Quaternion.Slerp(transform.rotation, currRot, Time.deltaTime * 10.0f);
        }

        if (ExwwAnim != CurwwAnim)
        {
            ExwwAnim = CurwwAnim;
            AnimFunc(CurwwAnim);
        }
    }

    void AnimFunc(wwAnim _anim)
    {
        switch (_anim)
        {
            case wwAnim.idle:
                _animation.CrossFade(anim.idle.name, 0.1f);
                break;

            case wwAnim.walk:
                _animation.CrossFade(anim.walk.name, 0.1f);
                break;

            case wwAnim.walkback:
                _animation.CrossFade(anim.walkback.name, 0.1f);
                break;

            case wwAnim.run:
                _animation.CrossFade(anim.run.name, 0.1f);
                break;

            case wwAnim.attack:
                _animation.CrossFade(anim.attack.name, 0.1f);
                break;

            case wwAnim.trapped:
                _animation.CrossFade(anim.trapped.name, 0.1f);
                break;

            case wwAnim.joke:
                _animation.CrossFade(anim.joke.name, 0.1f);
                break;

            case wwAnim.boost:
                _animation.CrossFade(anim.boost.name, 0.1f);
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //로컬 플레이어의 위치 정보 송신
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext((int)CurwwAnim);
        }
        else //원격 플레이어의 위치 정보 수신
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            CurwwAnim = (wwAnim)stream.ReceiveNext();
        }
    }

    [PunRPC]
    void JokeSound(string SoundName)
    {
        SoundManager.Instance.PlayEffSound(SoundName, this.transform.position);
    }
}
