using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    [SerializeField] GameObject tower;
    [SerializeField] GameObject line;
    [SerializeField] GameObject towerPicture;
    [SerializeField] GameObject menu;

    GameObject newLine;
    GameObject newTower;
    GameObject newTowerPicture;

    BuildSiteController potentialMove;
    BuildSiteController pendingTower;

    bool gameOver = false;

    // ################ STARTER METHODS ################
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        Assert.IsNotNull(tower);
        Assert.IsNotNull(line);
        Assert.IsNotNull(towerPicture);
        Assert.IsNotNull(menu);
    }

    void Start() {
        
    }

    // ################ UPDATE METHODS ################
    void Update() {
        if (!gameOver) {
            //---- PENDING AND MOUSE DOWN ----
            if ((potentialMove || pendingTower) && Input.GetMouseButtonDown(0)) {
                Transform clickedObject = MouseHit().transform;
                if (potentialMove) {
                    if (MouseDownOn("BuildSite")) {
                        // Get controller
                        BuildSiteController clickedSite = MouseHit().transform
                                                  .GetComponent<BuildSiteController>();
                        if (clickedSite != potentialMove)
                            MoveSuccess(clickedSite.transform);
                        else
                            MoveFailed();
                    }
                }

                if (pendingTower) {
                    if (MouseDownOn("Tower") && ParentSiteCtrl(clickedObject) == pendingTower)
                        PendingTowerSuccess();
                    else
                        PendingTowerFail();
                }
            }

            //---- PENDING AND MOUSE UP ----
            else if ((potentialMove || pendingTower) && Input.GetMouseButtonUp(0)) {
                Transform upedObject = MouseHit().transform;
                if (potentialMove) {
                    if (MouseUpOn("BuildSite") && upedObject != potentialMove.transform) //If upedObject != potentialMove
                        MoveSuccess(MouseHit().transform);
                    else if (MouseUpOnNothing() || upedObject != potentialMove.transform)
                        MoveFailed();
                }

                if (pendingTower) {
                    if (MouseUpOn("Tower") && ParentSiteCtrl(upedObject) == pendingTower)
                        PendingTowerSuccess();
                    else if (MouseUpOnNothing() || ParentSiteCtrl(upedObject) != pendingTower)
                        PendingTowerFail();
                }
            }

            //---- BUILD SITE CLICKED ---- 
            else if (MouseDownOn("BuildSite")) {
                BuildSiteController clickedSite = MouseHit().transform.GetComponent<BuildSiteController>();
                if (clickedSite.TroopsOnSite()) { //If friendly troops on site
                    PotentialMoveFrom(clickedSite);
                }

                if (clickedSite.status == BuildSiteController.TowerStatus.Empty) { //If site empty
                    if (clickedSite.PaymentTroops().Count > 5) { //If site has at least 6 friendly troops
                        PendingTowerAt(clickedSite);
                    }
                }
            }

            //---- LINE ----
            if (Input.GetMouseButtonUp(0)) { //if line, destroy
                if (newLine != null)
                    Destroy(newLine);
            }

            //---- DRAG ----
            if (Input.GetMouseButton(0) && newLine) { //line follow mouse
                Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                newLine.GetComponent<LineRenderer>().SetPosition(1, pos);
            }
        }
    }


    //################ PUBLIC METHODS ################
    public void GameOver() {
        gameOver = true;
        menu.SetActive(true);
    }


    //################ HELPER METHODS 1 ################
    void PendingTowerAt(BuildSiteController site) {
        newTowerPicture = Instantiate(towerPicture); //Create tower
        newTowerPicture.transform.SetParent(site.transform); //Set parent
        newTowerPicture.transform.position = site.transform.position + new Vector3(1.5f, 0, 0);
        site.status = BuildSiteController.TowerStatus.Pending; //site to pending
        site.GetComponent<SpriteRenderer>().enabled = true; //Highlight site
        pendingTower = site;
    }

    void CreateTower(Transform onSite) {
        newTower = Instantiate(tower); 
        newTower.transform.SetParent(onSite); //Set tower parent
        newTower.transform.position = onSite.position; //Set tower position
        //newTower.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.5f); //Set opacity to 50%
    }

    void PotentialMoveFrom(BuildSiteController buildSite) {
        buildSite.GetComponent<SpriteRenderer>().enabled = true;
        potentialMove = buildSite;
        if (newLine == null)
            CreateLine();
    }

    BuildSiteController ParentSiteCtrl(Transform child) {
        return child.GetComponentInParent<BuildSiteController>();
    }

    //---- SUCCESS/FAIL METHODS ----
    void PendingTowerSuccess() {
        List<Transform> paymentTroops = pendingTower.PaymentTroops();
        foreach (Transform troop in paymentTroops) {
            Destroy(troop.gameObject);
        }
        //pendingTower.ActivateTower(newTower);
        CreateTower(pendingTower.transform);
        Destroy(newTowerPicture);
        pendingTower.status = BuildSiteController.TowerStatus.Full; //Status full
        pendingTower.GetComponent<SpriteRenderer>().enabled = false; //Un hightlight
        pendingTower = null;
        potentialMove = null;
    }

    void PendingTowerFail() {
        pendingTower.status = BuildSiteController.TowerStatus.Empty; //Status empty
        pendingTower.GetComponent<SpriteRenderer>().enabled = false; //Un hightlight
        pendingTower = null;
        Destroy(newTowerPicture);
    }

    void MoveSuccess(Transform destination) {
        potentialMove.MoveTroops(destination);
        potentialMove.GetComponent<SpriteRenderer>().enabled = false;
        potentialMove = null;
    }

    void MoveFailed() {
        potentialMove.GetComponent<SpriteRenderer>().enabled = false;
        potentialMove = null;
    }


    //################ HELPER METHODS 2 ################
    private void CreateLine() {
        newLine = Instantiate(line);
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        newLine.GetComponent<LineRenderer>().SetPosition(0, pos);
        newLine.GetComponent<LineRenderer>().SetPosition(1, pos);
    }

   
    //################ INPUT METHODS ################
    bool MouseDownOn(string nameTag) {
        return (Input.GetMouseButtonDown(0) && MouseHit() != false && MouseHit()
                .transform.tag == nameTag);
    }

    bool MouseUpOn(string nameTag) {
        return (Input.GetMouseButtonUp(0) && MouseHit() != false && MouseHit()
                .transform.tag == nameTag);
    }

    bool MouseUpOnNothing() {
        return (Input.GetMouseButtonUp(0) && MouseHit() == false);
    }

    RaycastHit2D MouseHit() {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition); //Get mouse position in world 
        return Physics2D.Raycast(pos, Vector2.zero); //Send ray from that position.
    }







    //################ STARTER METHODS ################

    //################ UPDATE METHODS ################

    //################ PUBLIC METHODS ################

    //################ HELPER METHODS 1 ################

    //---- INSIDE BLOCK METHODS ----
}
