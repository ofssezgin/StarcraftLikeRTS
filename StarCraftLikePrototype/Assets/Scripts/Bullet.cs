using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody rb;

    [Header("Attributes")]
    [SerializeField] private float bulletSpeed = 5f;
    private Transform target;
    private bool targetDestroyed = false;
    private float damage;


    public void SetTarget(Transform _target)
    {
        target = _target;
    }
    
    public void SetDamage(float _damage)
    {
        damage = _damage;
    }

    private void FixedUpdate()
    {
        if (!target || targetDestroyed)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 direction = (target.position - transform.position).normalized;
        rb.velocity = direction * bulletSpeed;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Enemy enemy = other.gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
            Destroy(gameObject);
        }
    }

    public void OnTargetDestroyed()
    {
        targetDestroyed = true;
    }
}
