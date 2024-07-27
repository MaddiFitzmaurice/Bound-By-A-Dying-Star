using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyBob : MonoBehaviour
{
    [SerializeField] private float _bobTime;
    [SerializeField] private float _bobDisplacement = 1f;
    [SerializeField] private LeanTweenType _easeInType;
    [SerializeField] private LeanTweenType _easeOutType;

    // Update is called once per frame
    void Start()
    {
        Vector3 moveTo = new Vector3(this.transform.position.x, this.transform.position.y + _bobDisplacement, this.transform.position.z);
        LeanTween.move(this.gameObject, moveTo, _bobTime).setEase(_easeInType).setLoopPingPong().setEase(_easeOutType);
    }
}
