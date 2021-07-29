using System;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    #region Field
    public float m_startingHealth = 100f; //시작 체력
    public float m_health { get; protected set; } //현재 체력
    public bool m_dead { get; protected set; } //사망 상태
    
    public event Action OnDeath; //사망하는 순간에 실행할 콜백들
    
    private const float m_minTimeBetDamaged = 0.1f; //공격사이 최소 대기시간 - 변경할 일이 없어 const
    private float m_lastDamagedTime; //최근 공격을 당한 시점
    //-> 동시에 여러번 공격받는 것을 방지

    //무적모드인지 반환
    protected bool IsInvulnerable
    {
        get
        {
            //공격을 당할 수 있는 상태인 경우
            if (Time.time >= m_lastDamagedTime + m_minTimeBetDamaged) return false;

            return true;
        }
    }
    #endregion

    #region Unity Methods
    protected virtual void OnEnable()
    {
        //초기화
        m_dead = false;
        m_health = m_startingHealth;
    }
    #endregion

    #region Public Methods
    public virtual bool ApplyDamage(DamageMessage damageMessage)
    {
        //무적이거나, 데미지를 가하는 대상이 자기자신이거나, 사망한 상태거나
        if (IsInvulnerable || damageMessage.m_damager == gameObject || m_dead) return false;

        //최근 공격받은 시간을 초기화
        m_lastDamagedTime = Time.time;
        m_health -= damageMessage.m_amount; //체력을 깎는다.
        
        //사망한 경우
        if (m_health <= 0) Die();

        return true;
    }
    
    //체력 회복
    public virtual void RestoreHealth(float newHealth)
    {
        //사망인 경우 리턴
        if (m_dead) return;

        //체력 회복
        m_health += newHealth;
    }
    
    public virtual void Die()
    {
        //이벤트 콜백메서드 실행
        if (OnDeath != null) OnDeath();

        //사망 상태로 변경
        m_dead = true;
    }
    #endregion
}