using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour {

    public Transform LeftFootTarget,LeftHip;
    public Transform RightFootTarget,RightHip;

    public float stepLength = 0.5f;

    
    float leftLegLength, rightLegLength;
    public IkSolver LeftIk, RightIK;



    // Use this for initialization
    void Start () {
        leftLegLength = LeftIk.GetLegLength();
        rightLegLength = RightIK.GetLegLength();


        RaycastHit hit;
        int layerMask = 1 << 8;

        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerMask))
        {
            Vector3 pos = hit.transform.position;
            pos.y += (leftLegLength + rightLegLength) / 2;
            pos.z = transform.position.z;
            pos.x = transform.position.x;
            transform.position = pos;
        }



        LeftFootTarget.position = LeftHip.position;
        LeftFootTarget.Translate(new Vector3(0, -leftLegLength, 0));

        RightFootTarget.position = RightHip.position;
        RightFootTarget.Translate(new Vector3(0, -rightLegLength, 0));

        //LeftFoot step positions
        for (int i = 1; i < 10; i += 2)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            sphere.transform.position = LeftFootTarget.position;
            
            sphere.transform.position -= Vector3.forward * stepLength * i;
        }
        //RightFoot step positions
        for (int i = 2; i < 10; i += 2)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            sphere.transform.position = RightFootTarget.position;

            sphere.transform.position -= Vector3.forward * stepLength * i;
        }




    }
	
	// Update is called once per frame
	void Update () {
       
        
        Walk();
	}


    void Walk()
    {

    }
}
