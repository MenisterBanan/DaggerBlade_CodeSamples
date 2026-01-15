using UnityEngine;

public class Bow : MonoBehaviour
{
    [SerializeField] ArrowProjectile arrowPrefab;
    [SerializeField] EnemyController enemyController;
    [SerializeField] Transform arrowSpawnPos;
    Transform playerTransform;
    GameObject arrowParent;
    Animator animator;

    [SerializeField] float attackSpeed;
    [SerializeField] float arrowSpeed;
    [SerializeField] float range;
    float attackTimer;

    [SerializeField] SoundType BowAttack;

    private void Awake()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        arrowParent = GameObject.FindGameObjectWithTag("Trash").gameObject;
        animator = GetComponent<Animator>();

    }
    private void Update()
    {
        TryShootAroow();
    }

    public void ShootArrow()
    {
        Vector3 arrowDirection = playerTransform.position - arrowSpawnPos.position;
        arrowDirection += new Vector3(0, 0.8f, 0);
        arrowDirection.Normalize();
        ArrowProjectile arrow = Instantiate(arrowPrefab, arrowSpawnPos.position, Quaternion.identity, arrowParent.transform);
        arrow.transform.up = arrowDirection;
        arrow.GetComponent<Rigidbody>().linearVelocity = arrowDirection * arrowSpeed;
        ArrowProjectile arrowScript = arrow.GetComponent<ArrowProjectile>();
        arrowScript.shooter = enemyController;
        FModSoundManager.Instance.PlaySound(BowAttack);

    }


    void TryShootAroow()
    {

        float distance = Vector3.Distance(transform.position, playerTransform.position);

        if (Time.time > attackTimer && range >= distance)
        {
            attackTimer = Time.time + attackSpeed;

            animator.SetTrigger("shoot");
        }
    }

    public void ShootAnimation()
    {
        ShootArrow();
    }
}
