using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Walker : MonoBehaviour {

  

    public float stepLength = 0.5f;
    public float legSpeed = 1;

    public float Uprightness = 1;

    public float Weight = 10;
    public List<IkSolver> legs = new List<IkSolver>();
    Transform[] targets;
    float[] legOffsets;

    public BezierSpline Spline;

    public bool WillWalk = false;

    public float NoiseX = 0.2f, NoiseY = 0.3f, NoiseZ = 0.4f;

    public BezierSpline stepTracjectory;
    

    

   
    

    List<Vector3> Steps = new List<Vector3>();
    
    List<float> stepPositionInSpline = new List<float>();

    

    // Use this for initialization
    void Start () {

        transform.position = new Vector3(0, 5, 0);
        legOffsets = new float[legs.Count];
        for (int i = 0; i < legOffsets.Length; i++)
        {

            legOffsets[i] = legs[i % legs.Count].transform.position.x - transform.position.x;
            

            

        }

        
        targets = new Transform[legs.Count];

        

        RaycastHit hit;
        int layerMask = 1 << 8;

        
        transform.position = new Vector3(Spline.gameObject.transform.position.x+ Spline.points[0].x, transform.position.y, Spline.gameObject.transform.position.z+ Spline.points[0].z);




        transform.LookAt(new Vector3(Spline.GetPoint(0.01f).x, transform.position.y, Spline.GetPoint(0.01f).z));



        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100, layerMask))
        {
            Vector3 pos = hit.transform.position;
            pos.y += legs[0].GetLegLength()* Uprightness;
            pos.z = transform.position.z;
            pos.x = transform.position.x;
            transform.position = pos;
        }



        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = new GameObject().transform;

            legs[i].target = targets[i];
            targets[i].position = legs[i].transform.position;
            targets[i].position = new Vector3(targets[i].position.x, targets[i].position.y - (legs[i].GetLegLength() * Uprightness), targets[i].position.z);

        }

        

        //LeftFoot step positions
        
        float CheckedDis = 0;
        Vector3 lastStepPos = targets[0].position;


        //the big step meister
        bool  done = false;
        int denstoretælleleg = 0;
        while (! done)
        {


            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            sphere.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);


            //Debug.Log(i % 2);
            
                sphere.transform.position = Spline.GetPoint(0);
                sphere.transform.LookAt(Spline.GetPoint(0) + Spline.GetDirection(0));
                sphere.transform.Translate(legOffsets[denstoretælleleg % legs.Count], 0, 0);
            
            
            
            while (true)
            {
                CheckedDis += 0.005f;
                sphere.transform.position = Spline.GetPoint(CheckedDis);
                sphere.transform.LookAt(Spline.GetPoint(CheckedDis) + Spline.GetDirection(CheckedDis));
                sphere.transform.Translate(legOffsets[denstoretælleleg % legs.Count], 0, 0);
                

                if(Vector3.Distance(lastStepPos,sphere.transform.position)>=stepLength)
                    break;

                if (CheckedDis > 1)
                {
                    done = true;
                    break;
                    
                }
            }
                stepPositionInSpline.Add(CheckedDis);
                Steps.Add(sphere.transform.position);
                lastStepPos = sphere.transform.position;
            denstoretælleleg++;
            Destroy(sphere);
           
        }
        CheckedDis = 0;
        
        


    }
	
	// Update is called once per frame
	void Update () {
       
        if(WillWalk)
        Walk();
	}

    bool leftStepTaken = false;


    int currentStep = 0;
    void Walk()
    {
        TakeStep();
        /*
        if (leftStepTaken == false)
            TakeLeftStep();
        else
            TakeRightStep();
        */
    }
    bool FootInPos = false;
    bool bodyInPos = false;


    float bodyProgress = 0;

    Vector3 tempFootPos;
    bool firstFrame = true;
    float stepProgress = 0;
    

    float currentStepLength = 0;
    float prevDistance =0;
    Quaternion rotatttin;
    void TakeStep()
    {
        
        if (!FootInPos)
        {

            if (firstFrame)
            {
                
                //transform.LookAt(new Vector3(Spline.GetPoint(stepPositionInSpline[0]).x, transform.position.y, Spline.GetPoint(stepPositionInSpline[0]).z));

                
                
                
               

                stepTracjectory.points[0] = targets[currentStep].position;
                stepTracjectory.points[1] = targets[currentStep].position + new Vector3(0 + Random.Range(-NoiseX, NoiseX), 1 + Random.Range(-NoiseY, NoiseY), 0 + Random.Range(-NoiseZ, NoiseZ));
                stepTracjectory.points[2] = Steps[0] + new Vector3(0 + Random.Range(-NoiseX, NoiseX), 1 + Random.Range(-NoiseY, NoiseY), 0 + Random.Range(-NoiseZ, NoiseZ));
                stepTracjectory.points[3] = Steps[0];

                firstFrame = false;
            }

            //Debug.Log(stepProgress);

            stepProgress += Time.deltaTime * legSpeed;


            //rFootTarget.transform.RotateAround(tempFootPos, new Vector3(-1,1,0), 40 *Time.deltaTime);
            //rFootTarget.transform.position = tempFootPos + new Vector3(0, Mathf.Sin(stepProgress) * (currentStepLength), Mathf.Cos(stepProgress) * (currentStepLength));

            //rFootTarget.position = tempFootPos;
            //rFootTarget.Translate(new Vector3(0, Mathf.Sin(stepProgress) * (currentStepLength/2), Mathf.Cos(stepProgress) * (currentStepLength / 2)));

            targets[currentStep].position = stepTracjectory.GetPoint(stepProgress);
            

            //if ((targets[currentStep].position - Steps[0]).magnitude < 0.05f)
            if(targets[currentStep].position == stepTracjectory.GetPoint(1))
            {
                //Debug.Log("Step Taken");
                firstFrame = true;
                stepProgress = 0;
                FootInPos = true;
                
            }
        }
        else if (!bodyInPos)
        {
            bodyProgress += Time.deltaTime/100;
            //transform.LookAt(new Vector3(Spline.GetPoint(bodyProgress + 0.01f).x,transform.position.y, Spline.GetPoint(bodyProgress+0.01f).z));
            
            transform.LookAt(new Vector3(Spline.GetPoint(stepPositionInSpline[0]).x, transform.position.y, Spline.GetPoint(stepPositionInSpline[0]).z));
            //transform.position = new Vector3(Spline.GetPoint(bodyProgress).x, transform.position.y, Spline.GetPoint(bodyProgress).z);
            transform.Translate(Vector3.forward * Time.deltaTime);
            


            

            
            //if (prevDistance<number && prevDistance != 0)
            if(Vector3.Distance( transform.position, new Vector3(Spline.GetPoint(stepPositionInSpline[0]).x, transform.position.y, Spline.GetPoint(stepPositionInSpline[0]).z))<0.1)
            {
               // Debug.Log("BOdy moved");
                bodyInPos = true;
                
                
            }
            
            
        }
        else
        {

            stepPositionInSpline.RemoveAt(0);
            leftStepTaken = true;
            FootInPos = false;
            bodyInPos = false;
            Steps.RemoveAt(0);
            currentStep = (currentStep + 1) % legs.Count;
        }
    }
}
