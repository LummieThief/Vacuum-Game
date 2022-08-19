using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI, creditsMenuUI, mainMenuUI, optionsMenuUI;
	private void Awake()
	{
		if (MainMenu.UI != null)
		{
			Destroy(this);
			return;
		}
		PauseMenu.UI = pauseMenuUI;
		CreditsMenu.UI = creditsMenuUI;
		MainMenu.UI = mainMenuUI;
		OptionsMenu.UI = optionsMenuUI;
	}
}
