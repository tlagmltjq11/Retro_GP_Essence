using UnityEngine;

public class EffectManager : MonoBehaviour
{
    #region Field
    private static EffectManager m_Instance;
    public static EffectManager Instance
    {
        get
        {
            if (m_Instance == null) m_Instance = FindObjectOfType<EffectManager>();
            return m_Instance;
        }
    }

    public enum EffectType
    {
        Common,
        Flesh
    }
    
    public ParticleSystem m_commonHitEffectPrefab; //보통의 경우 사용할 프리팹
    public ParticleSystem m_fleshHitEffectPrefab; //생명체를 대상으로 사용할 프리팹
    #endregion

    #region Public Methods
    public void PlayHitEffect(Vector3 pos, Vector3 normal, Transform parent = null, EffectType effectType = EffectType.Common)
    {
        var targetPrefab = m_commonHitEffectPrefab;

        //타입이 추가된다면 분기문을 추가해주면 됨.
        if(effectType == EffectType.Flesh)
        {
            targetPrefab = m_fleshHitEffectPrefab;
        }

        //노말을 바라보는 방향으로 회전값을 지정!!
        var effect = Instantiate(targetPrefab, pos, Quaternion.LookRotation(normal));

        if(parent != null)
        {
            effect.transform.SetParent(parent);
        }

        effect.Play();
    }
    #endregion
}