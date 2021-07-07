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
    }

    private void FixedUpdate() //일정 시간마다 호출(물리엔진 검사에 용이)
    {
        //방향키로 좌우이동
        
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
        
        /*
        위의 public S를 속도로 입력받아 이동(addforce보다 간편)
        float xMove = Input.GetAxis("Horizontal");
        rigid.velocity = new Vector2(xMove * S, rigid.velocity.y);
        */
    }
}
