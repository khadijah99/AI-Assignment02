using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
   public float Healthpoints{
   get{
       return healthpoints;
   }
   set{
       healthpoints=value;
       if(healthpoints<=0){
           Destroy(gameObject);
       }
   }
   }
  [SerializeField]
public float healthpoints=100f;
}

