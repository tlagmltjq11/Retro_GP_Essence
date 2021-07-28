using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region Field
    private CharacterController m_characterController;
    private PlayerInput m_playerInput;
    private PlayerShooter m_playerShooter;
    private Animator m_animator;
    
    private Camera m_followCam; //메인 카메라
    
    public float m_speed = 6f; //움직임 최대속도
    public float m_jumpVelocity = 8f; //점프 속도

    //Range로 범위를 주어서 인스펙터 상에서 해당 범위 내에서만 값을 조정할 수 있도록 함.
    [Range(0.01f, 1f)] public float m_airControlPercent; //공중에 체류하는 동안 움직임의 속도를 원래 속도의 퍼센트로 구현

    //---------SmoothDamping에 사용할 변수들--------------
    public float m_speedSmoothTime = 0.1f;
    public float m_turnSmoothTime = 0.1f;
    
    private float m_speedSmoothVelocity;
    private float m_turnSmoothVelocity;
    //---------SmoothDamping에 사용할 변수들 끝-----------

    private float m_currentVelocityY; //중력을 적용하기 위해 Y 속도값을 컨트롤할 변수
    
    //외부에 플레이어의 현재 x,z만을 사용한 속도를 알려주기 위한 프로퍼티로 람다를 사용함
    public float m_currentSpeed =>
        new Vector2(m_characterController.velocity.x, m_characterController.velocity.z).magnitude;
    #endregion

    #region Unity Methods
    private void Start()
    {
        m_playerInput = GetComponent<PlayerInput>();
        m_playerShooter = GetComponent<PlayerShooter>();
        m_animator = GetComponent<Animator>();
        m_characterController = GetComponent<CharacterController>();
        m_followCam = Camera.main;
    }

    private void FixedUpdate()
    {
        //물리 갱신 주기에 맞춰서 실행되기 때문에 이동, 회전 관련된 코드를 넣기 적합하다.
        //즉 오차가 발생할 일이 적다.

        //TPS는 움직이거나 총을쏘는 경우가 아니라면 카메라와 플레이어 시점을 일치시키지 않음.
        //고로 움직이는 속도가 0.2f보다 크거나 총을쏠때 또는 견착자세중이라면 시점을 맞춰준다.
        if (m_currentSpeed > 0.2f || m_playerInput.m_fire || m_playerShooter.m_aimState == PlayerShooter.AimState.HipFire)
        {
            Rotate();
        }

        Move(m_playerInput.m_moveInput);

        if (m_playerInput.m_jump)
        {
            Jump();
        }
    }

    private void Update()
    {
        UpdateAnimation(m_playerInput.m_moveInput);
    }
    #endregion

    #region Public Methods
    public void Move(Vector2 moveInput)
    {
        //패드같은 경우 살짝 밀면 천천히가기에 다음과 같이 속력을 지정해준다.
        var targetSpeed = m_speed * moveInput.magnitude;
        //현재 플레이어의 정면을 기준으로 방향벡터를 구한다.
        var moveDir = Vector3.Normalize(transform.forward * moveInput.y + transform.right * moveInput.x);

        //댐핑
        //var smoothTime = characterController.isGrounded ? speedSmoothTime : speedSmoothTime / airControlPercent;
        //targetSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, smoothTime);
        
        //중력
        m_currentVelocityY += Time.deltaTime * Physics.gravity.y;

        //최종 이동 벡터를 구함.
        var velocity = moveDir * targetSpeed + Vector3.up * m_currentVelocityY;

        //이동할 수 없는 지점이면 알아서 멈추게는 점 주의
        m_characterController.Move(velocity * Time.deltaTime);

        //바닥에 닿아있는 경우면 누적되지않게 계속 초기화
        if(m_characterController.isGrounded)
        {
            m_currentVelocityY = 0;
        }
    }

    public void Rotate()
    {
        var targetRotation = m_followCam.transform.eulerAngles.y;

        //댐핑
        targetRotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetRotation, ref m_turnSmoothVelocity, m_turnSmoothTime);

        transform.eulerAngles = Vector3.up * targetRotation;
    }

    public void Jump()
    {
        if(!m_characterController.isGrounded)
        {
            return;
        }

        //점프하도록 y 벨로시티를 대입
        m_currentVelocityY = m_jumpVelocity;
    }
    #endregion

    #region Private Methods
    private void UpdateAnimation(Vector2 moveInput)
    {
        //현재속도가 최고속도 대비 몇퍼센트인지 구해서 해당 값을 파라메터에 적용
        //-> 실제속도에 기반해서 애니메이션이 블렌딩되거나 동작하도록 유도하는 것
        //var animationSpeedPercent = currentSpeed / speed;
        m_animator.SetFloat("Vertical Move", moveInput.y, 0.05f, Time.deltaTime);
        m_animator.SetFloat("Horizontal Move", moveInput.x, 0.05f, Time.deltaTime);
    }
    #endregion
}