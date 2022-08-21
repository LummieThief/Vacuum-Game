using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatAI : LivingEntity
{
    [SerializeField] float scurryChance;
    [SerializeField] float scurryRange;
    private Vector3 scurryPoint;
    [SerializeField] float scurrySpeed;
    [SerializeField] float poopChance;

    private bool scurrying;
    private Vector2 direction;
    private float radius;

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
        if (PauseMenu.paused) return;
        if (scurrying)
        {
            float dist = scurrySpeed * Time.deltaTime;

            if (Vector2.Distance(transform.position, scurryPoint) < dist)
            {
                transform.position = scurryPoint;
                Stop();
            }
            else
            {
                transform.Translate(direction * dist);
            }
        }
        else
        {
            if (Random.Range(0f, 1f) < scurryChance * Time.deltaTime)
			{
                Scurry();
			}
        }

        if (Random.Range(0f, 1f) < poopChance * Time.deltaTime)
		{
            int c = DirtController.instance.materials.Length - 1;
            DirtController.instance.AddParticle(transform.position, c);
        }
    }

    private void Scurry()
    {
        RaycastHit2D hit = new RaycastHit2D();
        Vector2 angle = Vector2.zero;
        float range = 0;
        for (int i = 0; i < 30; i++)
		{
            angle = Random.insideUnitCircle.normalized;
            range = Random.Range(scurryRange / 4, scurryRange);
            hit = Physics2D.CircleCast(transform.position, radius, angle, range, LayerMask.GetMask("Walls"));
            if (hit.collider == null)
			{
                break;
			}
        }

        if (hit.collider != null)
        {
            return;
        }

        direction = angle;
        scurryPoint = transform.position + (Vector3)(direction * range);
        scurrying = true;

        Quaternion rot = Quaternion.LookRotation(Vector3.forward, -direction);
        animator.transform.rotation = rot;
        animator.SetBool("Running", true);
    }

    private void Stop()
    {
        scurrying = false;
        animator.SetBool("Running", false);
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
        if (!isDead)
		{
            AudioManager.instance.Play("ratHurt");
            animator.SetTrigger("Hurt");
        }
        else
		{
            g.transform.rotation = animator.transform.rotation;
            AudioManager.instance.Play("ratDie");
        }
        
    }

	private void OnDestroy()
	{
        EnemySpawner.instance.numEnemies--;
	}
}
