using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnGenerator : MonoBehaviour
{
    #region Field
    public GameObject[] m_propPrefabs;
    public int m_count = 100;

    BoxCollider m_area;
    List<GameObject> m_props = new List<GameObject>(); //ObjPooling과 유사하게 사용하기 위해 리스트로 추적
    #endregion

    #region Unity Methods
    void Start()
    {
        m_area = GetComponent<BoxCollider>();

        for(int i=0; i<m_count; i++)
        {
            Spawn();
        }

        //범위 한번 알아냈으면 콜라이더 비활성
        m_area.enabled = false;
    }
    #endregion

    #region Private Methods
    private void Spawn()
    {
        //Prop 프리팹들 중 랜덤으로 하나를 선택
        int selection = Random.Range(0, m_propPrefabs.Length);
        GameObject selectedPrefab = m_propPrefabs[selection];

        Vector3 spawnPos = GetRandomPosition();
        GameObject instance = Instantiate(selectedPrefab, spawnPos, Quaternion.identity);

        m_props.Add(instance); //리스트에 등록
    }

    private Vector3 GetRandomPosition()
    {
        Vector3 basePosition = transform.position;
        Vector3 size = m_area.size;

        float posX = basePosition.x + Random.Range(-size.x / 2f, size.x / 2f);
        float posY = basePosition.y + Random.Range(-size.y / 2f, size.y / 2f);
        float posZ = basePosition.z + Random.Range(-size.z / 2f, size.z / 2f);

        return new Vector3(posX, posY, posZ);
    }
    #endregion

    #region Public Methods
    public void Reset()
    {
        for(int i=0; i<m_props.Count; i++)
        {
            m_props[i].transform.position = GetRandomPosition();
            m_props[i].SetActive(true);
        }
    }
    #endregion
}
