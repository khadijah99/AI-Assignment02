using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckMyVision : MonoBehaviour
{
    public enum Sensitivity {HIGH,LOW};
    public Sensitivity sensitivity = Sensitivity.HIGH;
    public bool targetInSight = false;
    public float fieldOfVision= 45f;
    public Transform target = null;
    public Transform myEyes = null;
    public Transform npcTransform = null;
    private SphereCollider sphereCollider = null;
    public Vector3 lastknownSight = Vector3.zero;

    private void Awake()
    {
        npcTransform= GetComponent<Transform>();
        sphereCollider= GetComponent<SphereCollider>();
        lastknownSight=npcTransform.position;
        target=GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

    }

    bool  InMyFieldofVision(){
        Vector3 dirToTarget = target.position - myEyes.position;
        float angle=Vector3.Angle(myEyes.forward, dirToTarget);
        if(angle<= fieldOfVision){
            Debug.Log(angle);
            return true;
        }
        else{
            return false;

        }

    }


    void  UpdateSight(){
        switch(sensitivity){
            case Sensitivity.HIGH:
            targetInSight=InMyFieldofVision() && ClearLineOfSight();
            break;
            case Sensitivity.LOW:
            targetInSight=InMyFieldofVision() || ClearLineOfSight();
            break;
        }
    }
        bool ClearLineOfSight(){
        RaycastHit hit;
        if(Physics.Raycast(myEyes.position, (target.position - myEyes.position).normalized, out hit, sphereCollider.radius )){
            if(hit.transform.CompareTag("Player")){
                return true;
            }  
        }
        return false;
    }

    private void OnTriggerStay(Collider other){
        UpdateSight();
        if(targetInSight){
            lastknownSight=target.position;
        }
    }
    void OnTriggerExit(Collider other){
        if(other.CompareTag("Player")){
            return;
        }
        targetInSight=false;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
