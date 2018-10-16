using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void MyButtons() {
        SceneManager.LoadScene("MainScene");
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("GameMenu");
    }
}
