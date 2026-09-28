using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class ThirstyState : BaseState
{
    Need _thirstyNeed;
    AnimalBlackboard _animalBlackboard;
    AnimalController _animalController;
    WaterSource _waterSource;
    public ThirstyState(AnimalBlackboard animalBlackboard, AnimalController animalController)
    {
        _animalBlackboard = animalBlackboard;
        _animalController = animalController;
        _thirstyNeed = animalBlackboard.Needs.FirstOrDefault(n => n.Type == NeedType.Thirst);
    }

    public override void Update()
    {
        if (_thirstyNeed == null)
            return;

        if (_waterSource == null || !_waterSource.isActiveAndEnabled)
        {
            _waterSource = _animalBlackboard.GetKnownWaterSources()
                .Where(source => source != null)
                .OrderBy(source => Vector3.Distance(_animalController.Animal.transform.position, source.transform.position))
                .FirstOrDefault();
        }

        if (_waterSource == null)
        {
            return;
        }

        _animalController.MoveTo(_waterSource.transform.position);
        if (!_animalBlackboard.IsInFrontOfObject(_waterSource.gameObject))
            return;

        _waterSource.Drink(_thirstyNeed);
        _waterSource = null;
    }

    
}
