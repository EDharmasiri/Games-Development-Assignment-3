using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GhostTweener : MonoBehaviour
{
    //private Tween activeTween;
    private List<Tween> activeTweens = new List<Tween>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
            foreach (Tween tween in activeTweens)
            {
                if (Vector3.Distance(tween.Target.position, tween.EndPos) > 0.1f)
                {
                    float timeFraction = (Time.time - tween.StartTime) / tween.Duration;
                    tween.Target.position = Vector3.Lerp(tween.StartPos, tween.EndPos, timeFraction);
                }
                else if (Vector3.Distance(tween.Target.position, tween.EndPos) <= 0.1f)
                {
                    tween.Target.position = tween.EndPos;
                }
            }

            activeTweens.RemoveAll(tween => tween.Target.position == tween.EndPos);
    }

    public bool AddTween(Transform targetObject, Vector3 startPos, Vector3 endPos, float duration)
    {
        if (TweenExists(targetObject))
            return false;
        activeTweens.Add(new Tween(targetObject, startPos, endPos, Time.time, duration));
        return true;
    }
    public bool TweenExists(Transform target)
    {
        foreach (Tween tween in activeTweens)
        {
            if (tween.Target == target)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveTween(Transform target)
    {
        foreach (Tween tween in activeTweens.ToList())
        {
            if (tween.Target == target)
            {
                activeTweens.Remove(tween);
            }
        }
    }

    public bool compareEndPos(Vector3 wantedEndPos)
    {
        foreach (Tween tween in activeTweens)
        {
            if (tween.EndPos == wantedEndPos)
            {
                return true;
            }
        }
        return false;
    }
}
