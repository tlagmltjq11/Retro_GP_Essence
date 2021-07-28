using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Field
    //키를 변경할때 코드를 직접 수정하지 않고, 변수의 값을 수정하도록 하여 좀 더 유연한 코드를 만들 수 있음.
    public string m_fireButtonName = "Fire1";
    public string m_jumpButtonName = "Jump";
    public string m_moveHorizontalAxisName = "Horizontal";
    public string m_moveVerticalAxisName = "Vertical";
    public string m_reloadButtonName = "Reload";

    //실제 사용자의 입력 값을 저장할 프로퍼티
    //외부에서 건들 수 없도록 set은 private
    public Vector2 m_moveInput { get; private set; }
    public bool m_fire { get; private set; }
    public bool m_reload { get; private set; }
    public bool m_jump { get; private set; }
    #endregion

    #region Unity Methods
    private void Update()
    {
        //GameOver인 경우 사용자 입력을 무시하도록 유도
        if(GameManager.Instance != null && GameManager.Instance.isGameover)
        {
            m_moveInput = Vector2.zero;
            m_fire = false;
            m_reload = false;
            m_jump = false;
            return;
        }

        //수직, 수평 방향 움직임에 대한 사용자 입력
        m_moveInput = new Vector2(Input.GetAxis(m_moveHorizontalAxisName), Input.GetAxis(m_moveVerticalAxisName));

        //대각선 입력이 들어왔을 경우 속도가 빨라지는 것을 대비하기 위해 정규화를 해주는 부분.
        //루트 연산을 배제해도 되는 상황이기 때문에, sqrMag를 사용해서 연산에서의 이득을 취함.
        if (m_moveInput.sqrMagnitude > 1)
        {
            m_moveInput = m_moveInput.normalized;
        }

        m_jump = Input.GetButtonDown(m_jumpButtonName);
        m_fire = Input.GetButton(m_fireButtonName);
        m_reload = Input.GetButtonDown(m_reloadButtonName);
    }
    #endregion
}