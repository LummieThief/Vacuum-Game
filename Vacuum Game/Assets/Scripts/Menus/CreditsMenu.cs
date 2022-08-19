using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsMenu : MonoBehaviour
{
	public static GameObject UI;
	public void Back()
	{
		CreditsMenu.UI.SetActive(false);
	}
}
