using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Sequence", menuName = "Scriptable Objects/Bullets/Sequence")]
public class PatternSequence : ScriptableObject, IGizmosDrawer
{
    [Tooltip("La durée de la séquence.\n" +
             "Dépend de la période de la phase.")]
    [SerializeField] internal FloatCurve duration;
    
    [Tooltip("Les timelines de pattern. Chaque pattern peut être défini avec un début et une fin.")]
    [SerializeField] internal PatternTimeline[] timelines;
    
    [Tooltip("Les triggers d'animation présentes dans la séquence.")]
    [SerializeField] internal AnimTriggerTimeline[] animTriggers;

    public PatternSequenceInstance Instance(Action<int> animTriggerAction, Action<BulletSpawn> spawnAction) => new(this, animTriggerAction, spawnAction);

    public void OnDrawGizmos()
    {
        foreach (PatternTimeline timeline in timelines)
        {
            timeline.pattern?.OnDrawGizmos();
        }
    }
}

public class PatternSequenceInstance
{
    private FloatCurveInstance _durationInstance;
    private float _sequenceStart;
    private float _sequenceTime;
    private float _lastSequenceTime;
    private readonly PatternTimelineInstance[] _timelineInstances;
    private readonly AnimTriggerTimeline[] _animTriggers;
    private readonly int[] _animTriggersHash;
    private readonly Action<int> _animTriggerAction;

    public PatternSequenceInstance(PatternSequence data, Action<int> animTriggerAction, Action<BulletSpawn> spawnAction)
    {
        _durationInstance = data.duration.Instance();
        _sequenceStart = Game.ScaledTime;
        _sequenceTime = 0f;
        _lastSequenceTime = 0f;
        _timelineInstances = new PatternTimelineInstance[data.timelines.Length];
        _animTriggers = data.animTriggers;
        _animTriggerAction = animTriggerAction;
        
        for (int i = 0; i < data.timelines.Length; i++)
        {
            _timelineInstances[i] = data.timelines[i].Instance(spawnAction);
        }

        if (data.animTriggers != null)
        {
            _animTriggersHash = new int[data.animTriggers.Length];

            for (int i = 0; i < data.animTriggers.Length; i++)
            {
                _animTriggersHash[i] = Animator.StringToHash(data.animTriggers[i].animationTrigger);
            }
        }
        else
        {
            _animTriggers = Array.Empty<AnimTriggerTimeline>();
        }
    }

    public void Start()
    {
        _sequenceStart = Game.ScaledTime;
        _sequenceTime = 0f;
    }

    public void Tick(float phaseTime01)
    {
        _sequenceTime = Game.ScaledTime - _sequenceStart;
        
        foreach (PatternTimelineInstance timeline in _timelineInstances)
        {
            timeline.Tick(phaseTime01, _sequenceTime);
        }

        for (var i = 0; i < _animTriggers.Length; i++)
        {
            if (_lastSequenceTime <= _animTriggers[i].triggerTime && _animTriggers[i].triggerTime <= _sequenceTime)
            {
                _animTriggerAction.Invoke(_animTriggersHash[i]);
            }
        }

        _lastSequenceTime = _sequenceTime;
    }

    public bool Ended(float phaseTime01)
    {
        return _sequenceTime >= _durationInstance.Eval(phaseTime01);
    }

    public void ReRoll()
    {
        _durationInstance.ReRoll();
        _sequenceStart = Game.ScaledTime;
        _sequenceTime = 0f;
        foreach (PatternTimelineInstance timeline in _timelineInstances)
        {
            timeline.ReRoll();
        }
    }
}

[Serializable]
public class PatternTimeline
{
    [Tooltip("Le pattern.")]
    public BulletPattern pattern;

    [Tooltip("Le début du pattern selon le début de la séquence.\n" +
             "Dépend de la période de la phase.")]
    public FloatCurve start;
    
    [Tooltip("La fin du pattern selon le début de la séquence.\n" +
             "Dépend de la période de la phase.")]
    public FloatCurve end;

    public PatternTimelineInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);
}

[Serializable]
public class AnimTriggerTimeline
{
    [Tooltip("Le nom du trigger.")]
    public string animationTrigger;

    [Tooltip("Le temps où le trigger se déclenche selon le début de la séquence.")]
    public float triggerTime;
}

public class PatternTimelineInstance
{
    private readonly BulletPatternInstance _patternInstance;
    private FloatCurveInstance _startInstance;
    private FloatCurveInstance _endInstance;

    public PatternTimelineInstance(PatternTimeline data, Action<BulletSpawn> spawnAction)
    {
        _patternInstance = data.pattern.Instance(spawnAction);
        _startInstance = data.start.Instance();
        _endInstance = data.end.Instance();
    }

    public void Tick(float phaseTime01, float sequenceTime)
    {
        if (sequenceTime < _startInstance.Eval(phaseTime01) || sequenceTime > _endInstance.Eval(phaseTime01)) return;
        
        float time01 = Mathf.InverseLerp(_startInstance.Eval(phaseTime01), _endInstance.Eval(phaseTime01), sequenceTime);
        
        _patternInstance.Tick(time01);
    }

    public void ReRoll()
    {
        _patternInstance.ReRoll();
        _startInstance.ReRoll();
        _endInstance.ReRoll();
    }
}