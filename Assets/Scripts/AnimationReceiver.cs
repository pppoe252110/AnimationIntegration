using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimationReceiver : MonoBehaviour
{
    [SerializeField]private UnityEvent onHit;
    [SerializeField]private UnityEvent onEndFinishing;

     
    public void Hit()
    {
        onHit.Invoke();
    }
    
    public void EndFinishing()
    {
        onEndFinishing.Invoke();
    }
}
