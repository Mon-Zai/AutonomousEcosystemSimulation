using System.Linq;
using UnityEngine;

public class FatiguedState : BaseState
{
    Need _fatiguedNeed;
    AnimalBlackboard _animalBlackboard;
    AnimalController _animalController;
    RestArea _restArea;
    public FatiguedState(AnimalBlackboard animalBlackboard, AnimalController animalController)
    {
        _animalBlackboard = animalBlackboard;
        _animalController = animalController;
        _fatiguedNeed = animalBlackboard.Needs.FirstOrDefault(n => n.Type == NeedType.Fatigue);
        OnUpdate += TrySearchRestArea;
        OnExit += () => { _restArea = null; _animalController.StopMovement(); };

    }

    private void TrySearchRestArea()
    {
        if (_fatiguedNeed == null || _fatiguedNeed.IsSatisfied)
            return;

        if (_restArea == null || !_restArea.isActiveAndEnabled)
        {
            _restArea = _animalBlackboard.GetKnownRestAreas()
                .Where(area => area != null && area.isActiveAndEnabled)
                .OrderBy(area => Vector3.Distance(_animalController.Animal.transform.position, area.transform.position))
                .FirstOrDefault();
        }

        if (_restArea == null)
        {
            return;
        }

        if (!_animalBlackboard.CanInteractWith(_restArea))
        {
            _animalController.MoveTo(_restArea.transform.position);
            return;
        }
        _animalController.StopMovement();
        _restArea.Rest(_fatiguedNeed, Time.deltaTime);
    }
}
