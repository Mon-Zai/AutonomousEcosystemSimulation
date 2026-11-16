using System.Linq;
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
        OnUpdate += TrySearchWaterSource;
        OnExit += () => { _waterSource = null; _animalController.StopMovement(); };
    }

    private void TrySearchWaterSource()
    {
        if (_thirstyNeed == null || _thirstyNeed.IsSatisfied)
            return;

        if (_waterSource == null || !_waterSource.isActiveAndEnabled)
        {
            _waterSource = _animalBlackboard.GetKnownWaterSources()
                .Where(source => source != null && source.isActiveAndEnabled)
                .OrderBy(source => Vector3.Distance(_animalController.Animal.transform.position, source.transform.position))
                .FirstOrDefault();
        }

        if (_waterSource == null)
        {
            return;
        }

        if (!_animalBlackboard.CanInteractWith(_waterSource))
        {
            _animalController.MoveTo(_waterSource.transform.position);
            return;
        }
        _animalController.StopMovement();
        _waterSource.Drink(_thirstyNeed);
        _waterSource = null;
    }

    
}
