using UnityEngine;

public class HeavyTank : Unit
{
    protected override void Start()
    {
        base.Start();
        moveSpeed = 3f;
        attackRange = 30f;
        fireRate = 0.67f; // 1.5 seconds per shot
        damage = 20f;
        hitPoints = 900f;
    }
}
