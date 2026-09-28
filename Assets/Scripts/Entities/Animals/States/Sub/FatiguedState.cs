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
    }

    public override void Update()
    {
        if (_fatiguedNeed == null)
            return;

        if (_restArea == null || !_restArea.isActiveAndEnabled)
        {
            _restArea = _animalBlackboard.GetKnownRestAreas()
                .Where(area => area != null)
                .OrderBy(area => Vector3.Distance(_animalController.Animal.transform.position, area.transform.position))
                .FirstOrDefault();
        }

        if (_restArea == null)
        {
            return;
        }

        _animalController.MoveTo(_restArea.transform.position);
        if (!_animalController.HasReachedDestination)
            return;
        _animalController.Animal.Stop();
        _restArea.Rest(_fatiguedNeed, Time.deltaTime);
    }
}
