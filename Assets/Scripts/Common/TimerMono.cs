using System;
using System.Collections;
using UnityEngine;

namespace Common
{
   public class TimerMono : MonoBehaviour
   {
      public float timeToDestroy = 10f;

      private float _currentTime = 0f;

      private void Start()
      {
         StartCoroutine(StartDestructiveTimer());
      }

      IEnumerator StartDestructiveTimer()
      {
         while (_currentTime <= timeToDestroy)
         {
            _currentTime += Time.deltaTime;
            yield return null;
         }
         Destroy(this.gameObject);
      }
   }
}
