using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TroopController : MonoBehaviour {

    [SerializeField] [Range(2, 4)] int speed;
    [Range(1, 5)] public int health = 3;
    [SerializeField] [Range(0.5f, 1.5f)] float ghostDuration;

    float durationToGhost = 1f;
    float distanceToTarget = 0.1f;
    bool canStartStuckCheck = true;
    bool move = true;
    bool canHitWall1 = true;
    bool canHitWall2 = true;
    bool obstacleMove = false;
    Vector2 obstaclePos;
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
        enemyTag = tag == "AITroop" ? "PlayerTroop" : "AITroop"; //If you are an enemy, your enemy is Troop.
        enemyTowerTag = tag == "AITroop" ? "PlayerTower" : "AITower";
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
            Move();

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

            if (obstaclePos != null && Vector2.Distance(transform.position, obstaclePos) < distanceToTarget) { //If close to obstaclePos
                obstacleMove = false;
            }
        }
    }

    //---- TRIGGER METHODS ----
    private void OnCollisionStay2D(Collision2D collision) {
        if (status != TroopStatus.Attack && collision.transform.tag == enemyTag) { //Collided with Enemy
            enemy = collision.transform.GetComponent<TroopController>(); //Set enemy
            StartCoroutine(Attack());
        } else if (collision.transform.tag == "Wall1" && canHitWall1) { //Collided with wall1
            obstaclePos = collision.transform.Find("Pos").position;
            obstacleMove = true;
            canHitWall1 = false;
        } else if (collision.transform.tag == "Wall2" && canHitWall2) { //Collided with wall2
            obstaclePos = collision.transform.Find("Pos").position;
            obstacleMove = true;
            canHitWall2 = false;
        }
    }

    private void OnTriggerStay2D(Collider2D collision) { //Collided with EnemyTower
        if (status != TroopStatus.Attack && collision.transform.tag == enemyTowerTag) {
            enemyTower = collision.transform.GetComponent<TowerController>(); //Set enemy tower
            StartCoroutine(Attack());
        }
    }


    //################ HELPER METHODS 1 ################
    void Move() {
        if (move) {
            Vector2 destination = new Vector2();
            destination = obstacleMove ? obstaclePos : target; //Move to obstaclePos or target
            transform.position = Vector3.MoveTowards(transform.position, destination, (float)(speed) / 100); //Every time, move "speed units" closer to target
            GetComponent<SpriteRenderer>().flipX = destination.x < transform.position.x ? true : false; //Sprite looks left or right
        }

    }

    void Die() {
        GetComponent<AudioSource>().Play();
        dead = true;
        EndAttack();
        anim.Play(dieHash);
        GetComponent<CapsuleCollider2D>().enabled = false;
        GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        Destroy(gameObject, 4);
    }

    void BeginWalk() {
        transform.localScale = Vector2.one; //Make big
        status = TroopStatus.Walk;
        anim.Play(walkHash);
        if (canStartStuckCheck)
            StartCoroutine(StuckCheck()); //If stuck become ghost temporary
    }

    void BeginIdle() {
        transform.localScale = new Vector2(0.7f, 0.7f); //Make small
        status = TroopStatus.Idle;
        anim.Play(idleHash);
    }

    IEnumerator Attack() {
        transform.localScale = Vector2.one; //Make big
        move = false;
        status = TroopStatus.Attack;
        atkPos = transform.position;
        anim.Play(attackHash);

        if (enemy != null) {
            while (enemy.health > 0 && status == TroopStatus.Attack && enemy) {
                enemy.health--;
                GetComponents<AudioSource>()[1].Play();
                yield return new WaitForSeconds(0.5f);
                GetComponents<AudioSource>()[2].Play();
                yield return new WaitForSeconds(0.5f);

            }
        } else if (enemyTower != null) {
            while (enemyTower.health > 0 && status == TroopStatus.Attack && enemyTower) {
                enemyTower.health--;
                List<Transform> troopsAtCite = enemyTower.GetComponentsInParent<BuildSiteController>()[0].GetTroops();
                foreach (Transform troop in troopsAtCite) {
                    troop.GetComponent<TroopController>().MakeObstacleMove(transform.position);
                }
                GetComponents<AudioSource>()[1].Play();
                yield return new WaitForSeconds(1);
            }
        }

        EndAttack();
    }

    void EndAttack() {
        status = TroopStatus.Nothing;
        move = true;
    }

    void ReachedBuildSite() {
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
        canStartStuckCheck = false;
        while (status == TroopStatus.Walk) {
            float toSlow = 0.3f; //Set "speed" that is to slow
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
    IEnumerator BecomeGhost() {
        GetComponent<CapsuleCollider2D>().isTrigger = true;
        yield return new WaitForSeconds(ghostDuration);
        GetComponent<CapsuleCollider2D>().isTrigger = false;
    }


    //################ PUBLIC METHODS ################
    public Vector2 Target {
        set { target = value; }
    }

    public Transform HomeBuildSite {
        set { homeBuildSite = value; }
    }

    public void ResetCanHitObstacle () {
        canHitWall1 = true;
        canHitWall2 = true;
        obstacleMove = false;
    }

    public void MakeObstacleMove(Vector2 pos) {
        obstacleMove = true;
        obstaclePos = pos;
    }
}
