//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Device;
//using UnityEngine.UI;

//public class OffscreenIndicator : MonoBehaviour
//{
//    #region EXTERNAL DATA
//    [Tooltip("Alpha value when player is just offscreen")]
//    [SerializeField][Range(0, 1)] private float _alphaAtMinDist;
//    [Tooltip("Alpha value when player is far offscreen")]
//    [SerializeField][Range(0, 1)] private float _alphaAtMaxDist;
//    [Tooltip("Max distance offscreen to reach max alpha value")]
//    [SerializeField] private float _alphaChangeDistMax = 10f;
//    #endregion

//    #region INTERNAL DATA
//    // UI Data
//    private RectTransform _rect;
//    private Image _image;

//    // Offscreen Target Data
//    private float _offsetX;
//    private float _offsetY;
//    #endregion

//    private void Awake()
//    {
//        // Get Components
//        _rect = GetComponent<RectTransform>();
//        _image = GetComponent<Image>();

//        // Set Data
//        _offsetX = _rect.rect.width / 2f;
//        _offsetY = _rect.rect.height / 2f;
//    }

//    public void UpdatePos(PlayerBase player, Camera cam, RectTransform screen)
//    {
//        Vector3 indicatorPos = cam.WorldToScreenPoint(player.transform.position);

//        // If player is onscreen
//        //if (indicatorPos.x > 0f + _offsetX && indicatorPos.x < screen.rect.width - _offsetX &&
//        //    indicatorPos.y > 0f + _offsetY && indicatorPos.y < screen.rect.height - _offsetY)
//        //{
//        //    return false;
//        //}
//        //else
//        //{ 
//        if (indicatorPos.x < 0f)
//        {
//            indicatorPos.x = 200f;
//        }
//        else if (indicatorPos.x > screen.rect.width)
//        {
//            indicatorPos.x = screen.rect.width - 200f;
//        }

//        if (indicatorPos.y < 0f - _offsetY)
//        {
//            indicatorPos.y = _offsetY;
//        }
//        else if (indicatorPos.y > screen.rect.height + _offsetY)
//        {
//            indicatorPos.y = screen.rect.height - _offsetY;
//        }

//        indicatorPos.z = 0;
//        _rect.position = indicatorPos;

//        //    return true;
//        //}
//    }
//}
