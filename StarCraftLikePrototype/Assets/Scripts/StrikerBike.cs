using UnityEngine;

public class StrikerBike : Unit
{
    protected override void Start()
    {
        base.Start();
        moveSpeed = 10f;
        attackRange = 10f;
        fireRate = 2f; // 0.5 seconds per shot
        damage = 5f;
        hitPoints = 100f;
    }
}
