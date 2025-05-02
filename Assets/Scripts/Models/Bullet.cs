using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour, IPooledObject
{
    private Vector2 direction;
    private float speed;
    private float damage;
    private Rigidbody2D rb;
    private TrailRenderer trailRenderer;
    private bool isDisappearing;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();//取得子彈的剛體
        trailRenderer = GetComponent<TrailRenderer>();//取得子彈的trail
    }
//這邊是子彈的初始化
    public void Initialize(Vector2 direction, float speed, float damage)
    {
        this.direction = direction;
        this.speed = speed;
        this.damage = damage;
        isDisappearing = false;
        
        // 重置 TrailRenderer，在每次初始化時重置
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = true;
        }
    }

    public void OnObjectSpawn()
    {
        // 重置子彈狀態
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        isDisappearing = false;
        
        // 重置 TrailRenderer
        if (trailRenderer != null)
        {
            trailRenderer.Clear();
            trailRenderer.enabled = true;
        }
    }

    private void Update()
    {
        if (!isDisappearing)
        {
            // 使用Transform直接移動子彈
            transform.position += (Vector3)(direction * speed * Time.deltaTime);
        }
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
            StartDisappearing();
        }
    }

    private void OnBecameInvisible()
    {
        StartDisappearing();
    }

    private void StartDisappearing()
    {
        if (isDisappearing) return;
        
        isDisappearing = true;
        // 停止子彈移動
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
        // 等待 TrailRenderer 播放完畢
        if (trailRenderer != null)
        {
            StartCoroutine(WaitForTrail());
        }
        else
        {
            ReturnToPool();
        }
    }

    private System.Collections.IEnumerator WaitForTrail()
    {
        // 等待 TrailRenderer 的時間
        yield return new WaitForSeconds(trailRenderer.time);
        
        // 禁用 TrailRenderer
        trailRenderer.enabled = false;
        
        // 返回物件池
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        ObjectPoolManager.Instance.ReturnToPool("Bullet", gameObject);
    }
} 