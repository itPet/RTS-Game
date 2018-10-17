using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaymentScript : MonoBehaviour {

	void Start () {
        GetComponent<TextMesh>().text = "-" + GameManager.instance.towerCost.ToString();
	}
}
