using System;
using System.Linq;

public class HungryState : BaseState
{
    private readonly Need _need;
    private readonly AnimalBlackboard _blackboard;
    private readonly AnimalController _controller;
    private Food _food;
    public event Action FoodUnavailable;

    public HungryState(AnimalBlackboard blackboard, AnimalController controller)
    {
        _blackboard = blackboard;
        _controller = controller;
        _need = blackboard.Needs.FirstOrDefault(need => need.Type == NeedType.Hunger);
        OnUpdate += TrySearchFood;
        OnExit += () => { _food = null; controller.StopMovement(); };
    }

    private void TrySearchFood()
    {
        if (_need == null || _need.IsSatisfied) return;
        if (_food == null || !_food.isActiveAndEnabled)
            _food = _blackboard.GetKnownFood(_controller.Animal)
                .Where(food => RaycastUtils.IsInLineOfSight(_controller.transform.position,
                    food.transform.position, _blackboard.ObstacleLayer))
                .OrderBy(food => (food.transform.position - _controller.transform.position).sqrMagnitude)
                .FirstOrDefault();
        if (_food == null)
        {
            FoodUnavailable?.Invoke();
            return;
        }
        if (!_blackboard.CanInteractWith(_food))
        {
            _controller.MoveTo(_food.transform.position);
            return;
        }
        _controller.StopMovement();
        int nutrition = _food.NutritionValue;
        if (_food.TryConsume()) _need.AddToCurrentValue(nutrition);
        _food = null;
    }
}
