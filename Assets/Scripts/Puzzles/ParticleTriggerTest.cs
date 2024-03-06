using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleTriggerTest : MonoBehaviour
{

    [field: SerializeField] private int _sentCode = 0;
    // Start is called before the first frame update
    private List<ParticleSystem.Particle> _insideColliders = new List<ParticleSystem.Particle>();
    private List<ParticleSystem.Particle> _insidePlate = new List<ParticleSystem.Particle>();
    //private List<ParticleSystem.Particle> _insidePlayer = new List<ParticleSystem.Particle>();

    private bool _activated = false;

    public void OnParticleTrigger()
    {
        // get ParticleSystem component
        ParticleSystem ps = GetComponent<ParticleSystem>();

        // particles
        _insideColliders.Clear();
        _insidePlate.Clear();
        //_insidePlayer.Clear();

        // get all particles
        int numInside = ps.GetTriggerParticles(ParticleSystemTriggerEventType.Inside, _insideColliders, out var insideData);

        // iterate over all the triggered particles
        for (int i = 0; i < numInside; i++)
        {
            ParticleSystem.Particle p = _insideColliders[i];
            if (insideData.GetColliderCount(i) == 1)
            {
                // add particles to lists depending on what collider they interacted with
                if (insideData.GetCollider(i, 0) == ps.trigger.GetCollider(0))
                {
                    _insidePlate.Add(p);
                }
                else
                {
                    //_insidePlayer.Add(p);
                    p.startColor = new Color32(0, 0, 255, 255); //set particle to be blue
                }
            }
            else
            {
                p.startColor = new Color32(0, 0, 0, 255); // just for testing - should never get here - should never see black particles
            }
            _insideColliders[i] = p;
        }

        // set modified particles back to the particle System
        ps.SetTriggerParticles(ParticleSystemTriggerEventType.Inside, _insideColliders);
        
        CheckPlateParticle(_insidePlate.Count);
    }

    // check the number of particles over the plate and do stuff accordingly
    private void CheckPlateParticle(int numInside)
    {
    if (numInside < 20 && !_activated) //if under 20 particles over plate
    {
        // activate plate
        _activated = true;
        EventManager.EventTrigger(EventType.ASSIGNMENT_CODE_TRIGGER, _sentCode);
    }
    else if (numInside >= 20 && _activated) //if more than 20 particles over plate
    {
        // deactivate plate
        _activated = false;
        EventManager.EventTrigger(EventType.ASSIGNMENT_CODE_TRIGGER, _sentCode);
    }
    }
}
