using UnityEngine;
using UnityEngine.UI;

public class Crosshair : MonoBehaviour
{
    #region Field
    public Image m_aimPointReticle; //조준위치
    public Image m_hitPointReticle; //실제 타격위치

    public float m_smoothTime = 0.2f;
    
    private Camera m_screenCamera;
    private RectTransform m_crossHairRectTransform; //hitPointReticle에서 가져옴

    private Vector2 m_currentHitPointVelocity; //댐핑
    private Vector2 m_targetPoint; //월드포인트를 화면상의 포인트로 변경할 때 사용
    #endregion

    #region Unity Methods
    private void Awake()
    {
        m_screenCamera = Camera.main;
        m_crossHairRectTransform = m_hitPointReticle.GetComponent<RectTransform>();
    }

    private void Update()
    {
        //활성화된 순간에만 이동
        if(!m_hitPointReticle.enabled)
        {
            return;
        }

        //크로스헤어 UI를 넘겨받은 스크린좌표로 이동시키는 부분 (댐핑)
        m_crossHairRectTransform.position = Vector2.SmoothDamp(m_crossHairRectTransform.position, m_targetPoint, ref m_currentHitPointVelocity, m_smoothTime);
    }
    #endregion

    #region Public Methods
    public void SetActiveCrosshair(bool active)
    {
        m_hitPointReticle.enabled = active;
        m_aimPointReticle.enabled = active;
    }

    public void UpdatePosition(Vector3 worldPoint)
    {
        //매개변수로 받은 월드포지션을 스크린 포지션으로 변경
        m_targetPoint = m_screenCamera.WorldToScreenPoint(worldPoint);
    }
    #endregion
}