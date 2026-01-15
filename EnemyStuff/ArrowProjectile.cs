using UnityEngine;

public class ArrowProjectile : MonoBehaviour
{
    Rigidbody rb;

    public EnemyController shooter = null;
    Transform playerTransform;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void Update()
    {
        DestroyArrowWhenFarAway();
    }
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            // things done to player when hit 
            collision.gameObject.GetComponent<PlayerController>().PlayerHealth.TakeDamage(1);

            Destroy(gameObject);
        }
        else
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground2"))
            {
                Destroy(gameObject);
            }
        }
        
        if (collision.gameObject.layer == LayerMask.NameToLayer("Weapon"))
        {
            rb.linearVelocity *= -1.5f;
            rb.rotation = Quaternion.Euler(0, 0, rb.rotation.eulerAngles.z + 180);
            gameObject.tag = "Deflect";
        }
      
        if (collision.gameObject.tag == "Enemy" && gameObject.tag == "Deflect")
        {
            if (shooter == collision.gameObject.GetComponent<EnemyController>())
            {
                collision.gameObject.GetComponent<EnemyHealth>().TakeDamage(1);
                Debug.Log("hit enemy");
                Destroy(gameObject);

            }
        }
    }

    void DestroyArrowWhenFarAway()
    {

        if(Vector3.Distance(gameObject.transform.position, playerTransform.position) > 50)
        {
            Destroy(gameObject);
        }
    }

    public void Deflect()
    {
        int quickFix = 0;
        if (playerTransform.position.x > shooter.transform.position.x)
        {
            quickFix = 20;
        }
        else
        {
            quickFix = 0;
        }
        Vector3 offset = new Vector3(0,1,0);
        Vector3 dirToShooter = (shooter.transform.position + offset - transform.position).normalized;

        rb.linearVelocity = dirToShooter * rb.linearVelocity.magnitude * 1.5f;
        rb.rotation = Quaternion.Euler(0, 0, rb.rotation.eulerAngles.z + 170 + quickFix);
        gameObject.tag = "Deflect";
    }
}
