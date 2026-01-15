using Unity.VisualScripting;
using UnityEngine;

public class RangedEnemy : EnemyController
{
    //[SerializeField] Transform armRotate;
    Transform playerPosition;
    Bow bowScript;
    public float rotationSpeed = 5f;
    protected override void Awake()
    {
        base.Awake();
        playerPosition = GameObject.FindGameObjectWithTag("Player").transform;
        bowScript = GetComponent<Bow>();


    }

    private void Update()
    {
        EnemyBodyRotate();
    }


    void EnemyArmRotate()
    {
        Vector3 bowDirection = playerPosition.position - transform.position;
        bowDirection += new Vector3(0, 1.5f, 0); // offset because its targeting the foot
        armRotate.right = bowDirection.normalized;
    }

    void EnemyBodyRotate()
    {
        Vector3 dirToPlayer = playerPosition.position - transform.position;
        dirToPlayer.y = 0;
        dirToPlayer.z = 0;
        transform.right = dirToPlayer;
    }

}
