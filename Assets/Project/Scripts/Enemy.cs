using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    [SerializeField]
    private GameObject explosionPrefab;

    //const
    private static float explosionDestroyingTime = 1.5f;

	// Use this for initialization
	void Start () {
		
	}

    void OnTriggerEnter2D(Collider2D otherCollider) {
        if (otherCollider.CompareTag("PlayerMissile")) {
            //Explosion
            GameObject explosionInstance = Instantiate(explosionPrefab);
            explosionInstance.transform.SetParent(transform.parent.parent);
            explosionInstance.transform.position = transform.position;
            Destroy(explosionInstance, explosionDestroyingTime);

            //destroying enemy and missle
            Destroy(gameObject);
            Destroy(otherCollider.gameObject);
        }
    }
}
