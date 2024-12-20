using UnityEngine;


   public class SkillScript : MonoBehaviour
   {
      public ESkillType skillType;
   
      public  float speed        = 5f;  
      public  float duration     = 10f; 
      private float _elapsedTime = 0f;

      public void Initialize(ESkillType type, float spd, float dtn)
      {
         skillType = type;
         speed     = spd;
         duration  = dtn;
      }
   
      void Update()
      {
         if (skillType == ESkillType.AttackShoot)
         {
            if (_elapsedTime < duration)
            {
               float step = speed                  * Time.deltaTime; 
               transform.Translate(Vector3.forward * step);          
               _elapsedTime += Time.deltaTime;                        
            }
         }
      
      }
   }

