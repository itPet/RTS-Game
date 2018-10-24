using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TowerController : MonoBehaviour {

    [SerializeField] GameObject troop;
    [SerializeField] [Range(0, 5)] int spawnDelay;
    [SerializeField] UnityEngine.UI.Slider healthBar;


    GameObject newTroop;
    bool checkHealth = true;

    [Range(10, 50)] public int health;

    // ################ STARTER METHODS ################
    void Awake() {
        Assert.IsNotNull(troop);
    }

    private void Start() {
        healthBar.minValue = 0;
        healthBar.maxValue = health;
        StartCoroutine(SpawnTroops(transform.parent.parent.GetComponent<BuildSiteController>().GetSpawnPositions()));
    }


    // ################ UDATE METHODS ################
    private void Update() {
        healthBar.value = health;
        if (health < 1 && checkHealth) {
            checkHealth = false;
            GetComponent<AudioSource>().Play();
            if (transform.name == "PlayerCastle") {
                GameManager.instance.GameOver();
            } else if (transform.name == "AICastle") {
                GameManager.instance.Victory();
            }
            Destroy(gameObject);
        }
    }

    IEnumerator SpawnTroops(List<Transform> positions) {
        while (true) {
            yield return new WaitForSeconds(spawnDelay);
            CreateOneTroop(positions);
        }
    }


    // ################ HELPER METHODS 1 ################
    void CreateOneTroop(List<Transform> positions) {
        for (int i = 0; i < positions.Count; i++) {
            if (positions[i].childCount == 0) {
                newTroop = Instantiate(troop); //Create new troop
                newTroop.transform.position = transform.position; //Set position
                newTroop.transform.localScale = new Vector2(0.7f, 0.7f); //Set scale
                newTroop.transform.SetParent(positions[i]); //Set parent
                newTroop.GetComponent<SpriteRenderer>().sortingOrder = i; //Set order in layer
                break;
            }
        }
    }


    // ################ PUBLIC METHODS ################
    //public void Activate(List<Transform> positions) {
    //    StartCoroutine(SpawnTroops(positions));
    //}
}
