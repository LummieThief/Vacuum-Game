using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;


public class OptionsMenu : MonoBehaviour
{
    public static GameObject UI;

    [SerializeField] AudioMixer musicMixer;
    [SerializeField] AudioMixer effectsMixer;

	private void Awake()
	{
        ChangeMusicVolume(0.5f);
        ChangeEffectsVolume(0.5f);
    }

	public void ChangeMusicVolume(float newVol)
    {
        if (newVol == 0)
        {
            newVol = 0.0001f;
        }
        musicMixer.SetFloat("Volume", Mathf.Log10(newVol) * 20);
    }

    public void ChangeEffectsVolume(float newVol)
    {
       
        if (newVol == 0)
		{
            newVol = 0.0001f;
		}
        effectsMixer.SetFloat("Volume", Mathf.Log10(newVol) * 20);
    }

    public void Back()
	{
        OptionsMenu.UI.SetActive(false);
	}
}
