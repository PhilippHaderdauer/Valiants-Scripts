using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathTrigger : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        other.transform.root.GetComponent<BaseCharacter>().health = 0;
    }
}
