using System;
using System.Collections;
using UnityEngine;

public class Gun : MonoBehaviour
{
    #region Field
    //총의 상태
    public enum State
    {
        Ready,
        Empty,
        Reloading
    }

    public State m_state { get; private set; }
    
    private PlayerShooter m_gunHolder;
    private LineRenderer m_bulletLineRenderer;
    
    private AudioSource m_gunAudioPlayer;
    public AudioClip m_shotClip;
    public AudioClip m_reloadClip;
    
    public ParticleSystem m_muzzleFlashEffect;
    public ParticleSystem m_shellEjectEffect;
    
    public Transform m_fireTransform; //총알 발사위치
    public Transform m_leftHandMount; //왼손의 위치

    public float m_damage = 25;
    public float m_fireDistance = 100f; //사거리

    public int m_ammoRemain = 100; //총 남은 탄약
    public int m_magAmmo; //현재 탄창의 탄약
    public int m_magCapacity = 30; //탄창의 최대 총알 수
    public int m_initAmmo = 100; //초기화 될 탄약의 수

    public float m_timeBetFire = 0.12f; //연사속도
    public float m_reloadTime = 1.8f; //재장전 시간
    
    [Range(0f, 10f)] public float m_maxSpread = 3f; //탄착군의 최대 범위
    [Range(1f, 10f)] public float m_stability = 1f; //반동이 증가하는 속도
    [Range(0.01f, 3f)] public float m_restoreFromRecoilSpeed = 2f; //연사 중단 후 탄퍼짐이 회복되는 속도
    private float m_currentSpread; //현재 탄퍼짐의 정도 값
    private float m_currentSpreadVelocity; //탄퍼짐 댐핑용

    private float m_lastFireTime; //가장 최근 발사 시점

    private LayerMask m_excludeTarget; //총을 맞지 않아야하는 대상 레이어
    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_gunAudioPlayer = GetComponent<AudioSource>();
        m_bulletLineRenderer = GetComponent<LineRenderer>();

        m_bulletLineRenderer.positionCount = 2;
        m_bulletLineRenderer.enabled = false;
    }

    private void OnEnable()
    {
        m_magAmmo = m_magCapacity;
        m_currentSpread = 0f;
        m_lastFireTime = 0f;
        m_state = State.Ready;
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        //max이상으로 반동이 심해지지 않도록 제한
        m_currentSpread = Mathf.Clamp(m_currentSpread, 0f, m_maxSpread);
        //탄퍼짐 스무스하게 회복
        m_currentSpread = Mathf.SmoothDamp(m_currentSpread, 0f, ref m_currentSpreadVelocity, 1f / m_restoreFromRecoilSpeed);
    }
    #endregion

    #region Public Methods
    //shooter 초기화
    public void Setup(PlayerShooter gunHolder)
    {
        this.m_gunHolder = gunHolder;
        //총의 holder가 맞추지않기로 결정한 레이어를 그대로 가져와 적용
        m_excludeTarget = gunHolder.m_excludeTarget;
    }

    public bool Fire(Vector3 aimTarget)
    {
        //현재시간이 마지막 발사시점 + 발사간격 보다 크거가 같으면 다시 쏠 수 있는 경우
        if(m_state == State.Ready && Time.time >= m_lastFireTime + m_timeBetFire)
        {
            var fireDir = aimTarget - m_fireTransform.position;

            //표준편차를 이용한 랜덤 각도 반환
            var xError = Utility.GetRandomNormalDistribution(0f, m_currentSpread);
            var yError = Utility.GetRandomNormalDistribution(0f, m_currentSpread);

            //랜덤 각을 적용해 탄퍼짐을 구현
            fireDir = Quaternion.AngleAxis(yError, Vector3.up) * fireDir;
            fireDir = Quaternion.AngleAxis(xError, Vector3.right) * fireDir;

            m_currentSpread += 1f / m_stability; //탄퍼짐 증가

            m_lastFireTime = Time.time; //마지막 발사시간
            Shot(m_fireTransform.position, fireDir);

            return true;
        }

        return false;
    }

    public bool Reload()
    {
        if(m_state == State.Reloading || m_ammoRemain <= 0 || m_magAmmo >= m_magCapacity)
        {
            return false;
        }

        StartCoroutine(ReloadRoutine());

        return true;
    }
    #endregion

    #region Private Methods
    private void Shot(Vector3 startPoint, Vector3 direction)
    {
        RaycastHit hit;
        Vector3 hitPosition;

        if(Physics.Raycast(startPoint, direction, out hit, m_fireDistance, ~m_excludeTarget))
        {
            var target = hit.collider.GetComponent<IDamageable>(); //데미지를 받을 수 있는 객체인지 판별
            
            if(target != null)
            {
                DamageMessage damageM;

                damageM.m_damager = m_gunHolder.gameObject;
                damageM.m_amount = m_damage;
                damageM.m_hitPoint = hit.point;
                damageM.m_hitNormal = hit.normal;

                target.ApplyDamage(damageM);
            }
            else
            {
                //데미지를 받을 수 없는 즉 생명체가 아니기 때문에 Common 이펙트를 재생시킨다.
                EffectManager.Instance.PlayHitEffect(hit.point, hit.normal, hit.transform);
            }

            hitPosition = hit.point;
        }
        else
        {
            //충돌하지 않았다면 최대거리로 날아간 위치로 조정 -> 라인렌더러 용
            hitPosition = startPoint + direction * m_fireDistance;
        }

        StartCoroutine(ShotEffect(hitPosition));
        m_magAmmo--;

        if(m_magAmmo <= 0)
        {
            m_state = State.Empty;
        }
    }
    #endregion

    #region Coroutine
    private IEnumerator ShotEffect(Vector3 hitPosition)
    {
        m_muzzleFlashEffect.Play();
        m_shellEjectEffect.Play();

        m_gunAudioPlayer.PlayOneShot(m_shotClip); //소리 중첩 가능

        m_bulletLineRenderer.enabled = true;
        m_bulletLineRenderer.SetPosition(0, m_fireTransform.position);
        m_bulletLineRenderer.SetPosition(1, hitPosition); //총알이 맞은지점을 끝 정점으로 할당
        
        yield return new WaitForSeconds(0.03f);

        m_bulletLineRenderer.enabled = false;
    }

    private IEnumerator ReloadRoutine()
    {
        m_state = State.Reloading;
        m_gunAudioPlayer.PlayOneShot(m_reloadClip);

        yield return new WaitForSeconds(m_reloadTime);

        var ammoToFill = Mathf.Clamp(m_magCapacity - m_magAmmo, 0, m_ammoRemain);
        m_magAmmo += ammoToFill;
        m_ammoRemain -= ammoToFill;

        m_state = State.Ready;
    }
    #endregion
}