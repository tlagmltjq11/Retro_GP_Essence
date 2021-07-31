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
        if(m_dead)
        {
            return;
        }

        //추적 상태인 경우
        if(m_state == State.Tracking)
        {
            var dist = Vector3.Distance(m_targetEntity.transform.position, transform.position);

            //공격 가능 거리인 경우
            if(dist <= m_attackDistance)
            {
                BeginAttack();
            }
        }

        // desiredVelocity == 실제 속도가 아니라 의도한 현재 속도
        m_animator.SetFloat("Speed", m_agent.desiredVelocity.magnitude);
    }

    private void FixedUpdate()
    {
        if (m_dead)
        {
            return;
        }

        //공격을 시작했거나 공격중인 경우
        if(m_state == State.AttackBegin || m_state == State.Attacking)
        {
            //현재 위치에서 타겟을 바라보는 방향을 나타내는 쿼터니언 값
            var lookRotation = Quaternion.LookRotation(m_targetEntity.transform.position - transform.position);
            var targetAngleY = lookRotation.eulerAngles.y;

            targetAngleY = Mathf.SmoothDamp(transform.eulerAngles.y, targetAngleY, ref m_turnSmoothVelocity, m_turnSmoothTime);
            transform.eulerAngles = Vector3.up * targetAngleY;
        }

        if(m_state == State.Attacking)
        {
            //공격의 궤적이 dir방향으로
            var dir = transform.forward;
            //1프레임동안 deltaDist만큼 이동함
            var deltaDist = m_agent.velocity.magnitude * Time.fixedDeltaTime;

            //감지된 콜라이더의 갯수
            //RaycastHit은 벨루타입이라 ref를 써줘야했는데, 이번엔 배열이기에 안써줘도된다.
            //레이어를 플레이어로 설정
            var size = Physics.SphereCastNonAlloc(m_attackRoot.position, m_attackRadius, dir, m_hits, deltaDist, m_whatIsTarget);

            for (var i = 0; i < size; i++)
            {
                var attackTargetEntity = m_hits[i].collider.GetComponent<LivingEntity>();

                if (attackTargetEntity != null && !m_lastAttackedTargets.Contains(attackTargetEntity))
                {
                    var message = new DamageMessage();
                    message.m_amount = m_damage;
                    message.m_damager = gameObject;

                    //SphereCastNonAlloc를 위해 sphere가 움직이려고 하자마자 충돌되는 콜라이더가
                    //존재한다면 즉 실행하기전에 이미 겹쳐져있던 콜라이더가 존재한다면 해당 콜라이더의
                    //hit.point는 zero 벡터, dist는 0으로 나오게 된다.
                    //고로 애초에 겹쳐있던 콜라이더는 힛포인트를 m_attackRoot.position로 지정한다.
                    if (m_hits[i].distance <= 0f)
                    {
                        message.m_hitPoint = m_attackRoot.position;
                    }
                    else
                    {
                        message.m_hitPoint = m_hits[i].point;
                    }

                    message.m_hitNormal = m_hits[i].normal;

                    attackTargetEntity.ApplyDamage(message);
                    m_lastAttackedTargets.Add(attackTargetEntity);
                    break; 
                    //플레이어의 콜라이더가 여러개이거나, 충돌포인트가 여러개일 수 있기 때문에
                    //SphereCastNonAlloc가 충돌포인트의 배열을 반환해준 것.
                    //고로 한번 공격을 적용했으면 break로 빠져나가야만 플레이어에게 중복 데미지를 주지 않는다.
                }
            }
        }
    }
    #endregion

    #region Public Methods
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;
        
        //공격을 받았는데 추적할 대상을 못찾은 경우였다면 즉시 추적대상으로 지정
        if(m_targetEntity == null)
        {
            m_targetEntity = damageMessage.m_damager.GetComponent<LivingEntity>();
        }

        EffectManager.Instance.PlayHitEffect(damageMessage.m_hitPoint, damageMessage.m_hitNormal, transform, EffectManager.EffectType.Flesh);
        m_audioPlayer.PlayOneShot(m_hitClip);

        return true;
    }

    public override void Die()
    {
        base.Die();

        GetComponent<Collider>().enabled = false; //길막 방지
        m_agent.enabled = false; //다른 좀비 에이전트가 피해가지 않도록 비활성화
        m_animator.applyRootMotion = true; //좀비의 사망모션이 자연스럽게 위치를 변경하도록함.
        m_animator.SetTrigger("Die");

        m_audioPlayer.PlayOneShot(m_deathClip);
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

        m_agent.isStopped = true; //추적 중단
        m_animator.SetTrigger("Attack"); //공격 애니메이션 재생
    }

    //데미지가 들어가기 시작하는 지점을 나타냄
    public void EnableAttack() //애니메이션 이벤트
    {
        m_state = State.Attacking;

        m_lastAttackedTargets.Clear();
    }

    //공격이 끝나는 지점을 나타냄
    public void DisableAttack() //애니메이션 이벤트
    {
        if(hasTarget)
        {
            m_state = State.Tracking;
        }
        else
        {
            m_state = State.Patrol;
        }

        m_agent.isStopped = false;
    }
    #endregion

    #region Private Methods
    private bool IsTargetOnSight(Transform target)
    {
        //벡터의 내적, 외적으로도 구현할 수 있는 부분

        var dir = target.position - m_eyeTransform.position; //목표위치를 향하는 벡터
        dir.y = m_eyeTransform.forward.y; //y값을 맞춰줌

        //시야각 내에 존재하는지 체크
        if(Vector3.Angle(dir, m_eyeTransform.forward) > m_fieldOfView * 0.5f)
        {
            return false;
        }

        //Ray를 쏠때에는 y값도 필요하기 때문에 y값을 포함하는 벡터를 다시 구해준다.
        dir = target.position - m_eyeTransform.position;

        RaycastHit hit;
        //처음 레이에 닿은 대상이 장애물은 아닌지 체크
        if (Physics.Raycast(m_eyeTransform.position, dir, out hit, m_viewDistance, m_whatIsTarget))
        {
            //장애물이 없다면 true
            if(hit.transform == target)
            {
                return true;
            }
        }
       
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

                //이전에 이동하던 목적지까지 거의 도착했다면 새로운 패트롤 목적지를 할당
                if (m_agent.remainingDistance <= 1f)
                {
                    var patrolTargetPosition = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    m_agent.SetDestination(patrolTargetPosition);
                }

                //콜라이더 검출 - 눈 위치에서 시야거리만큼 구를 그려서 검출함
                var colliders = Physics.OverlapSphere(m_eyeTransform.position, m_viewDistance, m_whatIsTarget);

                foreach(var col in colliders)
                {
                    //시야내에 존재하는지 체크
                    if(!IsTargetOnSight(col.transform))
                    {
                        continue;
                    }

                    var livingEntity = col.GetComponent<LivingEntity>();

                    //생명체이며 살아있는 상태인지 체크
                    if(livingEntity != null && !livingEntity.m_dead)
                    {
                        m_targetEntity = livingEntity; //타겟으로 설정
                        break; //foreach 탈출
                    }
                }
            }

            yield return new WaitForSeconds(0.05f);
        }
    }
    #endregion

}