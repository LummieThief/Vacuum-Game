using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
	public static GameObject UI;

	public static bool paused;

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!paused)
			{
				Pause();
			}
			else
			{
				Resume();
			}
		}
	}

	public void Pause()
	{
		if (MainMenu.UI.activeSelf) return;
		paused = true;
		PauseMenu.UI.SetActive(true);
	}

	public void Resume()
	{
		if (MainMenu.UI.activeSelf) return;
		paused = false;
		PauseMenu.UI.SetActive(false);
	}

	public void Options()
	{
		OptionsMenu.UI.SetActive(true);
	}

	public void Menu()
	{
		Resume();
		MainMenu.UI.SetActive(true);
		MusicController.instance.OpenedMenu();
		SceneManager.LoadScene("Main Menu");
	}
}
