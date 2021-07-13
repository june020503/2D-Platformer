using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriterenderer;
    Animator anim;
    CapsuleCollider2D capsulecollider;
    AudioSource audioSource;

    public GameManager gamemanager;

    public AudioClip audioJump;
    public AudioClip audioAttack;
    public AudioClip audioDamaged;
    public AudioClip audioItem;
    public AudioClip audioDie;
    public AudioClip audioFinish;
    
    //public float maxSpeed;
    public float jumpPower;
    private int jumpCnt = 0;
    public float S;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsulecollider = GetComponent<CapsuleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update() //매 프레임마다 호출(키 입력에 용이)
    {
        //키에서 손땔시 속력감소
        if(Input.GetButtonUp("Horizontal")) //키를 눌렀다 뗄 때 True 한 번 반환(Edit/Project Settings/Input Manager 의 Axis에 존재하는 키 사용)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //방향전환
        if(Input.GetButton("Horizontal")) //키가 눌리는 동안 True 계속 반환
        {
            spriterenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; //flipX = true 일 경우 이미지 반전
        }

        //idle-Run 애니메이션 전환
        if(Mathf.Abs(rigid.velocity.normalized.x) < 0.3)
        {
            anim.SetBool("isRun", false);
        }
        else
        {
            anim.SetBool("isRun", true);
        }


        //점프
        if (Input.GetButtonDown("Jump") && jumpCnt<2)
        {
            jumpCnt++;

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            //애니메이션
            anim.SetBool("isJump", true);

            //음향
            PlaySound("JUMP");
        }

        
        else if (Input.GetButtonUp("Jump") && rigid.velocity.y>0)
        {
            rigid.velocity = rigid.velocity * 0.5f;
        }

    }

    private void FixedUpdate() //일정 시간마다 호출(물리엔진 검사에 용이)
    {
        //방향키로 좌우이동
        /*
        float h = Input.GetAxisRaw("Horizontal"); //GetAxisRaw는 입력값에 따라 -1, 0, 1 값만을 반환. GetAxis는 더욱 연속적인 반환이 가능하여 다음에 보인 간편한 이동 스크립트 작성 가능

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed) //우측 속도 한계 설정
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        else if (rigid.velocity.x < maxSpeed*(-1)) //좌측 속도 한계 설정
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }
        */
        
        //위의 public S를 속도로 입력받아 이동(addforce보다 간편)
        float xMove = Input.GetAxis("Horizontal");

        rigid.velocity = new Vector2(xMove * S, rigid.velocity.y);

        //착지 감지
        if(rigid.velocity.y < 0)
        {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));

            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform"));

            if (rayHit.collider != null)
            {
                if (rayHit.distance < 0.6f)
                {
                    anim.SetBool("isJump", false);
                    jumpCnt = 0;
                }
            }
        }
    }

    //어떤 콜라이더와 충돌시 충돌표면이 위쪽을 보고 있으면 누적 점프 횟수 리셋
    /*
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.contacts[0].normal.y >= 0.7f)
        {
            jumpCnt = 0;
            anim.SetBool("isJump", false);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        anim.SetBool("isJump", true);
    }
    */

    void OnCollisionEnter2D(Collision2D collision)
        //적과 충돌
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                //공격
                onAttack(collision.transform);
            }

            else
            {
                //피격
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //코인 획득
        if (collision.gameObject.tag == "Item")
        {
            //점수 획득
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if(isBronze)
            {
                gamemanager.stagePoint += 10;
            }
            else if(isSilver)
            {
                gamemanager.stagePoint += 30;
            }
            else if(isGold)
            {
                gamemanager.stagePoint += 50;
            }

            //아이템 획득
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }

        else if (collision.gameObject.tag == "Finish")
        {
            //스테이지 이동
            gamemanager.NextStage();

            PlaySound("FINISH");
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        //적과 충돌시 PlayerDamaged 레이어로 변경하여 무적시간 제공
        gameObject.layer = 11;

        //적과 충돌시 캐릭터를 반투명하게 하여 피격효과
        spriterenderer.color = new Color(1, 1, 1, 0.4f);

        //적과 충돌시 반대로 튕겨나감
        int dirc = transform.position.x - targetPos.x > 0 ? 5 : -5;
        rigid.AddForce(new Vector2(dirc, 5), ForceMode2D.Impulse);

        //피격 애니메이션
        anim.SetTrigger("doDamaged");

        //체력감소
        gamemanager.HealthDown();

        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        //피격 후 무적판정 해제
        gameObject.layer = 10;
        spriterenderer.color = new Color(1, 1, 1, 1);
    }

    void onAttack(Transform enemy)
    {
        //적 공격

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        jumpCnt = 1;

        //점수
        gamemanager.stagePoint += 100;

        //음향
        PlaySound("ATTACK");

        //적 처치
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();
        enemyMove.onDamaged();
    }

    public void onDie()
    {
        spriterenderer.color = new Color(1, 1, 1, 0.4f);

        spriterenderer.flipY = true;

        capsulecollider.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        rigid.velocity = Vector2.zero;
    }

    public void PlaySound(string action)
    {
        //소리 재생
        switch(action)
        {
            case "JUMP":
                audioSource.clip = audioJump;
                break;
            case "ATTACK":
                audioSource.clip = audioAttack;
                break;
            case "DAMAGED":
                audioSource.clip = audioDamaged;
                break;
            case "ITEM":
                audioSource.clip = audioItem;
                break;
            case "DIE":
                audioSource.clip = audioDie;
                break;
            case "FINISH":
                audioSource.clip = audioFinish;
                break;
         }

        audioSource.Play();
    }
}
