using UnityEngine;
using System.Collections;

public class EnemySpawnSystem : MonoBehaviour
{
    private Transform player;
    private float spawnTimer;
    private float currentSpawnRate;
    private float spawnRateIncreaseInterval = 10f; // 每10秒增加生成率
    private float lastSpawnRateIncrease;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        currentSpawnRate = GameManager.Instance.enemySpawnRateStart;
        lastSpawnRateIncrease = Time.time;
        StartCoroutine(SpawnEnemies());
    }

    private void Update()
    {
        // 逐漸增加生成率
        if (Time.time >= lastSpawnRateIncrease + spawnRateIncreaseInterval)
        {
            currentSpawnRate = Mathf.Min(
                currentSpawnRate + 0.1f,
                GameManager.Instance.enemySpawnRateEnd
            );
            lastSpawnRateIncrease = Time.time;
        }
    }

    private IEnumerator SpawnEnemies()
    {
        while (true)
        {
            if (GameManager.Instance.CurrentState == GameManager.GameState.Playing)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(1f / currentSpawnRate);
        }
    }

    private void SpawnEnemy()
    {
        if (player == null) return;

        // 在玩家周圍隨機位置生成敵人
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        Vector3 spawnPosition = player.position + (Vector3)(randomDirection * GameManager.Instance.enemySpawnDistance);

        ObjectPoolManager.Instance.SpawnFromPool(
            "Enemy",
            spawnPosition,
            Quaternion.identity
        );
    }
} 