using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalfPointTrigger : MonoBehaviour
{
    public GameObject LapCompeteTrig;
    public GameObject HalfLapTrig;

    void OnTriggerEnter ()
    {
        LapCompeteTrig.SetActive(true);
        HalfLapTrig.SetActive(false);
    }
}
