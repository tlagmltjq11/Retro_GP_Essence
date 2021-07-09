using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    #region Field
    public static GameManager instance;

    public UnityEvent m_onReset; //이벤트

    public GameObject m_readyPanel;
    public Text m_scoreText;
    public Text m_bestScoreText;
    public Text m_messageText;
    public bool m_isRoundActive = false;
    public ShooterRotator m_shooterRotator;
    public CamFollow m_cam;

    private int m_score = 0;
    #endregion

    #region Unity Methods
    void Awake()
    {
        instance = this;
        UpdateUI();
    }

    void Start()
    {
        StartCoroutine("RoundRoutine");
    }
    #endregion

    #region Methods
    public void AddScore(int newScore)
    {
        m_score += newScore;
        UpdateBestScore();
        UpdateUI();
    }

    public void OnBallDestroy()
    {
        UpdateUI();
        m_isRoundActive = false;
    }

    public void Reset()
    {
        m_score = 0;
        UpdateUI();

        //라운드 재시작
        StartCoroutine("RoundRoutine");
    }

    IEnumerator RoundRoutine()
    {
        //레디
        m_onReset.Invoke();

        m_readyPanel.SetActive(true);
        m_cam.SetTarget(m_shooterRotator.transform, CamFollow.State.Idle);
        m_shooterRotator.enabled = false;
        m_isRoundActive = false;
        m_messageText.text = "Ready...";

        yield return new WaitForSeconds(3f);

        //플레이
        m_isRoundActive = true;
        m_readyPanel.SetActive(false);
        m_shooterRotator.enabled = true;
        m_cam.SetTarget(m_shooterRotator.transform, CamFollow.State.Ready);

        while(m_isRoundActive == true)
        {
            yield return null;
        }

        //엔드
        m_readyPanel.SetActive(true);
        m_shooterRotator.enabled = false;

        m_messageText.text = "Wait For Next Round...";

        yield return new WaitForSeconds(3f);
        Reset();
    }

    void UpdateBestScore()
    {
        if(GetBestScore() < m_score)
        {
            PlayerPrefs.SetInt("BestScore", m_score);
        }
            
    }

    int GetBestScore()
    {
        int bestScore = PlayerPrefs.GetInt("BestScore");
        return bestScore;
    }

    void UpdateUI()
    {
        m_scoreText.text = "Score: " + m_score;
        m_bestScoreText.text = "Best Score: " + GetBestScore();
    }
    #endregion
}
