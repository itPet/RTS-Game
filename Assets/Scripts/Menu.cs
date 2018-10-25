using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    [SerializeField] GameObject levelWindow;
    [SerializeField] GameObject kingdomRush;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoToLevelWindow() {
        GetComponent<AudioSource>().Play();
        kingdomRush.SetActive(false);
        levelWindow.SetActive(true);
    }

    public void GoToLevel1() {
        SceneManager.LoadScene("Level1");
    }

    public void GoToLevel2() {
        SceneManager.LoadScene("Level2");
    }

    public void GoToLevel3() {
        SceneManager.LoadScene("Level3");
    }

    public void GoToLevel4() {
        SceneManager.LoadScene("Level4");
    }

    public void GoToLevel5() {
        SceneManager.LoadScene("Level5");
    }

    public void GoToLevel6() {
        SceneManager.LoadScene("Level6");
    }

    public void GoToMainMenu() {
        SceneManager.LoadScene("MainMenu");
        GameManager.instance.gameOver = false;
        GameManager.instance.SetTimeScale(1);
        GameManager.instance.GetComponents<AudioSource>()[3].Stop();
    }

    public void Concede() {
        print("Concede button pressed");
        GameManager.instance.GameOver();
        gameObject.SetActive(false);
    }
}
