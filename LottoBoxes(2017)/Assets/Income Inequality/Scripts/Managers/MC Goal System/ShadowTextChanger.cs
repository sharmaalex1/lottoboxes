using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ShadowTextChanger : MonoBehaviour {

	public Text masterText;
	
	// Update is called once per frame
	void Update () {
		gameObject.GetComponent<Text>().text = masterText.text;
	}
}
