using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Popup : MonoBehaviour
{
    public static Popup instance;
    [SerializeField] Text text;
    // Start is called before the first frame update
    void Awake()
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
    }

    public void SetText(string s)
	{
        text.text = s;
	}
}
