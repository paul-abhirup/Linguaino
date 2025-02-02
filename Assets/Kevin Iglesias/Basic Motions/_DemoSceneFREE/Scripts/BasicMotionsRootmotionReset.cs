// This script is optional, only for the demo scene

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KevinIglesias
{
    public class BasicMotionsRootmotionReset : MonoBehaviour
    {
        public float delay = 1f;
        
        Vector3 initPosition;
        
        void Start()
        {
            initPosition = transform.localPosition;
            if(delay > 0)
            {
                InvokeRepeating("ResetPosition", 0f, delay);
            }
        }
        
        public void ResetPosition()
        {
            transform.localPosition = initPosition;
        }
        
        public void ResetPositionFromSMB()
        {
            StartCoroutine(ResetPositionAfterFrame());
        }
        
        private IEnumerator ResetPositionAfterFrame()
        {
            yield return new WaitForEndOfFrame();
            ResetPosition();
        }
    }
}
