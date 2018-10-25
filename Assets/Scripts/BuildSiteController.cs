using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class BuildSiteController : MonoBehaviour {

    [SerializeField] GameObject playerTower;
    [SerializeField] GameObject AITower;

    GameObject newTower;

    public enum Owner {
        Neutral,
        AI,
        Player
    }


    //################ STARTER METHODS ################
    private void Start() {

    }

    //################ UPDATE METHODS ################

    private void Update() {
        if (DifferentTroopsOnSite()) { //If enemies on site, move to center and back again
            MoveTroops(transform);
        } 
    }

    //################ HELPER METHODS 1 ################
    bool DifferentTroopsOnSite() {
        string troopTag = "Empty";
        foreach (Transform pos in GetSpawnPositions()) { //Loop through all spawn positions
            if (pos.childCount == 2) { //If 2 troops at pos, they are different
                return true;
            } else if (pos.childCount == 1) {
                if (troopTag != "Empty" && troopTag != pos.GetChild(0).tag) {
                    return true;
                } else {
                    troopTag = pos.GetChild(0).tag;
                }
            }
        }
        return false;
    }


    //################ PUBLIC METHODS ################
    public void CreateTower() {
        GetComponent<AudioSource>().Play();
        if (GetOwner() == Owner.AI) {
            newTower = Instantiate(AITower);
        } else if (GetOwner() == Owner.Player) {
            newTower = Instantiate(playerTower);
        }
        newTower.transform.SetParent(transform.Find("Building")); //Set parent
        newTower.transform.position = transform.position; //Set position
    }

    public bool CanBuild() {
        return transform.Find("Building").childCount == 0;
    }

    public Owner GetOwner() {
        Owner owner = Owner.Neutral;
        if (transform.Find("Building").transform.childCount > 0) {
            owner = transform.Find("Building").GetChild(0).tag == "AITower" ? Owner.AI : Owner.Player;
        } else if (!DifferentTroopsOnSite()) {
            foreach (Transform pos in GetSpawnPositions()) {
                if (pos.childCount > 0) {
                    owner = pos.GetChild(0).tag == "AITroop" ? Owner.AI : Owner.Player;
                    break;
                }
            }

        }
        return owner;
    }

    public List<Transform> GetTroops() {
        List<Transform> troops = new List<Transform>();
        foreach (Transform pos in GetSpawnPositions()) {
            if (pos.childCount > 0) {
                for (int i = 0; i < pos.childCount; i++) {
                    troops.Add(pos.GetChild(i));
                }
            }
        }
        return troops;
    }

    public void MoveTroops(Transform destination) {
        foreach (Transform pos in GetSpawnPositions()) {
            if (pos.childCount > 0) { //If pos has troop
                for (int i = 0; i < pos.childCount; i++) {
                    TroopController troop = pos.GetChild(i).GetComponent<TroopController>();
                    troop.Target = destination.position; //Set new destination for troop
                    troop.HomeBuildSite = destination; //Set homeBuildSite
                    troop.ResetCanHitObstacle();
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
