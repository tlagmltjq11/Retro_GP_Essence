using UnityEngine;

public struct DamageMessage
{
    public GameObject m_damager; //공격을 가한 측
    public float m_amount; //공격 데미지 양

    public Vector3 m_hitPoint; //공격이 가해진 위치
    public Vector3 m_hitNormal; //노말벡터
}