using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

[System.Serializable]
public class TeemoAnim
{
    public AnimationClip idle;
    public AnimationClip walk;
    public AnimationClip walkback;
    public AnimationClip death;
    public AnimationClip joke;
    public AnimationClip channeling;
    public AnimationClip attack;
}

enum tmAnim
{
    idle,
    walk,
    walkback,
    death,
    joke,
    channeling,
    attack
}

public class TeemoController : MonoBehaviourPunCallbacks, IPunObservable
{
    [HideInInspector] public PhotonView PV = null;
    public static TeemoController Inst = null;

    float h, v, accel = 1;
    float moveSpeed = 15.0f;
    private float CamRotSpeed = 3.0f;
    public TeemoAnim anim;
    [HideInInspector] public Animation _animation;
    tmAnim CurtmAnim = tmAnim.idle;
    tmAnim ExtmAnim = tmAnim.idle;
    Rigidbody rb;
    Vector3 currPos = Vector3.zero;
    Quaternion currRot = Quaternion.identity;
    float timer = 0.0f;

    bool isJoking = false;
    bool AttackAble = false;
    bool isAttacking = false;
    //bool DoorChannelingAble = false;
    [HideInInspector] public bool isDead = false;    

    RaycastHit hit;
    float MaxDistance = 150.0f;
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
            SoundManager.Instance.PlayEffSound("Teemo_Start", Vector3.zero);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (PV.IsMine)
        {
            if (isDead)
            {
                if (!StopSound)
                {
                    SoundManager.Instance.PlayEffSound("Teemo_Hurt", Vector3.zero);
                    if (PV != null)
                        PV.RPC("DelieverSound", RpcTarget.Others, "Teemo_Hurt");
                }
                StopSound = true;
                timer -= Time.deltaTime;
                CurtmAnim = tmAnim.death;
                if (timer <= 0)
                {
                    isDead = false;
                    this.transform.position = InGameMgr.Inst.SpawnPoint[0];
                    StopSound = false;
                }                    
            }
            else if (InGameMgr.Inst.isExit)
            {
                InGameMgr.Inst.DeliverMeExit();
                InGameMgr.Inst.GameOverFunc(true);
                PV.RPC("DestroyObj", RpcTarget.All);
            }
            else
            {
                if (Physics.Raycast(transform.position, transform.forward, out hit, MaxDistance))
                {
                    if (hit.transform.tag == "ITEM")
                    {
                        if (InGameMgr.Inst.TriggeredItem.Contains(hit.transform.gameObject))
                        {
                            InGameMgr.Inst.ChannelingAble = true;
                            if (InGameMgr.Inst.TargetItem == null)
                                InGameMgr.Inst.TargetItem = hit.transform.gameObject;
                        }
                        else
                        {
                            InGameMgr.Inst.ChannelingAble = false;
                            InGameMgr.Inst.TargetItem = null;
                        }
                    }
                    else if (hit.transform.tag == "WARWICKTRG" && InGameMgr.Inst.PoisonedDart != 0 && !isAttacking && !isJoking)
                    {
                        AttackAble = true;
                        InGameMgr.Inst.AttackTxt.gameObject.SetActive(true);
                    }
                    else if (hit.transform.tag == "JAILDOOR" && InGameMgr.Inst.DoorTriggered && !isAttacking && !isJoking)
                    {
                        InGameMgr.Inst.DoorChannelingAble = true;
                    }
                    else if (hit.transform.tag == "EXITDOOR" && InGameMgr.Inst.ExitTriggered && !isAttacking && !isJoking)
                    {
                        InGameMgr.Inst.ExitChannlingAble = true;
                    }
                    else
                    {
                        InGameMgr.Inst.ChannelingAble = false;
                        AttackAble = false;
                        InGameMgr.Inst.TargetItem = null;
                        InGameMgr.Inst.AttackTxt.gameObject.SetActive(false);
                    }
                }

                if (InGameMgr.Inst.ChannelingAble && Input.GetKey(KeyCode.F) && !isJoking)
                {
                    InGameMgr.Inst.Channeling = true;
                    CurtmAnim = tmAnim.channeling;
                }
                else if (InGameMgr.Inst.DoorChannelingAble && Input.GetKey(KeyCode.F) && !isJoking &&
                    InGameMgr.Inst.DoorKey != 0)
                {
                    InGameMgr.Inst.Channeling = true;
                    CurtmAnim = tmAnim.channeling;
                }
                else if (InGameMgr.Inst.ExitChannlingAble && Input.GetKey(KeyCode.F) && !isJoking &&
                    InGameMgr.Inst.ExitKey != 0)
                {
                    InGameMgr.Inst.Channeling = true;
                    CurtmAnim = tmAnim.channeling;
                }
                else if (!isJoking && Input.GetKeyDown(KeyCode.T))
                {
                    isJoking = true;
                    timer = 2.0f;
                }
                else if (isJoking)
                {
                    if (!StopSound)
                    {
                        SoundManager.Instance.PlayEffSound("Teemo_Joke", Vector3.zero);
                        if (PV != null)
                            PV.RPC("DelieverSound", RpcTarget.Others, "Teemo_Joke");
                    }
                    StopSound = true;
                    timer -= Time.deltaTime;
                    CurtmAnim = tmAnim.joke;
                    if (timer <= 0)
                    {
                        StopSound = false;
                        isJoking = false;
                    }
                }
                else if (AttackAble && Input.GetMouseButtonDown(0) && InGameMgr.Inst.PoisonedDart > 0)
                {
                    InGameMgr.Inst.PoisonedDartFunc();
                    InGameMgr.Inst.PoisonedDart--;
                    InGameMgr.Inst.RefreshSkillUI();
                    isAttacking = true;
                    timer = 0.5f;
                }
                else if (isAttacking)
                {
                    if (!StopSound)
                    {
                        SoundManager.Instance.PlayEffSound("Teemo_Trap", Vector3.zero);
                        if (PV != null)
                            PV.RPC("DelieverSound", RpcTarget.Others, "Teemo_Trap");
                    }
                    StopSound = true;
                    timer -= Time.deltaTime;
                    CurtmAnim = tmAnim.attack;
                    if (timer <= 0)
                    {
                        StopSound = false;
                        isAttacking = false;
                    }
                }
                else
                {
                    InGameMgr.Inst.Channeling = false;
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
                        accel = 2;
                    else
                        accel = 1;
                    //----- 키보드 이동 구현

                    //----- 애니메이션 구현
                    if (accel > 1 && v == 1)
                        CurtmAnim = tmAnim.walk;
                    else if (v == -1)
                        CurtmAnim = tmAnim.walkback;
                    else if (v == 0 && h == 0)
                        CurtmAnim = tmAnim.idle;
                    else
                        CurtmAnim = tmAnim.walk;
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
                            string _SoundName = "Teemo_Attack";
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

            if (isDead)
            {
                timer -= Time.deltaTime;
                if (timer <= 0)
                    isDead = false;
            }
        }

        if (ExtmAnim != CurtmAnim)
        {
            ExtmAnim = CurtmAnim;
            AnimFunc(CurtmAnim);
        }
    }

    void AnimFunc(tmAnim _anim)
    {
        switch (_anim)
        {
            case tmAnim.idle:
                _animation.CrossFade(anim.idle.name, 0.1f);
                break;

            case tmAnim.walk:
                _animation.CrossFade(anim.walk.name, 0.1f);
                break;

            case tmAnim.walkback:
                _animation.CrossFade(anim.walkback.name, 0.1f);
                break;

            case tmAnim.death:
                _animation.CrossFade(anim.death.name, 0.1f);
                break;

            case tmAnim.joke:
                _animation.CrossFade(anim.joke.name, 0.1f);
                break;

            case tmAnim.channeling:
                _animation.CrossFade(anim.channeling.name, 0.1f);
                break;

            case tmAnim.attack:
                _animation.CrossFade(anim.attack.name, 0.1f);
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
            stream.SendNext((int)CurtmAnim);
        }
        else //원격 플레이어의 위치 정보 수신
        {
            currPos = (Vector3)stream.ReceiveNext();
            currRot = (Quaternion)stream.ReceiveNext();
            CurtmAnim = (tmAnim)stream.ReceiveNext();
        }
    }    

    public void DeadFunc()
    {
        PV.RPC("TeemoDead", RpcTarget.All);
    }

    [PunRPC]
    void TeemoDead()
    {
        isDead = true;
        timer = 5.0f;
    }

    [PunRPC]
    void DestroyObj()
    {
        Destroy(this.gameObject);
    }

    [PunRPC]
    void JokeSound(string SoundName)
    {
        SoundManager.Instance.PlayEffSound(SoundName, this.transform.position);
    }
}
