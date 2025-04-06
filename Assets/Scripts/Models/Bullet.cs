using UnityEngine;

public class Bullet : MonoBehaviour, IPooledObject
{
    private Vector2 direction;
    private float speed;
    private float damage;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Initialize(Vector2 direction, float speed, float damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
    }

    public void OnObjectSpawn()
    {
        // 重置子彈狀態
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    private void Update()
    {
        // 使用Transform直接移動子彈
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            ObjectPoolManager.Instance.ReturnToPool("Bullet", gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        // 當子彈離開畫面時返回物件池
        ObjectPoolManager.Instance.ReturnToPool("Bullet", gameObject);
    }
} 