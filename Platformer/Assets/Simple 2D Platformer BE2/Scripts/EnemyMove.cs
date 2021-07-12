using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;

    public int nextMove;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        Think();
    }

    private void FixedUpdate()
    {
        //�̵�
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //���Ϲ����� ����Ȯ��
        Vector2 frontVec = new Vector2(rigid.position.x + nextMove * 0.3f, rigid.position.y);

        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));
        if (rayHit.collider == null)
        {
            Turn();
        }
    }

    
    void Think()
    {
        //�� �̵����� ���� ���� �Լ�
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(1f, 5f);
        Invoke("Think", nextThinkTime);

        //��������Ʈ �ִϸ��̼�
        anim.SetInteger("WalkSpeed", nextMove);
        
        //��������Ʈ ������ȯ
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
    }

    void Turn()
    {
        //���Ͻ� ������ȯ �Լ�
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 1);
    }
}
