using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    private Rigidbody rb;
    public Vector3 myVelocity;
    [SerializeField]
    private float lifeTime;
    public GameObject hitBoxPrefab;

    [SerializeField]
    private float damageModifier;

    private bool hitSomething;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
	}

    private void Update()
    {
        if (!rb.isKinematic)
        {
            lifeTime -= Time.deltaTime;
            if (lifeTime <= 0)
            {
                gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate () {
		if (!rb.isKinematic && rb.velocity == Vector3.zero)
        {
            rb.velocity = myVelocity;
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (!hitSomething)
        {
            Hitbox hitBox;
            hitBox = Instantiate(hitBoxPrefab, transform).GetComponent<Hitbox>();
            hitBox.transform.position = transform.position;
            hitBox.transform.rotation = transform.rotation;
            hitBox.transform.localScale = hitBox.transform.localScale * 2;
            hitBox.activeTime = 1;
            hitBox.delayTime = 0;
            hitBox.stunTime = 1;
            hitBox.damage = 15*damageModifier;
            hitBox.knockBackSpeed = 1;
            hitSomething = true;
            lifeTime = 0.1f;
            myVelocity = Vector3.zero;
        }
    }
}
