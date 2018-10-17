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

    public void GoToLevel1() {
        SceneManager.LoadScene("Level1");
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
        GameManager.instance.gameOver = false;
    }
}
