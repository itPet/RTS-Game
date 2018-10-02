using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopController : MonoBehaviour {

    [SerializeField] float speed;
    [SerializeField] float ghostDuration;
    [SerializeField] float durationToGhost;
    [SerializeField] float portionOfSpeedToGhost;
    [SerializeField] float SiteEnterExitWaitTime;

    float distanceToTarget = 0.1f;
    bool canEnterAndExit = true;
    bool canStartStuckCheck = true;
    bool move = true;
    int walkHash = Animator.StringToHash("Walk");
    int attackHash = Animator.StringToHash("Attack");
    int idleHash = Animator.StringToHash("Idle");
    int dieHash = Animator.StringToHash("Die");

    TroopController enemy;
    string enemyTag;
    Vector2 target;
    Vector2 oldTarget;
    Animator anim;

    public int health = 3;
    public TroopStatus status = TroopStatus.Nothing;
    public enum TroopStatus {
        Nothing,
        Walk,
        Arrived
    }

    //################ STARTER METHODS ################
    void Start() { //Set variables
        target = transform.parent != null ? (Vector2)transform.parent.position : (Vector2)transform.position;
        enemyTag = tag == "Enemy" ? "Troop" : "Enemy"; //If you are an enemy, your enemy is Troop.
        anim = GetComponent<Animator>();
    }


    //################ UPDATE METHODS ################
    private void Update() {
        transform.rotation = Quaternion.Euler(Vector3.zero); //Always don't rotate
        if (health < 1)
            Die();
    }

    private void FixedUpdate() {
        MoveToTarget();

        if (Vector2.Distance(transform.position, target) < distanceToTarget) { //If close to target
            if (enemy && status != TroopStatus.Arrived) {
                print(enemy.tag);
                status = TroopStatus.Arrived;
                StartCoroutine(Attack());
            } else if (status != TroopStatus.Arrived) {
                status = TroopStatus.Arrived;
                BeginIdle();
            }
        } else { //If far from target
            if (status != TroopStatus.Walk)
                BeginWalk();
        }
    }

    //---- TRIGGER METHODS ----
    private void OnCollisionEnter2D(Collision2D collision) {
        if (enemy == null && collision.transform.tag == enemyTag) {
            print("collided with enemy");
            SetEnemyTarget(collision);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.transform.tag == "BuildSite" && canEnterAndExit) {
            StartCoroutine(EnteredBuildSite(collision.GetComponent<BuildSiteController>()));
        } 
    }

    private void OnTriggerExit2D(Collider2D collision) {
        if (collision.transform.tag == "BuildSite" && canEnterAndExit) {
            StartCoroutine(ExitBuildSite());
        }
    }


    //################ HELPER METHODS 1 ################
    void MoveToTarget() {
        if (move) {
            transform.position = Vector3.MoveTowards(transform.position, target, speed); //Every time, move "speed units" closer to target
            GetComponent<SpriteRenderer>().flipX = target.x < transform.position.x ? true : false; //Sprite looks left or right
        }
    }

    void Die() {
        anim.Play(dieHash);
        GetComponent<BoxCollider2D>().enabled = false;
        move = false;
        Destroy(gameObject, 2);
    }

    void BeginWalk() {
        status = TroopStatus.Walk;
        anim.Play(walkHash);
        if (canStartStuckCheck)
            StartCoroutine(StuckCheck()); //If stuck become ghost temporary
    }

    void BeginIdle() {
        anim.Play(idleHash);
    }

    IEnumerator Attack() {
        print("Attack");
        anim.Play("Attack");
        GetComponent<SpriteRenderer>().flipX = enemy.transform.position.x < transform.position.x ? true : false; //Sprite looks left or right
        while (enemy && status == TroopStatus.Arrived) {
            yield return new WaitForSeconds(1);
            enemy.health--;
        }
        EndAttack();
    }

    void SetEnemyTarget(Collision2D collision) {
        print("Set enemy target");
        enemy = collision.transform.GetComponent<TroopController>();
        oldTarget = target;
        Transform atkPos = GetNearestAtkPosFrom(enemy.transform);
        atkPos.GetComponent<SpriteRenderer>().color = Color.red;
        target = atkPos.position;
    }

    void EndAttack() {
        print("EndAttack");
        status = TroopStatus.Nothing;
        target = oldTarget;
    }

    IEnumerator EnteredBuildSite(BuildSiteController site) {
        canEnterAndExit = false;
        transform.localScale = new Vector2(0.7f, 0.7f); //Make small
        if (transform.parent == null) { //If no parent
            foreach (Transform pos in site.GetSpawnPositions()) {
                if (pos.childCount == 0) { //If empty pos
                    transform.SetParent(pos); //Set parent
                    target = pos.position; //Set target
                }
            }
        }
        if (transform.parent == null) { //If no empty pos was found
            Destroy(gameObject); 
        }
        yield return new WaitForSeconds(SiteEnterExitWaitTime);
        canEnterAndExit = true;
    }

    IEnumerator ExitBuildSite() {
        canEnterAndExit = false;
        transform.localScale = Vector2.one; //Make big
        transform.parent = null; //Detatch from parent
        yield return new WaitForSeconds(SiteEnterExitWaitTime);
        canEnterAndExit = true;
    }


    //################ HELPER METHODS 2 ################
    Transform GetNearestAtkPosFrom(Transform otherTroop) {
        Transform[] atkPositions = otherTroop.GetComponentsInChildren<Transform>();
        Transform nearestPos = atkPositions[0];
        Vector2 troopPos = transform.position;
        foreach (Transform pos in atkPositions) {
            if (Vector2.Distance(troopPos, pos.position) < Vector2.Distance(troopPos, nearestPos.position)) //If distance to pos < distance to nearestPos
                nearestPos = pos;
        }
        return nearestPos;
    }

    IEnumerator StuckCheck() {
        canStartStuckCheck = false;
        while (status == TroopStatus.Walk) {
            float toSlow = MaxDistance(durationToGhost) * portionOfSpeedToGhost; //Set "speed" that is to slow
            Vector2 startPos = transform.position;
            yield return new WaitForSeconds(durationToGhost);
            Vector2 currentPos = transform.position;
            if (Vector2.Distance(startPos, currentPos) < toSlow && status == TroopStatus.Walk) { //If has been walking to slow for durationToGhost
                StartCoroutine(BecomeGhost());
            }
        }
        canStartStuckCheck = true;
    }


    //################ HELPER METHODS 3 ################
    float MaxDistance(float seconds) {
        float timesMoved = seconds / 0.02f; //FixedUpdate gets called every 0.02 second.
        return speed * timesMoved;
    }

    IEnumerator BecomeGhost() {
        GetComponent<BoxCollider2D>().isTrigger = true;
        yield return new WaitForSeconds(ghostDuration);
        GetComponent<BoxCollider2D>().isTrigger = false;
    }


    //################ PUBLIC METHODS ################
    public Vector2 Target {
        set { target = value; }
    }
}
