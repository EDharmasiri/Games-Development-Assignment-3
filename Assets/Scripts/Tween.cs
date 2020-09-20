using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Tween
{
    public Transform Target { get; private set; }
    public UnityEngine.Vector3 StartPos { get; private set; }
    public UnityEngine.Vector3 EndPos { get; private set; }
    public float StartTime { get; private set; }
    public float Duration { get; private set; }

    public Tween(Transform targetObject, UnityEngine.Vector3 startPos, UnityEngine.Vector3 endPos, float time, float duration) {
        Target = targetObject;
        StartPos = startPos;
        EndPos = endPos;
        StartTime = time;
        Duration = duration;
    }
}
