using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class classA : MonoBehaviour
{
    public static classA instance;
    private void Awake()
    {
        instance = this;
    }
    
    public void dosth()
    {
        Debug.Log("class a call b");
    }
}
