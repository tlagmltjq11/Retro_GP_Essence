using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    #region Field
    private static UIManager instance;
    
    public static UIManager Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<UIManager>();

            return instance;
        }
    }

    [SerializeField] private GameObject m_gameoverUI;
    [SerializeField] private Crosshair m_crosshair;

    [SerializeField] private Text m_healthText;
    [SerializeField] private Text m_lifeText;
    [SerializeField] private Text m_scoreText;
    [SerializeField] private Text m_ammoText;
    [SerializeField] private Text m_waveText;
    #endregion

    #region Public Methods
    public void UpdateAmmoText(int magAmmo, int remainAmmo)
    {
        m_ammoText.text = magAmmo + "/" + remainAmmo;
    }

    public void UpdateScoreText(int newScore)
    {
        m_scoreText.text = "Score : " + newScore;
    }
    
    public void UpdateWaveText(int waves, int count)
    {
        m_waveText.text = "Wave : " + waves + "\nEnemy Left : " + count;
    }

    public void UpdateLifeText(int count)
    {
        m_lifeText.text = "Life : " + count;
    }

    public void UpdateCrossHairPosition(Vector3 worldPosition)
    {
        m_crosshair.UpdatePosition(worldPosition);
    }
    
    public void UpdateHealthText(float health)
    {
        m_healthText.text = Mathf.Floor(health).ToString();
    }
    
    public void SetActiveCrosshair(bool active)
    {
        m_crosshair.SetActiveCrosshair(active);
    }
    
    public void SetActiveGameoverUI(bool active)
    {
        m_gameoverUI.SetActive(active);
    }
    
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    #endregion
}