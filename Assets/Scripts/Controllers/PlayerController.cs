using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Vector2 moveDirection;
    private float lastAttackTime;
    private Transform nearestEnemy;

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

        // 自動攻擊
        if (Time.time >= lastAttackTime + GameManager.Instance.playerAttackInterval)
        {
            FindNearestEnemy();
            if (nearestEnemy != null)
            {
                Attack();
            }
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
        float nearestDistance = float.MaxValue;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy.transform;
            }
        }
    }

    private void Attack()
    {
        if (nearestEnemy != null)
        {
            Vector2 direction = (nearestEnemy.position - transform.position).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            GameObject bullet = ObjectPoolManager.Instance.SpawnFromPool(
                "Bullet",
                transform.position,
                Quaternion.Euler(0, 0, angle)
            );

            if (bullet != null)
            {
                Bullet bulletComponent = bullet.GetComponent<Bullet>();
                if (bulletComponent != null)
                {
                    bulletComponent.Initialize(direction, GameManager.Instance.bulletSpeed, GameManager.Instance.CurrentAttack);
                }
            }

            lastAttackTime = Time.time;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameManager.Instance.AddPlayerHit();
        }
    }
} 