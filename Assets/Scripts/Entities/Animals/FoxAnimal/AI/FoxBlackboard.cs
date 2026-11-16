using System.Linq;
using UnityEngine;

public class FoxBlackboard : AnimalBlackboard
{
    public Food FoodTarget { get; private set; }
    public Prey PreyTarget { get; private set; }
    public Need Hunger => _needs.FirstOrDefault(need => need.Type == NeedType.Hunger);
    public bool IsHungry => Hunger != null && !Hunger.IsSatisfied;
    public void SelectTargets(Fox fox, AnimalPerception perception)
    {
        FoodTarget = GetKnownFood(fox)
            .Where(food => RaycastUtils.IsInLineOfSight(transform.position, food.transform.position, _obstacleLayer))
            .OrderBy(food => (food.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();
        if (FoodTarget != null) { PreyTarget = null; return; }
        var best = perception.Results
            .Where(member => member != null && member.gameObject != gameObject)
            .Select(member => member.GetComponent<Prey>())
            .Where(prey => prey != null && prey.IsAvailable && fox.FoodPreferences.Contains(prey.FoodType))
            .Where(prey => RaycastUtils.IsInLineOfSight(transform.position, prey.transform.position, _obstacleLayer))
            .Select(prey => new
            {
                Prey = prey,
                Distance = Vector3.Distance(transform.position, prey.transform.position),
                Attacks = Mathf.Ceil(prey.Health.CurrentHealth / Mathf.Max(1f, fox.AttackDamage)),
                Nutrition = Mathf.Min(prey.NutritionValue, Mathf.Max(1f, Hunger.MaxValue - Hunger.CurrentValue))
            })
            .Aggregate((Target: (Prey)null, Score: float.NegativeInfinity), (winner, candidate) =>
            {
                float score = candidate.Nutrition / (1f + candidate.Distance / Mathf.Max(0.1f, fox.Settings.MaxSpeed)
                    + candidate.Attacks * fox.AttackInterval);
                return score > winner.Score ? (candidate.Prey, score) : winner;
            });
        PreyTarget = best.Target;
    }
}
