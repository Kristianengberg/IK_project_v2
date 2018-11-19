using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour {

    public Transform rFootTarget,LeftHip;
    public Transform lFootTarget,RightHip;

    public float stepLength = 0.5f;

    public BezierSpline Spline;

    public bool WillWalk = false;


    public GameObject rotatePoint;


    float leftLegLength, rightLegLength;
    public IkSolver LeftIk, RightIK;

    List<Vector3> leftSteps = new List<Vector3>();
    List<Vector3> rightSteps = new List<Vector3>();

    Vector3 rotationPosition;

    // Use this for initialization
    void Start () {
        leftLegLength = LeftIk.GetLegLength();
        rightLegLength = RightIK.GetLegLength();
        

        RaycastHit hit;
        int layerMask = 1 << 8;

        
        transform.position = new Vector3(Spline.gameObject.transform.position.x+ Spline.points[0].x, transform.position.y, Spline.gameObject.transform.position.z+ Spline.points[0].z);




        transform.LookAt(new Vector3(Spline.GetPoint(0.01f).x, transform.position.y, Spline.GetPoint(0.01f).z));



        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerMask))
        {
            Vector3 pos = hit.transform.position;
            pos.y += ((leftLegLength + rightLegLength) / 2)+0.1f;
            pos.z = transform.position.z;
            pos.x = transform.position.x;
            transform.position = pos;
        }
        
        

        rFootTarget.position = LeftHip.position;
        rFootTarget.Translate(new Vector3(0, -leftLegLength, 0));

        lFootTarget.position = RightHip.position;
        lFootTarget.Translate(new Vector3(0, -rightLegLength, 0));

        //LeftFoot step positions
        
        float CheckedDis = 0;
        Vector3 lastStepPos = lFootTarget.position;
        for (int i = 0; i < 20; i++)
        {


            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


            
               
            sphere.transform.position = Spline.GetPoint(0);
            sphere.transform.LookAt(Spline.GetPoint(0)+Spline.GetDirection(0));
            sphere.transform.Translate(-0.5f, 0, 0);

            
            while (true)
            {
                CheckedDis += 0.005f;
                sphere.transform.position = Spline.GetPoint(CheckedDis);
                sphere.transform.LookAt(Spline.GetPoint(CheckedDis) + Spline.GetDirection(CheckedDis));
                sphere.transform.Translate(-0.5f, 0, 0);
                

                if(Vector3.Distance(lastStepPos,sphere.transform.position)>=stepLength)
                    break;

                if (CheckedDis > 1)
                    break;
            }
            

            leftSteps.Add(sphere.transform.position);
            lastStepPos = sphere.transform.position;

            
           
        }
        CheckedDis = 0;
        lastStepPos = rFootTarget.position;
        bool offset = false;
        //RightFoot step positions
        for (int i = 0; i < 20; i++)
        {


            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);




            sphere.transform.position = Spline.GetPoint(0);
            sphere.transform.LookAt(Spline.GetPoint(0) + Spline.GetDirection(0));
            sphere.transform.Translate(0.5f, 0, 0);


            while (true)
            {
                CheckedDis += 0.005f;
                sphere.transform.position = Spline.GetPoint(CheckedDis);
                sphere.transform.LookAt(Spline.GetPoint(CheckedDis) + Spline.GetDirection(CheckedDis));
                sphere.transform.Translate(0.5f, 0, 0);

                if(offset == false && Vector3.Distance(lastStepPos, sphere.transform.position) >= stepLength/2)
                {
                    
                    offset = true;
                    break;
                }

                else if (Vector3.Distance(lastStepPos, sphere.transform.position) >= stepLength)
                    break;

                if (CheckedDis > 1)
                    break;
            }
            

            rightSteps.Add(sphere.transform.position);
            lastStepPos = sphere.transform.position;



        }




    }
	
	// Update is called once per frame
	void Update () {
       
        if(WillWalk)
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


    float bodyProgress = 0;

    Vector3 tempFootPos;
    bool firstFrame = true;
    float stepProgress = Mathf.PI;
    void TakeRightStep()
    {
        if (!FootInPos)
        {

            if (firstFrame)
            {
                Vector3 tempPos = lFootTarget.transform.position;

                lFootTarget.GetComponent<Renderer>().material.color = Color.yellow;

                Debug.Log(leftSteps[0]);
                lFootTarget.transform.LookAt(leftSteps[0]);


                currentStepLength = Vector3.Distance(lFootTarget.transform.position, leftSteps[0]);
                lFootTarget.transform.Translate(Vector3.forward * currentStepLength / 2);
                tempFootPos = lFootTarget.transform.position;

                rotatePoint.transform.position = tempFootPos;

                lFootTarget.transform.position = tempPos;
                rotatttin = lFootTarget.transform.rotation;

                firstFrame = false;
            }



            stepProgress -= Time.deltaTime;


            //rFootTarget.transform.RotateAround(tempFootPos, new Vector3(-1,1,0), 40 *Time.deltaTime);
            //rFootTarget.transform.position = tempFootPos + new Vector3(0, Mathf.Sin(stepProgress) * (currentStepLength), Mathf.Cos(stepProgress) * (currentStepLength));

            lFootTarget.position = tempFootPos;
            lFootTarget.Translate(new Vector3(0, Mathf.Sin(stepProgress) * (currentStepLength / 2), Mathf.Cos(stepProgress) * (currentStepLength / 2)));



            if ((lFootTarget.position - leftSteps[0]).magnitude < 0.05f)
            {
                Debug.Log("Step Taken");
                firstFrame = true;
                stepProgress = Mathf.PI;
                FootInPos = true;
            }
        }
        else if (!bodyInPos)
        {
            bodyProgress += Time.deltaTime / 100;
            transform.LookAt(new Vector3(Spline.GetPoint(bodyProgress + 0.01f).x, transform.position.y, Spline.GetPoint(bodyProgress + 0.01f).z));
            transform.position = new Vector3(Spline.GetPoint(bodyProgress).x, transform.position.y, Spline.GetPoint(bodyProgress).z);



            float number = Vector3.Distance(transform.position, lFootTarget.position);


            if (prevDistance < number && prevDistance != 0)
            {
                Debug.Log("BOdy moved");
                bodyInPos = true;
                prevDistance = 0;
            }
            if (bodyInPos == false)
                prevDistance = number;
        }
        else
        {


            leftStepTaken = false;
            FootInPos = false;
            bodyInPos = false;
            leftSteps.RemoveAt(0);
        }
    }

    float currentStepLength = 0;
    float prevDistance =0;
    Quaternion rotatttin;
    void TakeLeftStep()
    {
        
        if (!FootInPos)
        {

            if (firstFrame)
            {
                Vector3 tempPos = rFootTarget.transform.position;

                rFootTarget.GetComponent<Renderer>().material.color = Color.yellow;

                Debug.Log(rightSteps[0]);
                rFootTarget.transform.LookAt(rightSteps[0]);


                currentStepLength = Vector3.Distance(rFootTarget.transform.position, rightSteps[0]);
                rFootTarget.transform.Translate(Vector3.forward * currentStepLength / 2);
                tempFootPos = rFootTarget.transform.position;

                rotatePoint.transform.position = tempFootPos;

                rFootTarget.transform.position = tempPos;
                rotatttin = rFootTarget.transform.rotation;

                firstFrame = false;
            }



            stepProgress -= Time.deltaTime;


            //rFootTarget.transform.RotateAround(tempFootPos, new Vector3(-1,1,0), 40 *Time.deltaTime);
            //rFootTarget.transform.position = tempFootPos + new Vector3(0, Mathf.Sin(stepProgress) * (currentStepLength), Mathf.Cos(stepProgress) * (currentStepLength));

            rFootTarget.position = tempFootPos;
            rFootTarget.Translate(new Vector3(0, Mathf.Sin(stepProgress) * (currentStepLength/2), Mathf.Cos(stepProgress) * (currentStepLength / 2)));
            

            if ((rFootTarget.position - rightSteps[0]).magnitude < 0.05f)
            {
                Debug.Log("Step Taken");
                firstFrame = true;
                stepProgress = Mathf.PI;
                FootInPos = true;
            }
        }
        else if (!bodyInPos)
        {
            bodyProgress += Time.deltaTime/100;
            transform.LookAt(new Vector3(Spline.GetPoint(bodyProgress + 0.01f).x,transform.position.y, Spline.GetPoint(bodyProgress+0.01f).z));
            transform.position = new Vector3(Spline.GetPoint(bodyProgress).x, transform.position.y, Spline.GetPoint(bodyProgress).z);



            float number = Vector3.Distance(transform.position, rFootTarget.position);

            
            if (prevDistance<number && prevDistance != 0)
            {
                Debug.Log("BOdy moved");
                bodyInPos = true;
                prevDistance = 0;
                
            }
            if(bodyInPos== false)
            prevDistance = number;
        }
        else
        {


            leftStepTaken = true;
            FootInPos = false;
            bodyInPos = false;
            rightSteps.RemoveAt(0);
        }
    }
}
