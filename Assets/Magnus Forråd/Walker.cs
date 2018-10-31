using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour {

    public Transform LeftFootTarget,LeftHip;
    public Transform RightFootTarget,RightHip;

    public float stepLength = 0.5f;

    
    float leftLegLength, rightLegLength;
    public IkSolver LeftIk, RightIK;

    List<Vector3> leftSteps = new List<Vector3>();
    List<Vector3> rightSteps = new List<Vector3>();
    


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
        for (int i = 1; i < 20; i += 2)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            
            sphere.transform.position = LeftFootTarget.position;
            
            sphere.transform.position -= Vector3.forward * stepLength * i;

            leftSteps.Add(sphere.transform.position);
        }
        //RightFoot step positions
        for (int i = 2; i < 20; i += 2)
        {
            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

            sphere.transform.position = RightFootTarget.position;

            sphere.transform.position -= Vector3.forward * stepLength * i;
            rightSteps.Add(sphere.transform.position);
        }




    }
	
	// Update is called once per frame
	void Update () {
       
        
        Walk();
	}

    bool leftStepTaken = false;
    void Walk()
    {
        if (leftStepTaken == false)
            TakeLeftStep();
        else
            TakeRightStep();
    }
    bool FootInPos = false;
    bool bodyInPos = false;

    float stepProgress = 0;
    void TakeRightStep()
    {
        if (!FootInPos)
        {
            //RightFootTarget.LookAt(rightSteps[0]);


            stepProgress += Time.deltaTime;


            // RightFootTarget.transform.Translate(Vector3.forward * Time.deltaTime );
            // RightFootTarget.transform.Translate(Vector3.up * (Mathf.Sin(Vector3.Normalize((RightFootTarget.position - rightSteps[0])).magnitude) * (stepLength / 2)) * Time.deltaTime);


            Vector3 tempPos = RightFootTarget.transform.position;
            RightFootTarget.transform.position = new Vector3(0, Mathf.Sin(stepProgress)*(stepLength/2),Mathf.Cos(stepProgress) * (stepLength / 2));



            if ((RightFootTarget.position - rightSteps[0]).magnitude < 0.05f)
            {
                Debug.Log("Step Taken");
                FootInPos = true;
            }
        }
        else if (!bodyInPos)
        {
            transform.Translate(Vector3.forward * -Time.deltaTime);

            if (transform.position.z - RightFootTarget.position.z < 0.05f)
            {
                Debug.Log("BOdy moved");
                bodyInPos = true;
            }
        }
        else
        {


            leftStepTaken = false;
            FootInPos = false;
            bodyInPos = false;
            rightSteps.RemoveAt(0);
        }
    }
    void TakeLeftStep()
    {
        if (!FootInPos)
        {
            LeftFootTarget.LookAt(leftSteps[0]);


            LeftFootTarget.transform.Translate(Vector3.forward * Time.deltaTime);
            LeftFootTarget.transform.Translate(Vector3.up * (Mathf.Sin(Vector3.Normalize( (LeftFootTarget.position - leftSteps[0])).magnitude)* (stepLength / 2)) * Time.deltaTime);

            if ((LeftFootTarget.position - leftSteps[0]).magnitude < 0.05f)
            {
                Debug.Log("Step Taken");
                FootInPos = true;
            }
        }
        else if (!bodyInPos)
        {
            transform.Translate(Vector3.forward * -Time.deltaTime);

            if (transform.position.z - LeftFootTarget.position.z < 0.05f)
            {
                Debug.Log("BOdy moved");
                bodyInPos = true;
            }
        }
        else
        {


            leftStepTaken = true;
            FootInPos = false;
            bodyInPos = false;
            leftSteps.RemoveAt(0);
        }
    }
}
