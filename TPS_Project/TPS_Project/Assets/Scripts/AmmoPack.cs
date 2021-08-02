using UnityEngine;

public class AmmoPack : MonoBehaviour, IItem
{
    #region Field
    public int m_ammo = 30;
    #endregion

    #region Public Methods
    public void Use(GameObject target)
    {
        var playerShooter = target.GetComponent<PlayerShooter>();

        //대상이 null이 아니고 대상이 총을 갖고있다면
        if(playerShooter != null && playerShooter.m_gun != null)
        {
            playerShooter.m_gun.m_ammoRemain += m_ammo;
        }

        Destroy(gameObject);
    }
    #endregion
}