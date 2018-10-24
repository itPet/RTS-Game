using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

    [SerializeField] List<BuildSiteController> buildSites;

    [SerializeField] [Range(1,12)] int aiCost;
    bool gameOver = true;

	// Use this for initialization
	void Start () {
		
	}

    // Update is called once per frame
    void Update() {
        foreach (BuildSiteController aiSite in GetAISites()) {
            if (aiSite.GetTroops().Count >= aiCost) {
                if (aiSite.CanBuild()) {
                    aiSite.CreateTower();
                    for (int i = 0; i < aiCost; i++) {
                        Destroy(aiSite.GetTroops()[i].gameObject);
                    }
                } else {
                    List<BuildSiteController> noneAISites = GetNoneAISites();
                    if (noneAISites.Count > 0) {
                        aiSite.MoveTroops(noneAISites[Random.Range(0, noneAISites.Count - 1)].transform);
                    }
                }
            }
        }
    }

    List<BuildSiteController> GetAISites() {
        List<BuildSiteController> aiSites = new List<BuildSiteController>();
        foreach (BuildSiteController site in buildSites) {
            if (site.GetOwner() == BuildSiteController.Owner.AI) {
                aiSites.Add(site);
            }
        }
        return aiSites;
    }

    List<BuildSiteController> GetNoneAISites() {
        List<BuildSiteController> noneAISites = new List<BuildSiteController>();
        foreach (BuildSiteController site in buildSites) {
            if (site.GetOwner() != BuildSiteController.Owner.AI) {
                noneAISites.Add(site);
            }
        }
        return noneAISites;
    }

}
