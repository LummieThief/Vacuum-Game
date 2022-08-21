using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	public static GameObject UI;
	public void Play()
	{
		MainMenu.UI.SetActive(false);
		SceneManager.LoadScene("Main Hall");
		MusicController.instance.ClosedMenu();
	}

	public void Credits()
	{
		CreditsMenu.UI.SetActive(true);
	}

	public void Options()
	{
		OptionsMenu.UI.SetActive(true);
	}

	public void Quit()
	{
		Application.Quit();
	}
}
