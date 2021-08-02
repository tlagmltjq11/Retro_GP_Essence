using UnityEngine;

public class Rotator : MonoBehaviour
{
    #region Field
    public float m_rotationSpeed = 60f;
    #endregion

    #region Unity Methods
    private void Update()
    {
        transform.Rotate(0f, m_rotationSpeed * Time.deltaTime, 0f);
    }
    #endregion
}