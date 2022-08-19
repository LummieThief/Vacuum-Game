using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Persistent : MonoBehaviour
{
	private void Awake()
	{
		foreach (Persistent p in FindObjectsOfType<Persistent>())
		{
			if (p != this && p.name == this.name)
			{
				Debug.Log("Persistent protected!");
				Destroy(this.gameObject);
			}
		}
		DontDestroyOnLoad(this);
	}
}
