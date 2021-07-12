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
        //스테이지 이동
        if(stageIndex < Stages.Length - 1)
        {
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);

            UIStage.text = "STAGE" + (stageIndex + 1);
        }
        
        else
        {
            //게임 클리어
            Time.timeScale = 0;

            //재시작 버튼 UI
            Text btnText = RestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Game Clear!";
            RestartBtn.SetActive(true);
        }

        //플레이어 위치 초기화
        PlayerReposition();

        //점수 합산
        totalPoint += stagePoint;
        stagePoint = 0;

        Health++;
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

            RestartBtn.SetActive(true);
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

    public void Restart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
