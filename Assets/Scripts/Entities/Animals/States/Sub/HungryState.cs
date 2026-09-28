using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class HungryState : BaseState
{
    Need _hungryNeed;
    AnimalBlackboard _animalBlackboard;
    AnimalController _animalController;
    Food _food;

    IEnumerable<Food> KnownFood => _animalBlackboard.GetKnownFood(_animalController.Animal);

    public HungryState(AnimalBlackboard animalBlackboard, AnimalController animalController)
    {
        _animalBlackboard = animalBlackboard;
        _animalController = animalController;
        _hungryNeed = animalBlackboard.Needs.FirstOrDefault(n => n.Type == NeedType.Hunger);
    }

    public override void Update()
    {
        if (_hungryNeed == null)
            return;

        if (_food == null || !_food.isActiveAndEnabled)
        {
            _food = KnownFood
            .FirstOrDefault(food => RaycastUtils.IsInLineOfSight(_animalController.Animal.transform.position, food.transform.position, _animalBlackboard.ObstacleLayer));
        }
        if (_food == null)
        {
            return;
        }
        _animalController.MoveTo(_food.transform.position);
        if (!_animalBlackboard.IsInFrontOfObject(_food.gameObject))
            return;

        var nutritionValue = _food.NutritionValue;
        _food.Consume();
        _hungryNeed.AddToCurrentValue(nutritionValue);
        _food = null;
    }


}
