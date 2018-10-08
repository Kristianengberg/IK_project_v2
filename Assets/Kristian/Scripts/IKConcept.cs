using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKConcept : MonoBehaviour {

    public Transform child;
    public float maxRange = 2.5f;
    

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        var heading = child.position - transform.position;
        var distance = heading.magnitude;
        var direction = heading / distance;

        if (heading.sqrMagnitude > maxRange * maxRange)
        {
            Vector3 relative = transform.InverseTransformPoint(child.position);
            float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
            transform.Rotate(0, 0, angle);
        }
    }
}
