using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [SerializeField]
    private float shootingInterval = 1f;                 // how often enemy shoots
    [SerializeField]
    private float shootingSpeed = 2f;                    // enemy missile speed
    [SerializeField]
    private float enemyMaximumMovemingInterval = 0.4f;   // slowest enemy movement
    [SerializeField]
    private float enemyMinimumMovemingInterval = 0.05f;  // fastest enemy movement
    [SerializeField]
    private float enemyMovingDistance = 0.1f;            // enemy shifting distance on screen
    [SerializeField]
    private float enemyHorizontalLimit = 2.5f;           // horizontal boundary
    [SerializeField]
    private GameObject enemyMissilePrefab;
    [SerializeField]
    private GameObject enemyContainer;
    [SerializeField]
    private GameObject explosionPrefab;
    [SerializeField]
    private Player player;

    private float enemyShootingTimer;
    private float enemyMovingTimer;
    private float enemyMovingDirection = -1f;           // 1: moving right, -1: moving left
    private float enemyMovingInterval;
    private int enemyCount;
    private bool loosing = false;

    private float restartTimer;

    //const
    private static float explosionDestroyingTime = 1.5f;
    private static float playerDestroyingTime = 1f;

    // Use this for initialization
    void Start () {
        restartTimer = explosionDestroyingTime + playerDestroyingTime;
        enemyShootingTimer = shootingInterval;
        enemyMovingInterval = enemyMaximumMovemingInterval;
        enemyCount = GetComponentsInChildren<Enemy>().Length;
	}
	
	// Update is called once per frame
	void Update () {

        int currentEnemyCount = GetComponentsInChildren<Enemy>().Length;

        //Enemy shooting ligic
        enemyShootingTimer -= Time.deltaTime;
        if (currentEnemyCount > 0 && enemyShootingTimer <= 0f) {
            enemyShootingTimer = shootingInterval;
            Enemy[] enemies = GetComponentsInChildren<Enemy>();
            Enemy randomEnemy = enemies[Random.Range(0, enemies.Length)];
            GameObject missileInstance = Instantiate(enemyMissilePrefab);
            missileInstance.transform.SetParent(transform);
            missileInstance.transform.position = randomEnemy.transform.position;
            missileInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(0, -shootingSpeed);
            Destroy(missileInstance, 2f);

        }

        //Enemy moving logic
        enemyMovingTimer -= Time.deltaTime;
        if (enemyMovingTimer <= 0) {
            // setting difficulty: the fewer enemies left the faster they move
            float difficulty = 1f - (float) currentEnemyCount / enemyCount;
            enemyMovingInterval = enemyMaximumMovemingInterval - (enemyMaximumMovemingInterval - enemyMinimumMovemingInterval) * difficulty;
            enemyMovingTimer = enemyMovingInterval;

            enemyContainer.transform.position = new Vector2(
                enemyContainer.transform.position.x + (enemyMovingDistance * enemyMovingDirection), 
                enemyContainer.transform.position.y
            );

            if (player != null) {
                if (enemyMovingDirection > 0) {
                    float rightMostPosition = 0f;
                    foreach (Enemy enemy in GetComponentsInChildren<Enemy>()) {
                        if (enemy.transform.position.x > rightMostPosition)
                            rightMostPosition = enemy.transform.position.x;
                    }

                    if (rightMostPosition > enemyHorizontalLimit)
                        EnemyGroupSwitchDirection();
                }
                else if (enemyMovingDirection < 0) {
                    float leftMostPosition = 0f;
                    foreach (Enemy enemy in GetComponentsInChildren<Enemy>()) {
                        if (enemy.transform.position.x < -leftMostPosition) {
                            leftMostPosition = enemy.transform.position.x;
                            // if enemy pass under player Y position - leads to restart
                            if (enemy.transform.position.y < player.transform.position.y)
                                loosing = true;
                            if (enemy.transform.position.x <= -enemyHorizontalLimit)
                                break;
                        }
                    }

                    if (leftMostPosition < -enemyHorizontalLimit)
                        EnemyGroupSwitchDirection();
                }
            }
        }

        if (loosing) {
            GameObject explosionInstance = Instantiate(explosionPrefab);
            explosionInstance.transform.SetParent(player.transform);
            explosionInstance.transform.position = player.transform.position;
            Destroy(explosionInstance, explosionDestroyingTime);

            Destroy(player.gameObject, playerDestroyingTime);
            loosing = false;
        }

        //restart game
        if (currentEnemyCount == 0 || player == null) {
            restartTimer -= Time.deltaTime;
            if (restartTimer < 0)
                SceneManager.LoadScene("Game");
        }
	}

    void EnemyGroupSwitchDirection() {
            enemyMovingDirection *= -1;
            enemyContainer.transform.position = new Vector2(
                enemyContainer.transform.position.x,
                enemyContainer.transform.position.y - enemyMovingDistance
            );
    }
}
