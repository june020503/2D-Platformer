using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public int stagePoint;
    public int totalPoint;
    public int stageIndex;
    public int Health;

    public PlayerMove player;
    public GameObject[] Stages;

    public Image UIhealth;
    public Text Hp;
    public Text UIPoint;
    public Text UIStage;
    public GameObject RestartBtn;

    private void Update()
    {
        UIPoint.text = (totalPoint + stagePoint).ToString();
        Hp.text = Health.ToString();
    }

    public void NextStage()
    {
        //�������� �̵�
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        
        else
        {
            //���� Ŭ����
            Time.timeScale = 0;

            //����� ��ư UI
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            RestartBtn.SetActive(true);
        }

        //�÷��̾� ��ġ �ʱ�ȭ
        PlayerReposition();

        //���� �ջ�
        totalPoint += stagePoint;
        stagePoint = 0;

        Health++;
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

            RestartBtn.SetActive(true);
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

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
