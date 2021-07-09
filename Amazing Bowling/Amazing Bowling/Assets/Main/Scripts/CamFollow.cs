using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour
{
    #region Field
    public enum State
    {
        Idle,
        Ready,
        Tracking
    }

    private State m_state
    {
        set
        {
            switch (value)
            {
                case State.Idle:
                    m_targetZoomSize = m_roundReadyZoomSize;
                    break;

                case State.Ready:
                    m_targetZoomSize = m_readyShootZoomSize;
                    break;

                case State.Tracking:
                    m_targetZoomSize = m_trackingZoomSize;
                    break;
            }
        }
    }

    public float m_smoothTime = 0.2f;

    public Transform m_target;
    Vector3 m_lastMovingVelocity;
    Vector3 m_targetPosition;
    Camera m_cam;
    float m_targetZoomSize = 5f;
    const float m_roundReadyZoomSize = 14.5f;
    const float m_readyShootZoomSize = 5f;
    const float m_trackingZoomSize = 10f;
    float m_lastZoomSpeed;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_cam = GetComponentInChildren<Camera>();
        m_state = State.Idle;
    }

    void FixedUpdate()
    {
        if(m_target != null)
        {
            Move();
            Zoom();
        }
    }
    #endregion

    #region Private Methods
    private void Move()
    {
        m_targetPosition = m_target.position;

        Vector3 smoothPosition = Vector3.SmoothDamp(transform.position, m_targetPosition, ref m_lastMovingVelocity, m_smoothTime);

        transform.position = smoothPosition;
    }

    private void Zoom()
    {
        float smoothZoomSize = Mathf.SmoothDamp(m_cam.orthographicSize, m_targetZoomSize, ref m_lastZoomSpeed, m_smoothTime);

        m_cam.orthographicSize = smoothZoomSize;
    }
    #endregion

    #region Public Methods
    public void Reset()
    {
        m_state = State.Idle;
    }

    public void SetTarget(Transform newTarget, State newState)
    {
        m_target = newTarget;
        m_state = newState;
    }
    #endregion
}
