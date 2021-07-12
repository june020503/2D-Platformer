using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int stagePoint;
    public int totalPoint;
    public int stageIndex;
    public int Health;

    public PlayerMove player;
    public GameObject[] Stages;

    public void NextStage()
    {
        //스테이지 이동
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
        }
        
        else
        {
            //게임 클리어
            Time.timeScale = 0;
        }

        //플레이어 위치 초기화
        PlayerReposition();

        //점수 합산
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        //체력감소
        if (Health > 1)
        {
            Health--;
        }

        else
        {
            player.onDie();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //낙사
        if (collision.gameObject.tag == "Player")
        {
            //원위치
            if(Health > 1)
            {
                PlayerReposition();
            }

            //체력감소
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }
}
