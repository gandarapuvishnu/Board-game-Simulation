using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class exit : MonoBehaviour
{
	public Button yourButton;

	void Start()
	{
		Button btn = yourButton.GetComponent<Button>();
		btn.onClick.AddListener(TaskOnClick);
	}

	void TaskOnClick()
	{
		Debug.Log("You have clicked the quit button!");
		Application.Quit();
	}
}
