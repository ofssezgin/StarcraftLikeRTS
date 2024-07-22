using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public float hitPoints = 500f;
    public Material deadMaterial;
    private bool isDead = false;
    private Material originalMaterial;

    private void Start()
    {
        originalMaterial = GetComponent<MeshRenderer>().material;
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        hitPoints -= damage;

        if (hitPoints <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        GetComponent<MeshRenderer>().material = deadMaterial;
        gameObject.tag = "Destroyed";
        Debug.Log(gameObject.name + " is destroyed!");
    }

    public bool IsDead()
    {
        return isDead;
    }
}
