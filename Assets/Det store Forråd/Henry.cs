using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Henry : MonoBehaviour {

    float CuntSucker = 0;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        CuntSucker += Time.deltaTime;
        print(Mathf.Sin(CuntSucker));
        transform.Translate(new Vector3( 0, (Mathf.Sin(CuntSucker)/2)*Time.deltaTime, 0));
	}
}
