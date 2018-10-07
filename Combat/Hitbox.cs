using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour {

    public List<BaseCharacter> hitObjects = new List<BaseCharacter>();
    public BaseCharacter currentHitObject;

    public float stunTime;
    public float activeTime;
    public float delayTime;
    public float damage;
    public float knockBackSpeed;

    public bool startedActiveTime = false;

    private void Update()
    {
        if (GameManager.instance.gameState == GameManager.gameStates.observeTime)
            Destroy(gameObject);
        if (!transform.root.GetComponent<Rigidbody>().isKinematic)
        {
            //Activate hitbox after a given delay, then let it stay active for a set amount of time
            if (delayTime >= 0)
            {
                delayTime -= Time.deltaTime;
            }
            else
            {
                if (!startedActiveTime)
                {
                    GetComponent<BoxCollider>().enabled = true;
                    GetComponent<MeshRenderer>().enabled = true;
                    startedActiveTime = true;
                }
                activeTime -= Time.deltaTime;
                if (activeTime <= 0)
                {
                    Destroy(gameObject);
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
            //If you hit another character than the character who spawned this, than add him to the already hit characters and call the ReceiveHit function on him
            if (other.gameObject.transform.root.GetComponent<BaseCharacter>() != transform.root.GetComponent<BaseCharacter>() && !hitObjects.Contains(other.gameObject.transform.root.gameObject.GetComponent<BaseCharacter>()))
            {
                currentHitObject = other.gameObject.transform.root.GetComponent<BaseCharacter>();
                hitObjects.Add(currentHitObject);
                currentHitObject.ReceiveHit(transform.root.rotation.eulerAngles.y,knockBackSpeed*transform.root.transform.forward,stunTime,damage);
            }
    }
}
