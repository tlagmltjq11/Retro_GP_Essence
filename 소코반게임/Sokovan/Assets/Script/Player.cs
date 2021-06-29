using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    #region Field
    [SerializeField]
    float m_speed = 4f;
    Rigidbody m_rigid;

    public GameManager m_gameManager;
    #endregion

    #region Unity Methods
    void Start()
    {
        m_rigid = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if(m_gameManager.m_isGameOver == true)
        {
            return;
        }

        float inputX = Input.GetAxis("Horizontal");
        float inputZ = Input.GetAxis("Vertical");

        //관성때문에 힘이 누적되며 속도가 되기 때문에, 조작이 쉽지 않다.(누적 때문에 누를수록 계속 빨라지는 문제까지)
        //m_rigid.AddForce(inputX * m_speed, 0f, inputZ * m_speed);
        // -> 그렇기 때문에 velocity에 곧바로 속도를 지정해주는 방식을 사용.
        float m_gravity = m_rigid.velocity.y; //0f값으로 지정하면 중력을 제대로 받지 않기 때문에, 떨어지는 속도를 유지시켜준다.
        m_rigid.velocity = new Vector3(inputX * m_speed, m_gravity, inputZ * m_speed);
    }
    #endregion
}
