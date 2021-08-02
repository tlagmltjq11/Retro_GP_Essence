using UnityEngine;
using UnityEngine.AI;

public class ItemSpawner : MonoBehaviour
{
    #region Field
    public GameObject[] m_items; //아이템 프리팹들
    public Transform m_playerTransform; //플레이어의 현재 위치
    
    private float m_lastSpawnTime; //가장 마지막에 아이템을 생성한 시간
    public float m_maxDistance = 5f; //아이템 생성위치 최대 반경
    
    private float m_timeBetSpawn; //다음 생성시간 까지의 대기시간

    //랜덤 결정할때 사용
    public float m_timeBetSpawnMax = 7f;
    public float m_timeBetSpawnMin = 2f;
    #endregion

    #region Unity Methods
    private void Start()
    {
        m_timeBetSpawn = Random.Range(m_timeBetSpawnMin, m_timeBetSpawnMax);
        m_lastSpawnTime = 0f;
    }

    private void Update()
    {
        if(Time.time >= m_lastSpawnTime + m_timeBetSpawn && m_playerTransform != null)
        {
            Spawn();

            m_lastSpawnTime = Time.time;
            m_timeBetSpawn = Random.Range(m_timeBetSpawnMin, m_timeBetSpawnMax);
        }
    }
    #endregion

    #region Private Methods
    private void Spawn()
    {
        //플레이어 위치를 기준으로 랜덤 네브메쉬 포지션을 받아옴.
        var spawnPosition = Utility.GetRandomPointOnNavMesh(m_playerTransform.position, m_maxDistance, NavMesh.AllAreas);
        spawnPosition += Vector3.up * 0.5f; //바닥에 박혀서 생성되지 않도록 올려줌

        var item = Instantiate(m_items[Random.Range(0, m_items.Length)], spawnPosition, Quaternion.identity);
        Destroy(item, 5f); //5초 뒤에 자동파괴 -> 쌓이지 않도록
    }
    #endregion
}