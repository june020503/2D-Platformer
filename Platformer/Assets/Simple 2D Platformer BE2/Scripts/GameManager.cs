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
        //�������� �̵�
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
        }
        
        else
        {
            //���� Ŭ����
            Time.timeScale = 0;
        }

        //�÷��̾� ��ġ �ʱ�ȭ
        PlayerReposition();

        //���� �ջ�
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    public void HealthDown()
    {
        //ü�°���
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
        //����
        if (collision.gameObject.tag == "Player")
        {
            //����ġ
            if(Health > 1)
            {
                PlayerReposition();
            }

            //ü�°���
            HealthDown();
        }
    }

    void PlayerReposition()
    {
        player.transform.position = new Vector3(0, 0, -1);
        player.VelocityZero();
    }
}
