using UnityEngine;


public class PlayerShooter : MonoBehaviour
{
    #region Field
    public enum AimState
    {
        Idle,
        HipFire
    }

    public AimState m_aimState { get; private set; }

    public Gun m_gun; //사용중인 무기
    public LayerMask m_excludeTarget; //제외할 레이어 대상
    
    private PlayerInput m_playerInput;
    private Animator m_playerAnimator;
    private Camera m_playerCamera;

    //마지막 발사 입력 시점에서 얼마동안 발사 입력이 없으면 IDLE로 돌아올지를 나타냄.
    private float m_waitingTimeForReleasingAim = 2.5f;
    private float m_lastFireInputTime;

    //실제 조준하고있는 대상 -> 즉 TPS이기 때문에 카메라의 중앙과
    //실제로 캐릭터의 총구가 조준하고있는 대상이 다를 수 있는 경우를 의미함.
    //고로 실제로 총알이 맞을 곳을 저장함
    private Vector3 m_aimPoint; 

    //캐릭터가 바라보는 방향과 카메라가 바라보는 방향의 각도가 너무 벌어져있는지 체크
    //벌어져있는 상태에서 발사를 누르면 일단 각을 정렬 후 발사해야할 것임.
    private bool m_linedUp => !(Mathf.Abs(m_playerCamera.transform.eulerAngles.y - transform.eulerAngles.y) > 1f);
    
    //벽에 가까이있으면 총구가 벽에 파묻히는데 이런 경우 발사하지 못하도록 강제할 것임.
    //고로 총의 개머리판 부분 ~ 총의 발사지점 까지 라인캐스트를 적용해 콜라이더가 존재하는지 판단한다.
    private bool m_hasEnoughDistance => !Physics.Linecast(transform.position + Vector3.up * m_gun.m_fireTransform.position.y, m_gun.m_fireTransform.position, ~m_excludeTarget);
    #endregion

    #region Unity Methods
    void Awake()
    {
        //혹시 자신의 레이어가 제외 레이어 대상에 추가되어있지 않다면 추가한다.
        if (m_excludeTarget != (m_excludeTarget | (1 << gameObject.layer)))
        {
            m_excludeTarget |= 1 << gameObject.layer;
        }
    }

    private void Start()
    {
        m_playerCamera = Camera.main;
        m_playerInput = GetComponent<PlayerInput>();
        m_playerAnimator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        m_aimState = AimState.Idle;
        m_gun.gameObject.SetActive(true);
        m_gun.Setup(this); //초기화
    }

    private void OnDisable()
    {
        m_aimState = AimState.Idle;
        m_gun.gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        if (m_playerInput.m_fire)
        {
            m_lastFireInputTime = Time.time;
            Shoot();
        }
        else if (m_playerInput.m_reload)
        {
            Reload();
        }
    }

    private void Update()
    {
        UpdateAimTarget();

        var angle = m_playerCamera.transform.eulerAngles.x;

        //-90도가 270도로 들어올 수도 있으니 아래와 같이 처리.
        if(angle > 270f)
        {
            angle -= 360f;
        }

        //-90~90도 범위를 0~1 범위의 값으로 변경
        angle = angle / -180f + 0.5f;
        //angle 값을 통해 상체 에이밍 애니메이션을 조절
        m_playerAnimator.SetFloat("Angle", angle);

        //fire가 눌리지않았으며, 2.5초 이상 눌리지 않고 있다면 idle로 설정해준다.
        if(!m_playerInput.m_fire && Time.time >= m_lastFireInputTime + m_waitingTimeForReleasingAim)
        {
            m_aimState = AimState.Idle;
        }

        UpdateUI();
    }
    #endregion

    #region Public Methods
    public void Shoot()
    {
        if(m_aimState == AimState.Idle)
        {
            //카메라와 정렬되어있는지 체크
            if(m_linedUp)
            {
                m_aimState = AimState.HipFire;
            }
        }
        else if (m_aimState == AimState.HipFire)
        {
            //총구가 파묻히진 않았는지 체크
            if(m_hasEnoughDistance)
            {
                if(m_gun.Fire(m_aimPoint))
                {
                    m_playerAnimator.SetTrigger("Shoot");
                }
            }
            else
            {
                m_aimState = AimState.Idle;
            }
        }
    }

    public void Reload()
    {
        if(m_gun.Reload())
        {
            m_playerAnimator.SetTrigger("Reload");
        }
    }
    #endregion

    #region Private Methods
    private void UpdateAimTarget()
    {
        RaycastHit hit;

        //뷰포트 즉 화면의 정중앙으로 레이를 발사함
        var ray = m_playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        //뷰포트 정중앙에서 발사한 레이와 충돌한 지점의 hit 포인트를 에임포인트로 지정
        if(Physics.Raycast(ray, out hit, m_gun.m_fireDistance, ~m_excludeTarget))
        {
            m_aimPoint = hit.point;

            //실제 캐릭터의 총구로 부터 위에서 지정한 에임포인트 까지 라인캐스트를 통해
            //콜라이더가 존재한다면 해당 hit 포인트를 실제 에임포인트로 지정해준다.
            if(Physics.Linecast(m_gun.m_fireTransform.position, hit.point, out hit, ~m_excludeTarget))
            {
                m_aimPoint = hit.point;
            }
        }
        //아무것도 감지되지 않았다면 최대사거리
        else
        {
            m_aimPoint = m_playerCamera.transform.position + m_playerCamera.transform.forward * m_gun.m_fireDistance;
        }
    }

    private void UpdateUI()
    {
        if (m_gun == null || UIManager.Instance == null) return;
        
        UIManager.Instance.UpdateAmmoText(m_gun.m_magAmmo, m_gun.m_ammoRemain);
        
        UIManager.Instance.SetActiveCrosshair(m_hasEnoughDistance);
        UIManager.Instance.UpdateCrossHairPosition(m_aimPoint);
    }
    #endregion

    #region IK
    private void OnAnimatorIK(int layerIndex)
    {
        //총이 없거나 재장전중이면 IK를 적용하지 않음
        if(m_gun == null || m_gun.m_state == Gun.State.Reloading)
        {
            return;
        }

        m_playerAnimator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1.0f);
        m_playerAnimator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1.0f);

        m_playerAnimator.SetIKPosition(AvatarIKGoal.LeftHand, m_gun.m_leftHandMount.position);
        m_playerAnimator.SetIKRotation(AvatarIKGoal.LeftHand, m_gun.m_leftHandMount.rotation);
    }
    #endregion
}