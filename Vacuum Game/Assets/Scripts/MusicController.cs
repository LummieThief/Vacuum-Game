using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicController : MonoBehaviour
{
	public static MusicController instance;
	private AudioSource dirtyMusic;
	private AudioSource cleanMusic;
	private float dirtyMusicVol, cleanMusicVol;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
			return;
		}

		dirtyMusic = AudioManager.instance.GetSource("dirtyRoom");
		cleanMusic = AudioManager.instance.GetSource("cleanRoom");
		dirtyMusicVol = dirtyMusic.volume;
		cleanMusicVol = cleanMusic.volume;
		dirtyMusic.volume = 0;
		cleanMusic.volume = 0;
		AudioManager.instance.Play("mainHall");
	}

	public void SwapTrack(bool clean)
	{
		if (clean)
		{
			cleanMusic.volume = cleanMusicVol;
			dirtyMusic.volume = 0;
		}
		else
		{
			cleanMusic.volume = 0;
			dirtyMusic.volume = dirtyMusicVol;
		}
	}

	public void OpenedMenu()
	{
		AudioManager.instance.Play("mainHall");
		dirtyMusic.Stop();
		cleanMusic.Stop();
	}

	public void ClosedMenu()
	{
		AudioManager.instance.GetSource("mainHall").Stop();
		dirtyMusic.Play();
		cleanMusic.Play();
		SwapTrack(true);
	}
}
