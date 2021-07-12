using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriterenderer;
    Animator anim;

    //public float maxSpeed;
    public float jumpPower;
    private int jumpCnt = 0;
    public float S;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriterenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
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
}
