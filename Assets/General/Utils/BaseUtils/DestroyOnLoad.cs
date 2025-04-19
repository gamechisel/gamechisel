using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnLoad : MonoBehaviour
{
    void Awake()
    {
        Destroy(this.gameObject);
    }
}
