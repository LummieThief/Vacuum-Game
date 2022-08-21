using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] GameObject dustBunnyPrefab;
    [SerializeField] GameObject ratPrefab;
	[SerializeField] DirtController dirtController;
	[SerializeField] float spawnRate;
	[SerializeField] float ratSpawnPercentage;
	[SerializeField] int spawnCap;
	[SerializeField] float spawnCooldown;
	private float spawnCooldownTimer;
	[HideInInspector] public int numEnemies;

	public static EnemySpawner instance;
	private int numParticlesForMaximumSpawnRate;

	private void Awake()
	{
		if (instance != null) 
		{
			Destroy(this);
		}
		instance = this;
		numParticlesForMaximumSpawnRate = dirtController.maxParticles;
	}

	private void Update()
	{
		if (PauseMenu.paused) return;

		if (spawnCooldownTimer > 0)
		{
			spawnCooldownTimer -= Time.deltaTime;
		}
		else
		{
			AttemptSpawn();
		}
	}

	private void AttemptSpawn()
	{
		if (numEnemies >= spawnCap)
		{
			return;
		}
		float r = Random.Range(0f, 1f);
		//throttle spawning attempts as number of particles goes down
		if (r >= ((float)Mathf.Min(dirtController.particleCount, numParticlesForMaximumSpawnRate) / numParticlesForMaximumSpawnRate) * spawnRate * Time.deltaTime)
		{
			return;
		}

		Vector2 point = dirtController.PollRandomSpawnLocation();
		if (point != Vector2.zero)
		{
			GameObject g;
			if (Random.Range(0f, 1f) < ratSpawnPercentage)
			{
				g = Instantiate(ratPrefab);
			}
			else
			{
				g = Instantiate(dustBunnyPrefab);
			}

			numEnemies++;
			g.transform.position = point;
			spawnCooldownTimer = spawnCooldown;
		}

	}
}
