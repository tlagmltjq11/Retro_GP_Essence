using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Prop : MonoBehaviour
{
    #region Field
    public ParticleSystem m_explosionParticle;
    public int m_score = 5;
    public float m_hp = 10f;
    #endregion

    #region Public Methods
    public void TakeDamage(float damage)
    {
        m_hp -= damage;

        if(m_hp <= 0)
        {
            ParticleSystem instance = Instantiate(m_explosionParticle, transform.position, transform.rotation);
            AudioSource explosionAudio = instance.GetComponent<AudioSource>();
            instance.Play();
            explosionAudio.Play();

            GameManager.instance.AddScore(m_score);

            Destroy(instance.gameObject, instance.duration);
            gameObject.SetActive(false);
        }
    }
    #endregion
}
