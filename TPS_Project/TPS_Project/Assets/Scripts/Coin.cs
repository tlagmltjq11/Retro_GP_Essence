using UnityEngine;

public class Coin : MonoBehaviour, IItem
{
    #region Field
    public int m_score = 200;
    #endregion

    #region Public Methods
    public void Use(GameObject target)
    {
        GameManager.Instance.AddScore(m_score);

        Destroy(gameObject);
    }
    #endregion
}