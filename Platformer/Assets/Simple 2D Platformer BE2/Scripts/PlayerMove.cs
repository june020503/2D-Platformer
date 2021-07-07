using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    Rigidbody2D rigid;
    SpriteRenderer spriterenderer;
    Animator anim;

    public float maxSpeed;
    //public float S;

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
    }

    private void FixedUpdate() //���� �ð����� ȣ��(�������� �˻翡 ����)
    {
        //����Ű�� �¿��̵�
        
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
        
        /*
        ���� public S�� �ӵ��� �Է¹޾� �̵�(addforce���� ����)
        float xMove = Input.GetAxis("Horizontal");
        rigid.velocity = new Vector2(xMove * S, rigid.velocity.y);
        */
    }
}
