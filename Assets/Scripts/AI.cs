using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    [SerializeField] [Range(1, 12)] int aiCost;
    [SerializeField] [Range(1, 12)] int aiMoveCost;
    [SerializeField] List<BuildSiteController> allSites;
    [SerializeField] List<BuildSiteController> sitesCloseToAI;
    [SerializeField] BuildSiteController aiStartSite;

    TowerController aiCastle;
    int startHealth;
    bool moveMinusOne = true;
    bool moveSeventyFive = true;
    bool moveFifty = true;
    bool moveTwentyFive = true;

    //################ STARTER METHODS ################
    private void Start() {
        aiCastle = aiStartSite.GetComponentInChildren<TowerController>();
        startHealth = aiCastle.health;

    }


    //################ UPDATE METHODS ################
    void Update() {
        //If castle damage, move all troops there
        if (moveMinusOne && aiCastle.health == startHealth-1) {
            MoveAllAIHome();
            moveMinusOne = false;
        } else if (moveSeventyFive && aiCastle.health == startHealth * 0.75f) {
            MoveAllAIHome();
            moveSeventyFive = false;
        } else if (moveFifty && aiCastle.health == startHealth * 0.5f) {
            MoveAllAIHome();
            moveFifty = false;
        } else if (moveTwentyFive && aiCastle.health == startHealth * 0.25f) {
            MoveAllAIHome();
            moveTwentyFive = false;
        }

        //At start site, move at aiCost to close to AI
        if (aiStartSite.GetTroops().Count >= aiCost) {
            int random = Random.Range(0, sitesCloseToAI.Count);
            aiStartSite.MoveTroops(sitesCloseToAI[random].transform);
        }

        //For all sites, build at buildCost, move at moveCost
        foreach (BuildSiteController aiSite in GetAISites()) {
            if (aiSite.GetTroops().Count >= aiCost) { //If count = aiCost, build
                if (aiSite.CanBuild()) {
                    aiSite.CreateTower();
                    for (int i = 0; i < aiCost; i++) {
                        Destroy(aiSite.GetTroops()[i].gameObject);
                    }
                } else if (aiSite.GetTroops().Count >= aiMoveCost){ //If moveCost, move to random
                    List<BuildSiteController> noneAISites = GetNoneAISites();
                    if (noneAISites.Count > 0) {
                        int random = Random.Range(0, noneAISites.Count);
                        aiSite.MoveTroops(noneAISites[random].transform);
                    }
                }
            }
        }
    }

    //################ PUBLIC METHODS ################
    //void MoveAllPlayerTroops(Transform target) {
    //    foreach (BuildSiteController site in allSites) {
    //        if (site.GetOwner() == BuildSiteController.Owner.Player) {
    //            site.MoveTroops(target);
    //        }
    //    }
    //}


    //################ HELPER METHODS 1 ################
    List<BuildSiteController> GetAISites() {
        List<BuildSiteController> aiSites = new List<BuildSiteController>();
        foreach (BuildSiteController site in allSites) {
            if (site.GetOwner() == BuildSiteController.Owner.AI) {
                aiSites.Add(site);
            }
        }
        return aiSites;
    }

    public List<BuildSiteController> GetPlayerSites() {
        List<BuildSiteController> playerSites = new List<BuildSiteController>();
        foreach (BuildSiteController site in allSites) {
            if (site.GetOwner() == BuildSiteController.Owner.Player) {
                playerSites.Add(site);
            }
        }
        return playerSites;
    }

    List<BuildSiteController> GetNoneAISites() {
        List<BuildSiteController> noneAISites = new List<BuildSiteController>();
        foreach (BuildSiteController site in allSites) {
            if (site.GetOwner() != BuildSiteController.Owner.AI) {
                noneAISites.Add(site);
            }
        }
        return noneAISites;
    }

    void MoveAllAIHome() {
        print(aiCastle.health);
        foreach (BuildSiteController site in GetAISites()) {
            site.MoveTroops(aiStartSite.transform);
        }
    }

}
