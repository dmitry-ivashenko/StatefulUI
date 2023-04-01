using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace StatefulUI.Runtime.States
{
    public class StateAnimationInfo
    {
        public bool IsFinished { get; set; }
        public AnimationClip Clip { get; }
        public GameObject Target { get; }
        public float StartTime { get; private set; }

        private readonly float _duration;
        private int _nextEventIndex;

        public StateAnimationInfo(AnimationClip clip, GameObject target, bool randomStartTime = false)
        {
            Clip = clip;
            Target = target;
            _duration = Clip.length;
            StartTime = Time.realtimeSinceStartup + (randomStartTime ? Random.Range(0, clip.length) : 0);
            _nextEventIndex = 0;
        }

        public void OnUpdate()
        {
            var time = Time.realtimeSinceStartup - StartTime;
            
            if (IsFinished)
            {
                time = _duration;
            }
            
            Clip.SampleAnimation(Target, time);

            if (Application.isPlaying && _nextEventIndex < Clip.events.Length && time >= Clip.events[_nextEventIndex].time)
            {
                var animEvent = Clip.events[_nextEventIndex];

                try
                {
                    if (animEvent.objectReferenceParameter != null)
                    {
                        Target.SendMessage(animEvent.functionName, animEvent.objectReferenceParameter);
                    }
                    else if (animEvent.stringParameter != null)
                    {
                        Target.SendMessage(animEvent.functionName, animEvent.stringParameter);
                    }
                    else
                    {
                        Target.SendMessage(animEvent.functionName, animEvent.intParameter);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }

                _nextEventIndex++;
            }
                
            if (time >= _duration)
            {
                if (Clip.isLooping)
                {
                    StartTime = Time.realtimeSinceStartup;
                    _nextEventIndex = 0;
                }
                else
                {
                    IsFinished = true;
                }
            }
        }
    }
}
