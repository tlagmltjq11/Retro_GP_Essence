using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BallShooter : MonoBehaviour
{
    #region Field
    public CamFollow m_cam;
    public Rigidbody m_ball;
    public Transform m_firePos;
    public Slider m_powerSlider;
    public AudioSource m_shootingAudio;
    public AudioClip m_fireClip;
    public AudioClip m_chargingClip;
    public float m_minForce = 15f;
    public float m_maxForce = 30f;
    public float m_chargingTime = 0.75f;

    float m_curForce;
    float m_chargeSpeed;
    bool m_isFired;
    #endregion

    #region Unity Methods
    void OnEnable()
    {
        //초기화
        m_curForce = m_minForce;
        m_powerSlider.minValue = m_minForce;
        m_isFired = false;
    }

    void Start()
    {
        //거리 / 시간 을 이용해서 1초에 힘이 얼마나 충전되는지 계산
        m_chargeSpeed = (m_maxForce - m_minForce) / m_chargingTime;
    }

    void Update()
    {
        if(m_isFired == true)
        {
            return;
        }

        if(m_curForce >= m_maxForce && !m_isFired)
        {
            m_curForce = m_maxForce;
            Fire();
        }
        else if(Input.GetButtonDown("Fire1"))
        {
            m_curForce = m_minForce;
            m_shootingAudio.clip = m_chargingClip;
            m_shootingAudio.Play();
        }
        else if(Input.GetButton("Fire1") && !m_isFired)
        {
            m_curForce = m_curForce + m_chargeSpeed * Time.deltaTime;
            m_powerSlider.value = m_curForce;
        }
        else if(Input.GetButtonUp("Fire1") && !m_isFired)
        {
            Fire();
        }
    }
    #endregion

    #region Private Methods
    private void Fire()
    {
        m_isFired = true;
        Rigidbody ball = Instantiate(m_ball, m_firePos.position, m_firePos.rotation);

        ball.velocity = m_curForce * m_firePos.forward;

        m_shootingAudio.clip = m_fireClip;
        m_shootingAudio.Play();

        m_curForce = m_minForce;
        m_powerSlider.value = m_minForce;

        m_cam.SetTarget(ball.transform, CamFollow.State.Tracking);
    }
    #endregion
}
