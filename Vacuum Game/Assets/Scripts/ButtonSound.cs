using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonSound : MonoBehaviour
{
    public void PlayEnterSound()
	{
		//no sound effect at the moment
	}

	public void PlayPressSound()
	{
		AudioManager.instance.Play("buttonSelect");
	}
}
