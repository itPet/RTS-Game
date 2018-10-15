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
    bool canStartStuckCheck = true;
    bool move = true;
    bool dead = false;
    int walkHash = Animator.StringToHash("Walk");
    int attackHash = Animator.StringToHash("Attack");
    int idleHash = Animator.StringToHash("Idle");
    int dieHash = Animator.StringToHash("Die");

    TroopController enemy;
    TowerController enemyTower;
    string enemyTag;
    string enemyTowerTag;
    Transform homeBuildSite;
    Vector2 target;
    Vector2 oldTarget;
    Animator anim;
    Vector2 atkPos;

    public int health = 3;
    public TroopStatus status = TroopStatus.Nothing;
    public enum TroopStatus {
        Nothing,
        Walk,
        Idle,
        Attack
    }

    //################ STARTER METHODS ################
    void Start() { //Set variables
        target = transform.parent == null ? transform.position : transform.parent.position;
        enemyTag = tag == "Enemy" ? "Troop" : "Enemy"; //If you are an enemy, your enemy is Troop.
        enemyTowerTag = tag == "Enemy" ? "Tower" : "EnemyTower";
        anim = GetComponent<Animator>();
    }


    //################ UPDATE METHODS ################
    private void Update() {
        if (!dead) {
            transform.rotation = Quaternion.Euler(Vector3.zero); //Always don't rotate
            if (health < 1)
                Die();
        }

    }

    private void FixedUpdate() {
        if (!dead) {
            MoveToTarget();

            if (status == TroopStatus.Attack) {
                transform.position = atkPos;
            }

            if (Vector2.Distance(transform.position, target) < distanceToTarget) { //If close to target
                if (status != TroopStatus.Idle && status != TroopStatus.Attack) {
                    if (transform.parent == null && homeBuildSite != null) {
                        ReachedBuildSite();
                    }

                    BeginIdle();
                }
            } else { //If far from target and not attack
                if (status != TroopStatus.Walk && status != TroopStatus.Attack)
                    BeginWalk();
            }
        }
    }

    //---- TRIGGER METHODS ----
    private void OnCollisionStay2D(Collision2D collision) { //Collided with Enemy
        if (status != TroopStatus.Attack && collision.transform.tag == enemyTag) {
            enemy = collision.transform.GetComponent<TroopController>(); //Set enemy
            StartCoroutine(Attack());
        }
    }

    private void OnTriggerStay2D(Collider2D collision) {
        if (status != TroopStatus.Attack && collision.transform.tag == enemyTowerTag) {
            enemyTower = collision.transform.GetComponent<TowerController>(); //Set enemy tower
            StartCoroutine(Attack());
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision) { //Enter BuildSite
    //    if (collision.transform.tag == "BuildSite" && collision.transform == homeBuildSite) {
    //        ReachedBuildSite(collision.GetComponent<BuildSiteController>());
    //    } 
    //}


    //################ HELPER METHODS 1 ################
    void MoveToTarget() {
        if (move) {
            transform.position = Vector3.MoveTowards(transform.position, target, speed); //Every time, move "speed units" closer to target
            GetComponent<SpriteRenderer>().flipX = target.x < transform.position.x ? true : false; //Sprite looks left or right
        }
    }

    void Die() {
        print(tag + " died");

        dead = true;
        EndAttack();
        anim.Play(dieHash);
        GetComponent<BoxCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        Destroy(gameObject, 4);
    }

    void BeginWalk() {
        status = TroopStatus.Walk;
        anim.Play(walkHash);
        if (canStartStuckCheck)
            StartCoroutine(StuckCheck()); //If stuck become ghost temporary
    }

    void BeginIdle() {
        status = TroopStatus.Idle;
        anim.Play(idleHash);
    }

    IEnumerator Attack() {
        move = false;
        status = TroopStatus.Attack;
        atkPos = transform.position;
        anim.Play(attackHash);
        if (enemy != null) {
            while (enemy.health > 0 && status == TroopStatus.Attack && enemy) {
                enemy.health--;
                yield return new WaitForSeconds(1);
            }
        }

        if (enemyTower != null) {
            while (enemyTower.health > 0 && status == TroopStatus.Attack && enemyTower) {
                enemyTower.health--;
                yield return new WaitForSeconds(1);
            }
        }

        EndAttack();
    }

    //void SetEnemyTarget(Collision2D collision) {
    //    enemy = collision.transform.GetComponent<TroopController>();
    //    oldTarget = target;
    //    Transform atkPos = GetNearestAtkPosFrom(enemy.transform);
    //    atkPos.GetComponent<SpriteRenderer>().color = Color.red;
    //    target = atkPos.position;
    //}

    void EndAttack() {
        status = TroopStatus.Nothing;
        move = true;
    }

    void ReachedBuildSite() {
        transform.localScale = new Vector2(0.7f, 0.7f); //Make small
        foreach (Transform pos in homeBuildSite.GetComponent<BuildSiteController>().GetSpawnPositions()) {
            if (pos.childCount == 0 || (pos.GetChild(0).tag == enemyTag && pos.childCount < 2)) {
                transform.SetParent(pos); //Set parent
                target = pos.position; //Set target
            }
        }
        
        if (transform.parent == null) { //If no empty pos was found
            Destroy(gameObject); 
        }
    }



    //################ HELPER METHODS 2 ################
    IEnumerator StuckCheck() {
        print(tag + " StuckCheck");
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
        print(tag + " became ghost");
        GetComponent<BoxCollider2D>().isTrigger = true;
        yield return new WaitForSeconds(ghostDuration);
        GetComponent<BoxCollider2D>().isTrigger = false;
    }


    //################ PUBLIC METHODS ################
    public Vector2 Target {
        set { target = value; }
    }

    public Transform HomeBuildSite {
        set { homeBuildSite = value; }
    }
}
