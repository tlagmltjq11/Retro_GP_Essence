using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR
using UnityEditor; //빌드에서는 빠지게됨.
#endif

public class Enemy : LivingEntity
{
    #region Field
    private enum State
    {
        Patrol,
        Tracking,
        AttackBegin,
        Attacking
    }
    
    private State m_state;
    
    private NavMeshAgent m_agent;
    private Animator m_animator;

    public Transform m_attackRoot; //공격을하는 피벗 포인트
    public Transform m_eyeTransform; //시야의 기준점, 눈의 위치
    
    private AudioSource m_audioPlayer;
    public AudioClip m_hitClip;
    public AudioClip m_deathClip;
    
    private Renderer m_skinRenderer; //좀비의 피부색을 공격력에 따라 변경할 것임

    public float m_runSpeed = 10f;
    public float m_patrolSpeed = 3f;
    [Range(0.01f, 2f)] public float m_turnSmoothTime = 0.1f; //방향회전 지연시간
    private float m_turnSmoothVelocity; //댐핑
    
    public float m_damage = 30f;
    public float m_attackRadius = 2f; //공격반경
    private float m_attackDistance; //공격을 시도하는 거리
    
    public float m_fieldOfView = 50f; //시야각
    public float m_viewDistance = 10f; //시야거리
    
    public LivingEntity m_targetEntity; //추적할 대상
    public LayerMask m_whatIsTarget; //레이어필터

    private RaycastHit[] m_hits = new RaycastHit[10]; //범위 기반의 레이캐스트를 사용할 것이기 때문에 배열로
    //직전 프레임까지 공격이 적용된 대상을 담아두는 리스트 - 포함된 적은 현재 프레임에서 데미지 적용 x
    private List<LivingEntity> m_lastAttackedTargets = new List<LivingEntity>();
    
    //추적할 대상이 존재하는지 반환하는 프로퍼티
    private bool hasTarget => m_targetEntity != null && !m_targetEntity.m_dead;
    #endregion

#if UNITY_EDITOR

    private void OnDrawGizmosSelected()
    {
        if (m_attackRoot != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(m_attackRoot.position, m_attackRadius);
        }

        if (m_eyeTransform != null)
        {
            //호의 왼쪽 시작각도
            var leftEyeRotation = Quaternion.AngleAxis(-m_fieldOfView * 0.5f, Vector3.up);
            //왼쪽 시작각도에 transform.forward를 곱해주면 해당 왼쪽 벡터를 구할 수 있게됨.
            //즉 포워드 벡터를 왼쪽으로 30도 회전시킨 벡터를 얻어낼 수 있음.
            var leftRayDirection = leftEyeRotation * transform.forward;
            Handles.color = new Color(1f, 1f, 1f, 0.2f);
            Handles.DrawSolidArc(m_eyeTransform.position, Vector3.up, leftRayDirection, m_fieldOfView, m_viewDistance);
        }
    }

#endif

    #region Unity Methods
    private void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
        m_audioPlayer = GetComponent<AudioSource>();
        m_skinRenderer = GetComponentInChildren<Renderer>();

        //y값을 맞춰준 후 거리를 계산
        var attackPivot = m_attackRoot.position;
        attackPivot.y = transform.position.y;
        m_attackDistance = Vector3.Distance(transform.position, attackPivot) + m_attackRadius;

        m_agent.stoppingDistance = m_attackDistance;
        m_agent.speed = m_patrolSpeed;
    }

    private void Start()
    {
        StartCoroutine(UpdatePath());
    }

    private void Update()
    {

    }

    private void FixedUpdate()
    {
        if (m_dead) return;
    }
    #endregion

    #region Public Methods
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;
        
        return true;
    }

    public override void Die()
    {

    }

    public void Setup(float health, float damage, float runSpeed, float patrolSpeed, Color skinColor)
    {
        m_startingHealth = health;
        m_health = health;
        m_damage = damage;
        m_runSpeed = runSpeed;
        m_patrolSpeed = patrolSpeed;
        m_skinRenderer.material.color = skinColor;

        m_agent.speed = m_patrolSpeed; //패트롤스피드가 바뀌었으니 에이전트에 다시 초기화
    }

    public void BeginAttack()
    {
        m_state = State.AttackBegin;

        m_agent.isStopped = true;
        m_animator.SetTrigger("Attack");
    }

    public void EnableAttack()
    {
        m_state = State.Attacking;

        m_lastAttackedTargets.Clear();
    }

    public void DisableAttack()
    {
        m_state = State.Tracking;

        m_agent.isStopped = false;
    }
    #endregion

    #region Private Methods
    private bool IsTargetOnSight(Transform target)
    {
        return false;
    }
    #endregion

    #region Coroutine
    private IEnumerator UpdatePath()
    {
        //무한루프
        while (!m_dead)
        {
            //타겟이 존재하는 경우
            if (hasTarget)
            {
                //Patrol 상태였던 경우
                if(m_state == State.Patrol)
                {
                    m_state = State.Tracking; //추적모드로 변경
                    m_agent.speed = m_runSpeed;
                }

                //타겟의 위치로 이동시킨다.
                m_agent.SetDestination(m_targetEntity.transform.position);
            }
            else
            {
                //hasTarget이 false인데 m_targetEntity이 null이 아니라는것은 사망했다는 것
                //고로 null로 초기화해준다.
                if (m_targetEntity != null) m_targetEntity = null;

                if(m_state != State.Patrol)
                {
                    m_state = State.Patrol;
                    m_agent.speed = m_patrolSpeed;
                }

                var patrolTargetPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
            }

            yield return new WaitForSeconds(0.2f);
        }
    }
    #endregion

}