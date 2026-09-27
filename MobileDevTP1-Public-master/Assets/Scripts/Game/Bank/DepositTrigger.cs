using System;
using UnityEngine;

public class DepositTrigger : MonoBehaviour
{
    private bool IsEmpty = false;
    
    private void OnTriggerEnter(Collider other)
    {
        if (IsEmpty)
        {
            IsEmpty = true;
            
        }
        else
        {
            
        }
    }
}
