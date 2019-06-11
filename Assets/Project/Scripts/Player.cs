using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField]
    private float speed = 1.5f;                  // moving speed
    [SerializeField]
    private float firingSpeed = 4f;              // player missile speed
    [SerializeField]
    private float firingCooldownDuration = 1f;   // cooldown beetwen shots
    [SerializeField]
    private float horizontalLimit = 2.7f;        // horizontal boundary

    [SerializeField]
    private GameObject missilePrefab;
    [SerializeField]
    private GameObject explosionPrefab;

    private float cooldownTimer;

    //const
    private static float explosionDestroyingTime = 1.5f;
    private static float missileDestroyingTime = 2f;

    private Rigidbody2D rb;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {
        rb.velocity = new Vector2(Input.GetAxis("Horizontal") * speed, 0);

        //keep player within bounce
        if (transform.position.x > horizontalLimit) {
            transform.position = new Vector3(horizontalLimit, transform.position.y);
            rb.velocity = Vector2.zero;
        }
        else if (transform.position.x < -horizontalLimit) {
            transform.position = new Vector3(-horizontalLimit, transform.position.y);
            rb.velocity = Vector2.zero;
        }

        //fire missle
        cooldownTimer -= Time.deltaTime;
        if (cooldownTimer <= 0 && Input.GetAxis("Fire1") == 1f) {
            cooldownTimer = firingCooldownDuration;
            GameObject missileInstance = Instantiate(missilePrefab);
            missileInstance.transform.SetParent(transform.parent);

            missileInstance.transform.position = transform.position;
            missileInstance.GetComponent<Rigidbody2D>().velocity = new Vector2(0, firingSpeed);
            Destroy(missileInstance, missileDestroyingTime);
        }
    }

    void OnTriggerEnter2D(Collider2D otherCollider) {
        if (otherCollider.CompareTag("EnemyMissile") || otherCollider.CompareTag("Enemy")) {
            //Explosion
            GameObject explosionInstance = Instantiate(explosionPrefab);
            explosionInstance.transform.SetParent(transform.parent);
            explosionInstance.transform.position = transform.position;
            Destroy(explosionInstance, explosionDestroyingTime);

            //destroying enemy and missle
            Destroy(gameObject);
            Destroy(otherCollider.gameObject);
        }
    }
}
