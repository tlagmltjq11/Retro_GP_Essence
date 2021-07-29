using UnityEngine;

public class PlayerHealth : LivingEntity
{
    #region Field
    private Animator m_animator;
    private AudioSource m_playerAudioPlayer;

    public AudioClip m_deathClip; //사망 시 오디오
    public AudioClip m_hitClip; //피격 시 오디오
    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        m_playerAudioPlayer = GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable(); //부모의 메서드 실행
        UpdateUI(); //체력 UI 갱신
    }
    #endregion

    #region Public Override Methods
    public override void RestoreHealth(float newHealth)
    {
        base.RestoreHealth(newHealth); //부모의 메서드 실행
        UpdateUI(); //체력 UI 갱신
    }
    
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        //부모의 메서드를 실행하는데 만약 데미지 적용에 실패했다면 이 메서드 역시 실패
        if (!base.ApplyDamage(damageMessage)) return false;

        //데미지 적용에 성공했으니 이펙트를 생성하고 오디오를 재생 후 체력 UI 갱신
        EffectManager.Instance.PlayHitEffect(damageMessage.m_hitPoint, damageMessage.m_hitNormal, transform, EffectManager.EffectType.Flesh);
        m_playerAudioPlayer.PlayOneShot(m_hitClip);
        UpdateUI();

        return true;
    }
    
    public override void Die()
    {
        base.Die(); //부모의 메서드 실행
        m_playerAudioPlayer.PlayOneShot(m_deathClip);
        m_animator.SetTrigger("Die");
        UpdateUI();
    }
    #endregion

    #region Private Methods
    private void UpdateUI()
    {
        //체력 UI 갱신
        UIManager.Instance.UpdateHealthText(m_dead ? 0f : m_health);
    }
    #endregion
}