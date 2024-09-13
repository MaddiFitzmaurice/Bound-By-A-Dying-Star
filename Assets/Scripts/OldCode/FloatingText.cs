using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    // Start is called before the first frame update

    public float DestroyTime = 3f;
    void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}
