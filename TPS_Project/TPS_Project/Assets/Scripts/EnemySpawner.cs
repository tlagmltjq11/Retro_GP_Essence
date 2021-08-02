using System.Collections.Generic;
using UnityEngine;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour
{
    #region Field
    //현재 씬에 존재하는 좀비들의 리스트
    private readonly List<Enemy> m_enemies = new List<Enemy>();

    public float m_damageMax = 40f;
    public float m_damageMin = 20f;
    public Enemy m_enemyPrefab; //좀비 프리팹

    public float m_healthMax = 200f;
    public float m_healthMin = 100f;

    public Transform[] m_spawnPoints; //생성 위치

    public float m_speedMax = 12f;
    public float m_speedMin = 3f;

    public Color m_strongEnemyColor = Color.red; //강한 좀비일수록 빨갛게 표현할 것임.
    private int m_wave; //웨이브가 늘어날 수록 물량이 많아지도록 할 것임.
    #endregion

    #region Unity Methods
    private void Update()
    {
        //게임오버 상태 시 리턴
        if (GameManager.Instance != null && GameManager.Instance.isGameover) return;
        
        //모든 좀비가 죽었다면 다음 웨이브로 넘어감.
        if (m_enemies.Count <= 0) SpawnWave();
        
        UpdateUI();
    }
    #endregion

    #region Public Methods
    private void UpdateUI()
    {
        UIManager.Instance.UpdateWaveText(m_wave, m_enemies.Count);
    }
    #endregion

    #region Private Methods
    private void SpawnWave()
    {
        m_wave++; //웨이브 증가

        var spawnCount = Mathf.RoundToInt(m_wave * 5f); //웨이브 x 5만큼 좀비 생성, 반올림

        for(var i=0; i<spawnCount; i++)
        {
            //랜덤한 능력치 설정
            var enemyIntensity = Random.Range(0f, 1f);

            //좀비 생성
            CreateEnemy(enemyIntensity);
        }
    }
    
    private void CreateEnemy(float intensity)
    {
        //보간을 사용해서 min ~ max 범위내에 intensity 퍼센트 값을 얻어낸다.
        var health = Mathf.Lerp(m_healthMin, m_healthMax, intensity);
        var damage = Mathf.Lerp(m_damageMin, m_damageMax, intensity);
        var speed = Mathf.Lerp(m_speedMin, m_speedMax, intensity);
        var skinColor = Color.Lerp(Color.white, m_strongEnemyColor, intensity);

        //위치 랜덤 설정
        var spawnPoint = m_spawnPoints[Random.Range(0, m_spawnPoints.Length)];

        //좀비 생성
        var enemy = Instantiate(m_enemyPrefab, spawnPoint.position, spawnPoint.rotation);

        //능력치 초기화
        enemy.Setup(health, damage, speed, speed * 0.3f, skinColor);
        m_enemies.Add(enemy); //좀비 리스트에 추가

        //Die 이벤트 콜백메서드를 익명메서드로 추가
        enemy.OnDeath += () => m_enemies.Remove(enemy);
        enemy.OnDeath += () => Destroy(enemy.gameObject, 10f);
        enemy.OnDeath += () => GameManager.Instance.AddScore(100);
    }
    #endregion
}