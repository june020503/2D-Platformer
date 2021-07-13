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

    private void Update() //�� �����Ӹ��� ȣ��(Ű �Է¿� ����)
    {
        //Ű���� �ն��� �ӷ°���
        if(Input.GetButtonUp("Horizontal")) //Ű�� ������ �� �� True �� �� ��ȯ(Edit/Project Settings/Input Manager �� Axis�� �����ϴ� Ű ���)
        {
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y);
        }

        //������ȯ
        if(Input.GetButton("Horizontal")) //Ű�� ������ ���� True ��� ��ȯ
        {
            spriterenderer.flipX = Input.GetAxisRaw("Horizontal") == -1; //flipX = true �� ��� �̹��� ����
        }

        //idle-Run �ִϸ��̼� ��ȯ
        if(Mathf.Abs(rigid.velocity.normalized.x) < 0.3)
        {
            anim.SetBool("isRun", false);
        }
        else
        {
            anim.SetBool("isRun", true);
        }


        //����
        if (Input.GetButtonDown("Jump") && jumpCnt<2)
        {
            jumpCnt++;

            rigid.velocity = Vector2.zero;
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);

            //�ִϸ��̼�
            anim.SetBool("isJump", true);

            //����
            PlaySound("JUMP");
        }

        
        else if (Input.GetButtonUp("Jump") && rigid.velocity.y>0)
        {
            rigid.velocity = rigid.velocity * 0.5f;
        }

    }

    private void FixedUpdate() //���� �ð����� ȣ��(�������� �˻翡 ����)
    {
        //����Ű�� �¿��̵�
        /*
        float h = Input.GetAxisRaw("Horizontal"); //GetAxisRaw�� �Է°��� ���� -1, 0, 1 ������ ��ȯ. GetAxis�� ���� �������� ��ȯ�� �����Ͽ� ������ ���� ������ �̵� ��ũ��Ʈ �ۼ� ����

        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if(rigid.velocity.x > maxSpeed) //���� �ӵ� �Ѱ� ����
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }

        else if (rigid.velocity.x < maxSpeed*(-1)) //���� �ӵ� �Ѱ� ����
        {
            rigid.velocity = new Vector2(maxSpeed*(-1), rigid.velocity.y);
        }
        */
        
        //���� public S�� �ӵ��� �Է¹޾� �̵�(addforce���� ����)
        float xMove = Input.GetAxis("Horizontal");

        rigid.velocity = new Vector2(xMove * S, rigid.velocity.y);

        //���� ����
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

    //� �ݶ��̴��� �浹�� �浹ǥ���� ������ ���� ������ ���� ���� Ƚ�� ����
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
        //���� �浹
    {
        if(collision.gameObject.tag == "Enemy")
        {
            if(rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
            {
                //����
                onAttack(collision.transform);
            }

            else
            {
                //�ǰ�
                OnDamaged(collision.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        //���� ȹ��
        if (collision.gameObject.tag == "Item")
        {
            //���� ȹ��
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

            //������ ȹ��
            collision.gameObject.SetActive(false);

            PlaySound("ITEM");
        }

        else if (collision.gameObject.tag == "Finish")
        {
            //�������� �̵�
            gamemanager.NextStage();

            PlaySound("FINISH");
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        //���� �浹�� PlayerDamaged ���̾�� �����Ͽ� �����ð� ����
        gameObject.layer = 11;

        //���� �浹�� ĳ���͸� �������ϰ� �Ͽ� �ǰ�ȿ��
        spriterenderer.color = new Color(1, 1, 1, 0.4f);

        //���� �浹�� �ݴ�� ƨ�ܳ���
        int dirc = transform.position.x - targetPos.x > 0 ? 5 : -5;
        rigid.AddForce(new Vector2(dirc, 5), ForceMode2D.Impulse);

        //�ǰ� �ִϸ��̼�
        anim.SetTrigger("doDamaged");

        //ü�°���
        gamemanager.HealthDown();

        Invoke("OffDamaged", 2);
    }

    void OffDamaged()
    {
        //�ǰ� �� �������� ����
        gameObject.layer = 10;
        spriterenderer.color = new Color(1, 1, 1, 1);
    }

    void onAttack(Transform enemy)
    {
        //�� ����

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        jumpCnt = 1;

        //����
        gamemanager.stagePoint += 100;

        //����
        PlaySound("ATTACK");

        //�� óġ
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
        //�Ҹ� ���
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
