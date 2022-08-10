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
	private int numParticlesForMaximumSpawnRate;

	private void Awake()
	{
		numParticlesForMaximumSpawnRate = dirtController.maxParticles;
	}

	private void Update()
	{
		AttemptSpawn();
	}

	private void AttemptSpawn()
	{
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

			g.transform.position = point;
		}

	}
}
