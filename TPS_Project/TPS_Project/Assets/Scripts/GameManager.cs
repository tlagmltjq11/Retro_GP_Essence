using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Field
    private static GameManager m_instance;
    
    public static GameManager Instance
    {
        get
        {
            if (m_instance == null) m_instance = FindObjectOfType<GameManager>();
            
            return m_instance;
        }
    }

    private int m_score;
    public bool isGameover { get; private set; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        if (Instance != this) Destroy(gameObject);
    }
    #endregion

    #region Public Methods
    public void AddScore(int newScore)
    {
        if (!isGameover)
        {
            m_score += newScore;
            UIManager.Instance.UpdateScoreText(m_score);
        }
    }
    
    public void EndGame()
    {
        isGameover = true;
        UIManager.Instance.SetActiveGameoverUI(true);
    }
    #endregion
}