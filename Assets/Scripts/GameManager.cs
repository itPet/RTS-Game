using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class GameManager : MonoBehaviour {

    public static GameManager instance = null;

    [SerializeField] GameObject line;
    [SerializeField] GameObject towerPicture;
    [SerializeField] GameObject gameOverCanvas;
    [SerializeField] GameObject victoryCanvas;
    [SerializeField] GameObject concedeCanvas;
    [Range(1, 12)] public int towerCost;
    public bool gameOver = false;

    GameObject newLine;
    GameObject newTowerPicture;
    AI aiBrain;

    Transform potentialMove;
    List<Transform> allPlayerSites = new List<Transform>();
    BuildSiteController pendingTowerOnSite;


    // ################ STARTER METHODS ################
    void Awake() {
        if (instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);

        Assert.IsNotNull(line);
        Assert.IsNotNull(towerPicture);
        Assert.IsNotNull(gameOverCanvas);
    }

    void Start() {

    }

    private void OnLevelWasLoaded(int level) {
        if (level != 0) { //If not menu
            aiBrain = GameObject.FindWithTag("AIBrain").GetComponent<AI>();
            StartCoroutine(BackgroundMusic());
            GetComponents<AudioSource>()[4].Stop();
            GetComponents<AudioSource>()[5].Play();
        } else { //If menu
            GetComponents<AudioSource>()[4].Play();
        }
    }


    // ################ UPDATE METHODS ################
    void Update() {
        if (!gameOver) {
            //---- PENDING AND MOUSE DOWN ----
            if ((potentialMove || pendingTowerOnSite) && Input.GetMouseButtonDown(0)) {
                Transform clickedObject = MouseHit().transform;
                if (potentialMove) {
                    if (MouseDownOn("BuildSite")) {
                        // Get controller
                        BuildSiteController clickedSite = MouseHit().transform
                                                  .GetComponent<BuildSiteController>();
                        if (clickedSite.transform != potentialMove)
                            MoveSuccess(clickedSite.transform);
                        else
                            MoveFailed();
                    }
                }

                if (pendingTowerOnSite) {
                    if (MouseDownOn("PlayerTower") && ParentSiteCtrl(clickedObject) == pendingTowerOnSite)
                        PendingTowerSuccess();
                    else
                        PendingTowerFail();
                }
            }

            //---- PENDING AND MOUSE UP ----
            else if ((potentialMove || pendingTowerOnSite) && Input.GetMouseButtonUp(0)) {
                Transform upedObject = MouseHit().transform;
                if (potentialMove) {
                    if (MouseUpOn("BuildSite") && upedObject != potentialMove) //If upedObject != potentialMove
                        MoveSuccess(MouseHit().transform);
                    else if (MouseUpOnNothing() || upedObject != potentialMove)
                        MoveFailed();
                }

                if (pendingTowerOnSite) {
                    if (MouseUpOn("PlayerTower") && ParentSiteCtrl(upedObject) == pendingTowerOnSite)
                        PendingTowerSuccess();
                    else if (MouseUpOnNothing() || ParentSiteCtrl(upedObject) != pendingTowerOnSite)
                        PendingTowerFail();
                }
            }

            //---- BUILD SITE CLICKED ---- 
            else if (MouseDownOn("BuildSite")) {
                BuildSiteController clickedSite = MouseHit().transform.GetComponent<BuildSiteController>();
                if (clickedSite.GetOwner() == BuildSiteController.Owner.Player) { //If friendly troops on site
                    PotentialMoveFrom(clickedSite.transform);
                    GetComponent<AudioSource>().Play();
                }

                if (clickedSite.CanBuild()) { //If site empty
                    if (clickedSite.GetTroops().Count > (towerCost - 1) && clickedSite.GetOwner() == BuildSiteController.Owner.Player) { //If site has at least 6 friendly troops
                        PendingTowerAt(clickedSite);
                        GetComponent<AudioSource>().Play();
                    }
                }
            }

            //---- MOVE ALL CLICKED ----
            else if (MouseDownOn("MoveAll") && aiBrain.GetPlayerSites().Count > 0) {
                PotentialMoveFrom(MouseHit().transform);
                GetComponent<AudioSource>().Play();
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
        GetComponents<AudioSource>()[1].Stop(); //Level music
        GetComponents<AudioSource>()[3].Play(); //GameOver sound
        gameOver = true;
        Instantiate(gameOverCanvas);
        Time.timeScale = 0;
    }

    public void Victory() {
        GetComponents<AudioSource>()[1].Stop(); //Level music
        GetComponents<AudioSource>()[6].Play(); //Winning sound
        gameOver = true;
        Instantiate(victoryCanvas);
        Time.timeScale = 0;
    }

    public void SetTimeScale(int i) {
        Time.timeScale = i;
    }


    //################ HELPER METHODS 1 ################
    IEnumerator BackgroundMusic() {
        GetComponents<AudioSource>()[2].Play();
        yield return new WaitForSeconds(2.5f);
        GetComponents<AudioSource>()[1].Play();
        Instantiate(concedeCanvas);
    }

    void PendingTowerAt(BuildSiteController site) {
        newTowerPicture = Instantiate(towerPicture); //Create tower
        newTowerPicture.transform.SetParent(site.transform); //Set parent
        newTowerPicture.transform.position = site.transform.position + new Vector3(1.5f, 0, 0);
        site.GetComponent<SpriteRenderer>().enabled = true; //Highlight site
        pendingTowerOnSite = site;
    }

    void PotentialMoveFrom(Transform clickedObject) {
        if (clickedObject.tag == "MoveAll") {
            foreach (BuildSiteController site in aiBrain.GetPlayerSites()) {
                site.transform.GetComponent<SpriteRenderer>().enabled = true;
                allPlayerSites.Add(site.transform);
            }
            clickedObject.GetComponent<SpriteRenderer>().color = Color.green;
        } else {
            clickedObject.GetComponent<SpriteRenderer>().enabled = true;
        }

        potentialMove = clickedObject;
        if (newLine == null)
            CreateLine();
    }

    BuildSiteController ParentSiteCtrl(Transform child) {
        return child.GetComponentInParent<BuildSiteController>();
    }

    //---- SUCCESS/FAIL METHODS ----
    void PendingTowerSuccess() {
        List<Transform> paymentTroops = pendingTowerOnSite.GetTroops();
        for (int i = 0; i < towerCost; i++) {
            Destroy(paymentTroops[i].gameObject);
        }
        pendingTowerOnSite.CreateTower();
        Destroy(newTowerPicture);
        pendingTowerOnSite.GetComponent<SpriteRenderer>().enabled = false; //Un hightlight
        pendingTowerOnSite = null;
        potentialMove = null;
    }

    void PendingTowerFail() {
        pendingTowerOnSite.GetComponent<SpriteRenderer>().enabled = false; //Un hightlight
        pendingTowerOnSite = null;
        Destroy(newTowerPicture);
    }

    void MoveSuccess(Transform destination) {
        if (potentialMove.tag == "MoveAll") {
            foreach (Transform site in allPlayerSites) {
                site.GetComponent<SpriteRenderer>().enabled = false;
            }
            foreach (BuildSiteController site in aiBrain.GetPlayerSites()) {
                site.MoveTroops(destination);
            }
            allPlayerSites.Clear();
            potentialMove.GetComponent<SpriteRenderer>().color = Color.white;
        } else {
            potentialMove.GetComponent<BuildSiteController>().MoveTroops(destination);
            potentialMove.GetComponent<SpriteRenderer>().enabled = false;
        }

        potentialMove = null;
    }

    void MoveFailed() {
        foreach (Transform site in allPlayerSites) {
            site.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (potentialMove.tag == "MoveAll") {
            potentialMove.GetComponent<SpriteRenderer>().color = Color.white;
        } else {
            potentialMove.GetComponent<SpriteRenderer>().enabled = false;
        }

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
