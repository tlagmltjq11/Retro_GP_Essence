using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    #region Field
    //키를 변경할때 코드를 직접 수정하지 않고, 변수의 값을 수정하도록 하여 좀 더 유연한 코드를 만들 수 있음.
    public string fireButtonName = "Fire1";
    public string jumpButtonName = "Jump";
    public string moveHorizontalAxisName = "Horizontal";
    public string moveVerticalAxisName = "Vertical";
    public string reloadButtonName = "Reload";

    //실제 사용자의 입력 값을 저장할 프로퍼티
    //외부에서 건들 수 없도록 set은 private
    public Vector2 moveInput { get; private set; }
    public bool fire { get; private set; }
    public bool reload { get; private set; }
    public bool jump { get; private set; }
    #endregion

    #region Unity Methods
    private void Update()
    {
        //GameOver인 경우 사용자 입력을 무시하도록 유도
        if(GameManager.Instance != null && GameManager.Instance.isGameover)
        {
            moveInput = Vector2.zero;
            fire = false;
            reload = false;
            jump = false;
            return;
        }

        //수직, 수평 방향 움직임에 대한 사용자 입력
        moveInput = new Vector2(Input.GetAxis(moveHorizontalAxisName), Input.GetAxis(moveVerticalAxisName));

        //대각선 입력이 들어왔을 경우 속도가 빨라지는 것을 대비하기 위해 정규화를 해주는 부분.
        //루트 연산을 배제해도 되는 상황이기 때문에, sqrMag를 사용해서 연산에서의 이득을 취함.
        if (moveInput.sqrMagnitude > 1)
        {
            moveInput = moveInput.normalized;
        }

        jump = Input.GetButtonDown(jumpButtonName);
        fire = Input.GetButton(fireButtonName);
        reload = Input.GetButtonDown(reloadButtonName);
    }
    #endregion
}