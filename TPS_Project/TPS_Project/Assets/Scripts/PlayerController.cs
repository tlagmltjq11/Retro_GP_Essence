using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    #region Field
    private Animator m_animator;
    public AudioClip m_itemPickupClip;
    public int m_lifeRemains = 3; //남은 생명의 수
    private AudioSource m_playerAudioPlayer;

    //해당 스크립트의 주 기능은 아래 3개의 컴포넌트를 관리하는 것임.
    private PlayerHealth m_playerHealth;
    private PlayerMovement m_playerMovement;
    private PlayerShooter m_playerShooter;
    #endregion

    #region Unity Methods
    private void Start()
    {
        m_playerMovement = GetComponent<PlayerMovement>();
        m_playerShooter = GetComponent<PlayerShooter>();
        m_playerHealth = GetComponent<PlayerHealth>();
        m_playerAudioPlayer = GetComponent<AudioSource>();

        //LivingEntity에 존재하던 Action 변수에 죽고난 후의 이벤트 처리를 추가
        m_playerHealth.OnDeath += HandleDeath;

        UIManager.Instance.UpdateLifeText(m_lifeRemains);
        Cursor.visible = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(m_playerHealth.m_dead)
        {
            return;
        }

        var item = other.GetComponent<IItem>();

        if(item != null)
        {
            item.Use(gameObject); //아이템 사용
            m_playerAudioPlayer.PlayOneShot(m_itemPickupClip);
        }
    }
    #endregion

    #region Public Methods
    public void Respawn()
    {
        //---OnEnable, OnDisable에서 사용했던 초기화 코드들을 실행시키도록 유도하기 위함.
        gameObject.SetActive(false);
        transform.position = Utility.GetRandomPointOnNavMesh(transform.position, 30f, NavMesh.AllAreas);

        m_playerMovement.enabled = true;
        m_playerShooter.enabled = true;
        gameObject.SetActive(true);
        //---

        m_playerShooter.m_gun.m_ammoRemain = m_playerShooter.m_gun.m_initAmmo;
        Cursor.visible = false;
    }
    #endregion

    #region Private Methods
    private void HandleDeath()
    {
        m_playerMovement.enabled = false;
        m_playerShooter.enabled = false;

        if(m_lifeRemains > 0)
        {
            m_lifeRemains--;
            UIManager.Instance.UpdateLifeText(m_lifeRemains);
            Invoke("Respawn", 3f);
        }
        else
        {
            GameManager.Instance.EndGame();
        }

        Cursor.visible = true;
    }
    #endregion
}