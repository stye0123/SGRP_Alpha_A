using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float lastAttackTime;
    private Transform nearestEnemy;
    private float nearestEnemyDistance;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        // 獲取輸入
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector2(moveX, moveY).normalized;

        // 尋找最近的敵人
        FindNearestEnemy();

        // 自動攻擊
        if (Time.time >= lastAttackTime + GameManager.Instance.playerAttackInterval)
        {
            // 確保敵人在攻擊範圍內
            if (nearestEnemy != null && nearestEnemyDistance <= GameManager.Instance.playerAttackRange)
            {
                Attack();
            }
        }

        // 添加调试信息
        if (nearestEnemy != null)
        {
            Debug.Log($"玩家位置: {transform.position}, 敌人位置: {nearestEnemy.position}, 距离: {nearestEnemyDistance}, 玩家速度: {rb.velocity}");
        }
    }

    private void FixedUpdate()
    {
        // 移動
        rb.velocity = moveDirection * GameManager.Instance.playerMoveSpeed;
    }

    private void FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        nearestEnemyDistance = float.MaxValue;
        nearestEnemy = null;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < nearestEnemyDistance)
            {
                nearestEnemyDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
    }

    private void Attack()
    {
        if (nearestEnemy != null)
        {
            // 計算方向向量
            Vector2 direction = (nearestEnemy.position - transform.position).normalized;
            
            // 計算角度
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // 設置子彈生成位置（稍微偏移以避免碰撞）
            Vector3 spawnPosition = transform.position + new Vector3(direction.x, direction.y, 0) * 0.5f;

            // 生成子彈
            GameObject bullet = ObjectPoolManager.Instance.SpawnFromPool(
                "Bullet",
                spawnPosition,
                Quaternion.Euler(0, 0, angle)
            );

            if (bullet != null)
            {
                Bullet bulletComponent = bullet.GetComponent<Bullet>();
                if (bulletComponent != null)
                {
                    // 初始化子彈
                    bulletComponent.Initialize(direction, GameManager.Instance.bulletSpeed, GameManager.Instance.CurrentAttack);
                }
            }

            lastAttackTime = Time.time;
        }
    }

    protected void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"触发器检测到物体: {other.gameObject.name}, 标签: {other.gameObject.tag}, 位置: {other.transform.position}");
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("检测到敌人触发器，增加受伤次数");
            GameManager.Instance.AddPlayerHit();
        }
    }
} 