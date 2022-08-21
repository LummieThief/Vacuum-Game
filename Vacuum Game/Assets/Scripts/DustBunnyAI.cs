using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustBunnyAI : LivingEntity
{
    [SerializeField] float jumpCooldown;
    private float jumpCooldownTimer;
    [SerializeField] float jumpDistance;
    private float distanceJumped;
    [SerializeField] float jumpSquat;
    private float timeSquatted;
    [SerializeField] float jumpSpeed;
    [SerializeField] int numDustParticles;

    private bool jumping;
    private bool collided;
    private Vector2 direction;
    private Vector2 nextDirection;
    private float radius;
    private float speedMult = 1;
    private float lastSlowTime;
    bool isSlowed = false;
    [SerializeField] float slowTime = 1f;
    [SerializeField] float maxSlow = 0.5f;

    [SerializeField] Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        radius = transform.localScale.x / 2f;
        direction = Random.insideUnitCircle.normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isSlowed) speedMult = 1f;
        else speedMult = maxSlow;
        if (jumping)
		{
            if (timeSquatted < jumpSquat)
			{
                timeSquatted += Time.deltaTime;
			}
            else
			{
                animator.SetBool("Airborne", true);
                animator.ResetTrigger("Squat");
                float dist = jumpSpeed * Time.deltaTime * speedMult;

                if (dist + distanceJumped > jumpDistance)
                {
                    dist = jumpDistance - distanceJumped;
                    transform.Translate(direction * dist);
                    Land();
                }
                else
                {
                    distanceJumped += dist;
                    transform.Translate(direction * dist);
                }
            }
        }
        else
		{
            if (jumpCooldownTimer <= 0)
            {
                Jump();
            }
            else
            {
                jumpCooldownTimer -= Time.deltaTime;
            }
        }
    }

    private void Jump()
	{
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, radius, direction, jumpDistance, LayerMask.GetMask("Walls"));
        if (hit.collider != null)
		{
            //dust bunny will bump into a wall
            Vector2 point = hit.point + hit.normal * radius;
            Debug.DrawLine(hit.point, point, Color.blue, jumpCooldown);
            distanceJumped = jumpDistance - Vector2.Distance(transform.position, point);
            collided = true;
            nextDirection = RotateVector2(hit.normal, Random.Range(-45, 45));
		}
        else
		{
            distanceJumped = 0;
            collided = false;
		}
        jumpCooldownTimer = jumpCooldown;
        jumping = true;
        timeSquatted = 0;
        animator.SetTrigger("Squat");
        
        Quaternion rot = Quaternion.LookRotation(Vector3.forward, -direction);
        animator.transform.rotation = rot;
    }

    private void Land()
	{
        jumping = false;
        if (collided)
        {
            direction = nextDirection;
        }
        float r = radius - DirtController.pixelSize / 2;
        for (int i = 0; i < numDustParticles; i++)
		{
            int c = Random.Range(0, DirtController.instance.materials.Length - 1);
            DirtController.instance.AddParticle((Vector2)transform.position + Vector2.up * Random.Range(-r, r) + Vector2.right * Random.Range(-r, r), c);
		}
        animator.SetBool("Airborne", false);
        animator.ResetTrigger("Squat");

    }

    private Vector2 RotateVector2(Vector2 v, float delta)
    {
        delta *= Mathf.Deg2Rad;
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public override void TakeDamage(float damage)
    {
        base.TakeDamage(damage);
        if(!isDead){
            isSlowed = true;
            lastSlowTime = Time.time + slowTime;
            AudioManager.instance.Play("bunnyHurt");
            animator.SetTrigger("Hurt");
        }
        else
		{
            AudioManager.instance.Play("bunnyDie");
        }
    }

    private void OnDestroy()
    {
        EnemySpawner.instance.numEnemies--;
    }
}
