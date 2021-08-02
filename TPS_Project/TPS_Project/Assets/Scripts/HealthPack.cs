using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    #region Field
    public float m_health = 50;
    #endregion

    #region Public Methods
    public void Use(GameObject target)
    {
        var livingEntity = target.GetComponent<LivingEntity>();
        
        if(livingEntity != null)
        {
            livingEntity.RestoreHealth(m_health);
        }

        Destroy(gameObject);
    }
    #endregion
}