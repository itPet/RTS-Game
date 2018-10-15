using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class TowerController : MonoBehaviour {

    [SerializeField] GameObject troop;
    [SerializeField] float spawnDelay;

    GameObject newTroop;

    public int health = 10;

    // ################ STARTER METHODS ################
    void Awake() {
        Assert.IsNotNull(troop);
    }


    // ################ UDATE METHODS ################
    private void Update() {
        if (health < 1) {
            transform.parent.GetComponent<BuildSiteController>().status = BuildSiteController.TowerStatus.Empty;
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
    public void Activate(List<Transform> positions) {
        StartCoroutine(SpawnTroops(positions));
    }
}
