using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsulecollider;

    public int nextMove;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsulecollider = GetComponent<CapsuleCollider2D>();

        Think();
    }

    private void FixedUpdate()
    {
        //이동
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //낙하방지용 지형확인
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
        //적 이동방향 랜덤 지정 함수
        nextMove = Random.Range(-1, 2);

        float nextThinkTime = Random.Range(1f, 5f);
        Invoke("Think", nextThinkTime);

        //스프라이트 애니메이션
        anim.SetInteger("WalkSpeed", nextMove);
        
        //스프라이트 방향전환
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
    }

    void Turn()
    {
        //낙하 방지를 위한 방향전환 함수
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;

        CancelInvoke();
        Invoke("Think", 1);
    }

    public void onDamaged()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        spriteRenderer.flipY = true;

        capsulecollider.enabled = false;

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        Invoke("DeActivate", 5);
    }

    void DeActivate()
    {
        gameObject.SetActive(false);
    }
}
