using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    #region Field
    public ParticleSystem m_explosionParticle;
    public AudioSource m_explosionAudio;

    public float m_maxDamage = 100f;
    public float m_explosionForce = 1000f;
    public float m_explosionRadius = 20f;
    public float m_lifeTime = 10f;
    #endregion

    #region Unity Methods
    void Start()
    {
        Destroy(gameObject, m_lifeTime);
    }

    private void OnDestroy()
    {
        GameManager.instance.OnBallDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {
        //폭발 반경 내 Prop들을 받아옴.
        int layerMask = 1 << LayerMask.NameToLayer("Prop");
        Collider[] cols = Physics.OverlapSphere(transform.position, m_explosionRadius, layerMask);

        for(int i=0; i<cols.Length; i++)
        {
            Rigidbody targetRigid = cols[i].GetComponent<Rigidbody>();
            targetRigid.AddExplosionForce(m_explosionForce, transform.position, m_explosionRadius);

            Prop targetProp = cols[i].GetComponent<Prop>();
            float damage = CalculateDamage(cols[i].transform.position);
            targetProp.TakeDamage(damage);
        }

        //Ball이 삭제되면 이펙트도 삭제되니 계층관계를 해제
        m_explosionParticle.transform.parent = null;

        m_explosionParticle.Play();
        m_explosionAudio.Play();

        Destroy(m_explosionParticle.gameObject, m_explosionParticle.duration);
        Destroy(gameObject); //Ball 삭제
    }
    #endregion

    #region Private Methods
    private float CalculateDamage(Vector3 targetPosition)
    {
        Vector3 explosionToTarget = targetPosition - transform.position;
        float dist = explosionToTarget.magnitude; //거리

        //폭발 지점과 얼마나 가까운지 계산
        float percentage = (m_explosionRadius - dist) / m_explosionRadius;

        //만약 오브젝트의 원점은 반경 밖인데 콜라이더 모서리가 겹쳐서 인지되는 경우
        //오히려 -% 로 계산되어 체력이 회복될 수 있다. 고로 데미지의 범위를 정해줌.
        return Mathf.Clamp(m_maxDamage * percentage, 0, m_maxDamage);
    }
    #endregion
}
