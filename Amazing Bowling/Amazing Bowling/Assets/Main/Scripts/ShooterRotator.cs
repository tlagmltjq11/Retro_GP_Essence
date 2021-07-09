using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterRotator : MonoBehaviour
{
    #region Field
    private enum RotateState
    {
        Idle,
        Vertical,
        Horizontal,
        Ready
    }

    private RotateState m_state = RotateState.Idle;
    public float m_verticalRotateSpeed = 360f;
    public float m_horizontalRotateSpeed = 360f;

    public BallShooter m_ballShooterScript;
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        //초기화
        transform.rotation = Quaternion.identity;
        m_state = RotateState.Idle;
        m_ballShooterScript.enabled = false;   
    }

    void Update()
    {
        switch(m_state)
        {
            case RotateState.Idle:
                if(Input.GetButtonDown("Fire1"))
                {
                    m_state = RotateState.Horizontal;
                }
                break;

            case RotateState.Horizontal:
                if(Input.GetButton("Fire1"))
                {
                    transform.Rotate(new Vector3(0f, m_horizontalRotateSpeed * Time.deltaTime, 0f));
                }
                else if(Input.GetButtonUp("Fire1"))
                {
                    m_state = RotateState.Vertical;
                }
                break;

            case RotateState.Vertical:
                if (Input.GetButton("Fire1"))
                {
                    transform.Rotate(new Vector3(-m_verticalRotateSpeed * Time.deltaTime, 0f, 0f));
                }
                else if (Input.GetButtonUp("Fire1"))
                {
                    m_state = RotateState.Ready;
                    m_ballShooterScript.enabled = true;
                }
                break;

            case RotateState.Ready:
                break;

            default:
                break;
        }
    }
    #endregion
}
