using UnityEngine;

public class ExpOrb : MonoBehaviour, IPooledObject
{
    private Transform player;
    private float pickupRange;
    private float moveSpeed = 5f;

    public void OnObjectSpawn()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        pickupRange = GameManager.Instance.expPickupRange;
    }

    private void Update()
    {
        if (player != null)
        {
            float distance = Vector2.Distance(transform.position, player.position);
            
            if (distance <= pickupRange)
            {
                // 移動向玩家
                Vector2 direction = (player.position - transform.position).normalized;
                transform.position += (Vector3)(direction * moveSpeed * Time.deltaTime);

                // 如果非常接近玩家，則拾取
                if (distance <= 0.5f)
                {
                    Pickup();
                }
            }
        }
    }

    private void Pickup()
    {
        GameManager.Instance.AddAttackBonus();
        ObjectPoolManager.Instance.ReturnToPool("ExpOrb", gameObject);
    }
} 