using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Sequence", menuName = "Scriptable Objects/Bullets/Sequence")]
public class PatternSequence : ScriptableObject, IGizmosDrawer
{
    [Tooltip("La durée de la séquence.\n" +
             "Utiliser une courbe ici n'a aucun effet.")]
    [SerializeField] internal FloatCurve duration;
    
    [Tooltip("Les timelines de pattern. Chaque pattern peut être défini avec un début et une fin.")]
    [SerializeField] internal PatternTimeline[] timelines;

    public PatternSequenceInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);

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
    private readonly PatternTimelineInstance[] _timelineInstances;

    public PatternSequenceInstance(PatternSequence data, Action<BulletSpawn> spawnAction)
    {
        _durationInstance = data.duration.Instance();
        _sequenceStart = Time.time;
        _sequenceTime = 0f;
        _timelineInstances = new PatternTimelineInstance[data.timelines.Length];
        
        for (int i = 0; i < data.timelines.Length; i++)
        {
            _timelineInstances[i] = data.timelines[i].Instance(spawnAction);
        }
    }

    public void Tick()
    {
        _sequenceTime = Time.time - _sequenceStart;
        
        foreach (PatternTimelineInstance timeline in _timelineInstances)
        {
            timeline.Tick(_sequenceTime);
        }
    }

    public bool Ended()
    {
        return _sequenceTime >= _durationInstance;
    }

    public void ReRoll()
    {
        _durationInstance.ReRoll();
        _sequenceStart = Time.time;
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
             "Utiliser une courbe ici n'a aucun effet.")]
    public FloatCurve start;
    
    [Tooltip("La fin du pattern selon le début de la séquence.\n" +
             "Utiliser une courbe ici n'a aucun effet.")]
    public FloatCurve end;

    public PatternTimelineInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);
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

    public void Tick(float sequenceTime)
    {
        if (sequenceTime < _startInstance || sequenceTime > _endInstance) return;
        
        float time01 = Mathf.InverseLerp(_startInstance, _endInstance, sequenceTime);
        
        _patternInstance.Tick(time01);
    }

    public void ReRoll()
    {
        _patternInstance.ReRoll();
        _startInstance.ReRoll();
        _endInstance.ReRoll();
    }
}