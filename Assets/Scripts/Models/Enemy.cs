using UnityEngine;

public class Enemy : MonoBehaviour, IPooledObject
{
    private float health;
    private float moveSpeed;
    private Transform player;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void OnObjectSpawn()
    {
        health = GameManager.Instance.enemyBaseHealth;
        moveSpeed = GameManager.Instance.enemyMoveSpeed;
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void FixedUpdate()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.velocity = direction * moveSpeed;
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        GameManager.Instance.AddKill();
        
        // 生成經驗道具
        ObjectPoolManager.Instance.SpawnFromPool(
            "ExpOrb",
            transform.position,
            Quaternion.identity
        );

        // 返回物件池
        ObjectPoolManager.Instance.ReturnToPool("Enemy", gameObject);
    }
} 