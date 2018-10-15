using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerPicture : MonoBehaviour {

	// Use this for initialization
	void Start () {
        GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); //Set opacity to 50%
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
