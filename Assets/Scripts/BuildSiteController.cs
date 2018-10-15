using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildSiteController : MonoBehaviour {

    public TowerStatus status;
    public enum TowerStatus {
        Empty,
        Pending,
        Full
    }


    //################ STARTER METHODS ################
    private void Start() {
        status = TowerStatus.Empty;
    }

    //################ UPDATE METHODS ################

    //################ HELPER METHODS 1 ################

    //################ PUBLIC METHODS ################
    public bool TroopsOnSite() {
        foreach (Transform pos in GetSpawnPositions()) {
            if (pos.childCount == 1) {
                return true;
            }
        }
        return false;
    }

    public void ActivateTower(GameObject tower) {
        tower.GetComponent<TowerController>().Activate(GetSpawnPositions()); //Activate tower
        tower.GetComponent<SpriteRenderer>().color = Color.white; //Set tower opaciy to 100%
        tower.layer = 2; //Set layer to Ignore Raycast
    }

    public void MoveTroops(Transform destination) {
        foreach (Transform pos in GetSpawnPositions()) {
            if (pos.childCount > 0) { //If pos has troop
                for (int i = 0; i < pos.childCount; i++) {
                    print(i);
                    pos.GetChild(i).GetComponent<TroopController>().Target = destination.position; //Set new destination for troop
                    pos.GetChild(i).GetComponent<TroopController>().HomeBuildSite = destination; //Set homeBuildSite
                    pos.GetChild(i).transform.localScale = Vector2.one; //Make big
                }
                pos.DetachChildren();
            }
        }
    }

    public List<Transform> GetSpawnPositions() {
        List<Transform> positions = new List<Transform>();
        Transform[] children = transform.Find("SpawnPositions").GetComponentsInChildren<Transform>();
        foreach (Transform child in children) {
            if (child.tag == "SpawnPosition")
                positions.Add(child);
        }
        return positions;
    }





    //---- INSIDE BLOCK METHODS ----












    //[SerializeField] private GameObject tower;
    //[SerializeField] private GameObject line;

    //private GameObject newLine;
    //private GameObject newTower;

    // ######## STARTER METHODS ########


    // ######## UPDATE METHODS ########

    //private void OnTriggerEnter2D(Collider2D collision) {
    //    print("Enter");
    //    collision.transform.localScale = new Vector2(0.65f, 0.65f);
    //}

    //private void OnTriggerExit2D(Collider2D collision) {
    //    collision.transform.localScale = Vector2.one;
    //}



    //void Update() {
    // If pending: create or destroy tower
    //if (Input.GetMouseButtonDown(0)) {
    //if (status == Status.Pending) {
    //if (HitTower()) {
    //    ActivateTower();
    //    status = Status.Full;
    //} else {
    //    Destroy(newTower);
    //    status = Status.Empty;
    //}
    //} else if (status == Status.Move) {
    //if (HitBuildSite()) {
    //    MoveTroops(MouseHit().transform.position);
    //    status = Status.Full;
    //}
    //        }
    //    }
    //}

    //private void OnMouseDown() {
    // If buildSite is empty, create tower
    //if (status == Status.Empty) {
    //    CreateTower();
    //} else if (status == Status.Full && !GameManager.instance.GlobalPending) {
    //    // Highlight
    //    GameManager.instance.GlobalPending = true;
    //}
    //}

    //private void OnMouseDrag() {
    //    // If full, create line
    //    if (status == Status.Full && newLine == null) {
    //        CreateLine();
    //    } else if (newLine != null) {
    //        //Follow mouse
    //        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //        newLine.GetComponent<LineRenderer>().SetPosition(1, pos);
    //    }
    //}

    //private void OnMouseUp() {
    //    // if line, destroy
    //    if (newLine != null)
    //        Destroy(newLine);

    //    if (status == Status.Empty) {
    //        if (HitTower()) {
    //            ActivateTower();
    //            status = Status.Full;
    //        } else {
    //            status = Status.Pending;
    //        }
    //    } else if (status == Status.Full) {
    //        if (HitBuildSite() && MouseHit().transform != transform) {
    //            MoveTroops(MouseHit().transform.position);
    //        }
    //        GameManager.instance.GlobalPending = false;
    //    }
    //}


    // ######## HELPER METHODS 1 ########
    //private void MoveTroops(Vector2 destination) {
    //    foreach (Transform pos in GetSpawnPositions()) {
    //        if (pos.childCount == 1) {
    //            pos.GetChild(0).GetComponent<TroopController>().Move(destination);
    //            pos.DetachChildren();
    //        }
    //    }
    //}

    //private void CreateLine() {
    //    newLine = Instantiate(line);
    //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    newLine.GetComponent<LineRenderer>().SetPosition(0, pos);
    //    newLine.GetComponent<LineRenderer>().SetPosition(1, pos);
    //}

    //private void CreateTower() {
    //    newTower = Instantiate(tower);
    //    // Set tower parent
    //    newTower.transform.SetParent(transform);
    //    // Set tower position
    //    newTower.transform.position = transform.position + new Vector3(-0.5f, 0.8f, 0);
    //    // Set opacity to 50%
    //    newTower.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f);
    //}

    //private bool HitBuildSite() {
    //    return (MouseHit() != false && MouseHit().collider.tag == "BuildSite");
    //}

    //private bool HitTower() {
    //    // true if hit tower
    //    return (MouseHit() != false && MouseHit().collider.tag == "Tower");
    //}

    //private void ActivateTower() {
    //    // Activate tower
    //    newTower.GetComponent<TowerController>().Activate(GetSpawnPositions());
    //    // Set tower opaciy to 100%
    //    newTower.transform.GetComponent<SpriteRenderer>().color = Color.white;
    //    // Set layer to Ignore Raycast
    //    newTower.layer = 2;
    //}


    // ######## HELPER METHODS 2 ########
    //Set class variable objectHit to contain the raycast hit.
    //private RaycastHit2D MouseHit() {
    //    // Get mouse position in world
    //    Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    //    // Send ray from that position. 
    //    return Physics2D.Raycast(pos, Vector2.zero);
    //}

    //private Transform[] GetSpawnPositions() {
    //    return transform.Find("SpawnPositions").gameObject
    //        .transform.GetComponentsInChildren<Transform>();
    //}
}
