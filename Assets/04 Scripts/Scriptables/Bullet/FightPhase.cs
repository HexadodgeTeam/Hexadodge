using System;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "Phase", menuName = "Scriptable Objects/Fight/Phase")]
public class FightPhase : ScriptableObject, IGizmosDrawer
{
    [Tooltip("La période utilisée pour les curves de la phase. Doit être supérieure à 0.\n" +
             "Utiliser une courbe ici n'a aucun effet.")]
    [SerializeField] internal FloatCurve period = 20f;
    
    [Tooltip("Les séquences qui se succèdent jusqu'à la prochaine phase.")]
    [SerializeField] internal PatternSequence[] sequences;

    [Tooltip("Si les séquences s'enchaînent de manière aléatoire.")]
    [SerializeField] internal bool randomOrder = true;
    
    [Tooltip("Supposant que Random Order = true, si une même séquence peut s'enchaîner plus d'une fois d'affilée.")]
    [SerializeField] internal bool randomSequenceCanRepeat = false;

    [Tooltip("Durée d'attente entre 2 séquences.\n" +
             "Utiliser une courbe ici n'a aucun effet.")]
    [SerializeField] internal FloatCurve waitBetweenSequence = 0.5f;

    [Tooltip("Couleur du joueur lors de la phase.")]
    [SerializeField] internal Color color = Color.white;
    
    [Tooltip("Dégradé de la peinture lors de la phase.")]
    [SerializeField] internal Gradient gradient;

    public FightPhaseInstance Instance(Action<BulletSpawn> spawnAction) => new(this, spawnAction);

    public void OnDrawGizmos()
    {
        foreach (PatternSequence sequence in sequences)
        {
            sequence?.OnDrawGizmos();
        }
    }
}

public class FightPhaseInstance
{
    private readonly FightPhase _data;
    private readonly PatternSequenceInstance[] _sequenceInstances;
    private FloatCurveInstance _periodInstance;
    private FloatCurveInstance _waitBetweenSequenceInstance;

    private float _phaseStart = 0f;
    private float _phaseTime;
    private int _currentSequenceIndex;
    private float _waitTimer = 0f;

    public FightPhaseInstance(FightPhase data, Action<BulletSpawn> spawnAction)
    {
        _data = data;
        _sequenceInstances = new PatternSequenceInstance[_data.sequences.Length];
        for (int i = 0; i < _data.sequences.Length; i++)
        {
            _sequenceInstances[i] = _data.sequences[i].Instance(spawnAction);
        }

        _periodInstance = _data.period.Instance();
        _waitBetweenSequenceInstance = _data.waitBetweenSequence.Instance();
        _currentSequenceIndex = _data.randomOrder ? Random.Range(0, _data.sequences.Length) : 0;
    }

    public void Start()
    {
        Player.UpdateColor(_data.color);
        FillGrid.Instance.SetGradient(_data.gradient);
        UIManager.FillUI.SetColor(_data.color);
    }
    
    public void Tick()
    {
        if (_waitTimer > 0f)
        {
            _waitTimer -= Time.deltaTime;
        }
        else
        {
            _phaseTime = Time.time - _phaseStart;
            float time01 = Mathf.Clamp01(_phaseTime / _periodInstance);
        
            PatternSequenceInstance currentSequence = _sequenceInstances[_currentSequenceIndex];
        
            currentSequence.Tick(time01);
            if (currentSequence.Ended(time01))
            {
                _waitTimer = _waitBetweenSequenceInstance.Eval(time01);
                currentSequence.ReRoll();
                GetNextSequenceIndex();
                _phaseStart = 0f;
            }
        }
    }

    private void GetNextSequenceIndex()
    {
        if (!_data.randomOrder)
        {
            _currentSequenceIndex = (_currentSequenceIndex + 1) % _data.sequences.Length;
        }
        else
        {
            if (!_data.randomSequenceCanRepeat)
            {
                if (_data.sequences.Length <= 1) return;
                int random = Random.Range(0, _data.sequences.Length - 1);
                if (random == _currentSequenceIndex) random++;
                _currentSequenceIndex = random;
            }
            else
            {
                _currentSequenceIndex = Random.Range(0, _data.sequences.Length);
            }
        }
    }

    public void ReRoll()
    {
        foreach (PatternSequenceInstance sequence in _sequenceInstances)
        {
            sequence.ReRoll();
        }
        _periodInstance.ReRoll();
        _waitBetweenSequenceInstance.ReRoll();
    }
}
