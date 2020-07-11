using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Hazard", menuName = "Hazard")]
public class HazardSpawnData : ScriptableObject
{
    public GameObject hazardPrefab;
    [Range(0, 10)]
    public float commonality = 10;//10 = common, 1 = rare, 0 = extinct
    public float timeFloor = 0;//(sec) the game must last for this long in order for this object to be in the pool
}
