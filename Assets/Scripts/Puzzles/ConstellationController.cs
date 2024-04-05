using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConstellationController : MonoBehaviour
{
    [SerializeField] private PedestalConstellation[] _pedestalArray;
    [SerializeField] private List<PedestalNode> _pedestalNodeList;
    private bool[] _mirroredPedestals;
    private bool[] _activatedNodes;
    private int _pedestalNum;
    // Awake is called before the first frame update
    void Awake()
    {
        _pedestalNum = _pedestalArray.Length;
    }

    void Start()
    {
        _mirroredPedestals = new bool[_pedestalNum];
        _activatedNodes = new bool[_pedestalNodeList.Count];
        for (int i = 0; i < _pedestalNum; i++)
        {
            _mirroredPedestals[i] = false;
        }
    }

    public void PedestalHasMirror(PedestalConstellation sender)
    {
        int senderIndex = Array.IndexOf(_pedestalArray, sender);
        _mirroredPedestals[senderIndex] = true;
        PedestalChecker(senderIndex);
    }

    private void PedestalChecker(int senderIndex)
    {
        for (int i = 0; i < _pedestalNodeList.Count; i++)
        {
            PedestalNode node = _pedestalNodeList[i];
            if (node._pedestalA == _pedestalArray[senderIndex])
            {
                int otherIndex = Array.IndexOf(_pedestalArray, node._pedestalB);
                if (_mirroredPedestals[otherIndex] == true)
                {
                    node._pedestalA.ActivateEffect(node._pedestalB);
                    _activatedNodes[i] = true;
                    ConstellationChecker();
                }
            }
            else if(node._pedestalB == _pedestalArray[senderIndex])
            {
                 int otherIndex = Array.IndexOf(_pedestalArray, node._pedestalA);
                if (_mirroredPedestals[otherIndex] == true)
                {
                    node._pedestalB.ActivateEffect(node._pedestalA);
                    _activatedNodes[i] = true;
                    ConstellationChecker();
                }
            }
        }
    }

    private void ConstellationChecker()
    {
        bool done = true;

        foreach (var check in _activatedNodes)
        {
            if(check != true)
            {
                done = false;
                return;
            }
        }

        if (done)
        {
            EventManager.EventTrigger(EventType.PUZZLE_DONE, 1);
        }
    }
}

[Serializable]
public class PedestalNode
{
    public PedestalConstellation _pedestalA;
    public PedestalConstellation _pedestalB;
}
