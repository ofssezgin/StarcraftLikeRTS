using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Unit : MonoBehaviour
{
    public float moveSpeed;
    public float attackRange;
    public float fireRate;
    public float damage;
    public float hitPoints;
    public GameObject bulletPrefab;
    public Transform bulletSpawnPoint;
    public Material highlightMaterial;
    public Material selectionMaterial;

    private Material originalMaterial;
    private bool isSelected = false;
    private bool isHighlighted = false;
    private Transform targetEnemy;
    private bool isMoving = false;
    private bool isAttacking = false;
    private Vector3 targetPosition;
    private float nextFireTime = 0f;
    private Camera mainCamera;

    protected virtual void Start()
    {
        mainCamera = Camera.main;
        originalMaterial = GetComponent<MeshRenderer>().material;
    }

    protected virtual void Update()
    {
        HandleHighlight();
        HandleSelection();
        HandleCommands();
        HandleMovement();
        HandleAttack();
    }

    private void HandleHighlight()
    {
        if (isSelected) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            if (raycastHit.transform == transform)
            {
                if (!isHighlighted)
                {
                    GetComponent<MeshRenderer>().material = highlightMaterial;
                    isHighlighted = true;
                }
            }
            else
            {
                if (isHighlighted)
                {
                    GetComponent<MeshRenderer>().material = originalMaterial;
                    isHighlighted = false;
                }
            }
        }
        else
        {
            if (isHighlighted)
            {
                GetComponent<MeshRenderer>().material = originalMaterial;
                isHighlighted = false;
            }
        }
    }

    private void HandleSelection()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                if (raycastHit.transform == transform)
                {
                    isSelected = true;
                    GetComponent<MeshRenderer>().material = selectionMaterial;
                }
                else if (isSelected)
                {
                    isSelected = false;
                    GetComponent<MeshRenderer>().material = originalMaterial;
                }
            }
        }
    }

    private void HandleCommands()
    {
        if (Input.GetKey(KeyCode.Mouse1) && isSelected)
        {
            if (Physics.Raycast(mainCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit raycastHit))
            {
                if (raycastHit.transform.CompareTag("Enemy"))
                {
                    Enemy enemy = raycastHit.transform.GetComponent<Enemy>();
                    if (enemy != null && !enemy.IsDead())
                    {
                        AttackTarget(raycastHit.transform);
                    }
                }
                else
                {
                    MoveTo(raycastHit.point);
                }
            }
        }
    }

    private void HandleMovement()
    {
        if (isMoving && targetEnemy != null)
        {
            LookAtTarget(targetEnemy.position);
            float step = moveSpeed * Time.deltaTime;
            Vector3 targetPositionWithNoY = new Vector3(targetEnemy.position.x, transform.position.y, targetEnemy.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPositionWithNoY, step);

            if (Vector3.Distance(transform.position, targetEnemy.position) <= attackRange)
            {
                isMoving = false;
                isAttacking = true;
            }
        }
        else if (isMoving)
        {
            LookAtTarget(targetPosition);
            float step = moveSpeed * Time.deltaTime;
            Vector3 targetPositionWithNoY = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPositionWithNoY, step);

            if (Vector3.Distance(transform.position, targetPositionWithNoY) < 0.1f)
            {
                isMoving = false;
            }
        }
    }

    private void HandleAttack()
    {
        if (isAttacking && targetEnemy != null)
        {
            LookAtTarget(targetEnemy.position);

            if (Vector3.Distance(transform.position, targetEnemy.position) <= attackRange)
            {
                if (Time.time >= nextFireTime)
                {
                    Debug.Log(gameObject.name + " is shooting!");

                    FireBullet();

                    nextFireTime = Time.time + 1f / fireRate;

                    Enemy enemy = targetEnemy.GetComponent<Enemy>();
                    if (enemy != null && enemy.hitPoints <= 0)
                    {
                        isAttacking = false;
                        targetEnemy = null;
                        DestroyRemainingBullets();
                    }
                }
            }
        }
    }

     private void FireBullet()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Bullet bulletScript = bullet.GetComponent<Bullet>();
            if (bulletScript != null)
            {
                bulletScript.SetTarget(targetEnemy);
                bulletScript.SetDamage(damage);
            }
        }
    }

    private void DestroyRemainingBullets()
    {
        foreach (Bullet bullet in FindObjectsOfType<Bullet>())
        {
            if (bullet != null && bullet.gameObject != null)
            {
                bullet.OnTargetDestroyed();
            }
        }
    }

    private void LookAtTarget(Vector3 target)
    {
        Vector3 direction = (target - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * moveSpeed);
    }

    public void MoveTo(Vector3 position)
    {
        targetPosition = position;
        isMoving = true;
        targetEnemy = null;
        isAttacking = false;
    }

    public void AttackTarget(Transform enemy)
    {
        targetEnemy = enemy;
        if (Vector3.Distance(transform.position, enemy.position) <= attackRange)
        {
            isMoving = false;
            isAttacking = true;
        }
        else
        {
            isMoving = true;
            isAttacking = false;
        }
    }
}
