using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamTrigger : MonoBehaviour
{
    #region EXTERNAL DATA
    [Tooltip("Camera to trigger on/off")]
    [SerializeField] CinemachineVirtualCamera _cam;
    #endregion
}
