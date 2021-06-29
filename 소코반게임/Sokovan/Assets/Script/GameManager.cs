using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Field
    public ItemBox[] m_itemBoxes;
    public bool m_isGameOver;

    public GameObject m_winUI;
    #endregion

    #region Unity
    void Start()
    {
        m_isGameOver = false;
    }

    void Update()
    {
        //Restart
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Main");
        }

        if(m_isGameOver == true)
        {
            return;
        }

        //싱글턴 패턴도 적용하지 않았기 때문에, 강의에서는 콜백 식으로 Update문의 최소화를 하지 않은듯.
        int cnt = 0;
        for(int i=0; i<3; i++)
        {
            if(m_itemBoxes[i].m_isOverlaped == true)
            {
                cnt++;
            }
        }

        if(cnt >= 3)
        {
            m_isGameOver = true;
            m_winUI.SetActive(true);
        }
    }
    #endregion
}
