using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Baseball : MonoBehaviour {
    public TrailRenderer t;

    private Rigidbody rb;
    private float velocityMax = 200f;

	void Awake () {
        rb = gameObject.GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * Random.Range(2f, 3f), ForceMode.Impulse);
	}

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.name == "Cylinder") {
            rb.velocity = Vector3.zero;
            

            // To make sure that it will only collide once
            Physics.IgnoreCollision(collision.gameObject.GetComponent<CapsuleCollider>(), gameObject.GetComponent<SphereCollider>());

            float forceMultiplier = GetBatForce(collision.gameObject.GetComponent<Rigidbody>());
            Vector3 direction = (transform.position - collision.contacts[0].point).normalized;
            // print("Direction: ");
            // print(direction);
            rb.AddForce(direction * forceMultiplier, ForceMode.Impulse);
            rb.useGravity = true;

            t.enabled = true;
            Destroy(gameObject, 2f);
        }
    }

    private float GetBatForce(Rigidbody batRB) {
        return batRB.velocity.magnitude / velocityMax * 2f;
    }
}
