using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBox : MonoBehaviour
{
    #region Field
    Renderer m_renderer;

    public bool m_isOverlaped = false; //EndPoint와 겹쳐있는지 판별.
    public Color m_touchColor;
    private Color m_originColor;
    #endregion

    #region Unity
    void Start()
    {
        m_renderer = GetComponent<Renderer>();
        m_originColor = m_renderer.material.color;
    }

    private void OnTriggerEnter(Collider other)
    {
        //CompareTag를 사용하는 것이 더 효율적임.
        if(other.CompareTag("EndPoint"))
        {
            m_isOverlaped = true;
            m_renderer.material.color = m_touchColor;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            m_isOverlaped = false;
            m_renderer.material.color = m_originColor;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("EndPoint"))
        {
            m_isOverlaped = true;
            m_renderer.material.color = m_touchColor;
        }
    }
    #endregion
}
