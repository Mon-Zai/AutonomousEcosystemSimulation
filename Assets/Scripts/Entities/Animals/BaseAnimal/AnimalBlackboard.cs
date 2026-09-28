using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.MemoryProfiler;
using UnityEngine;

public abstract class AnimalBlackboard : MonoBehaviour
{
    [SerializeField] protected float _detectionDistance = 1f;
    [SerializeField] protected LayerMask _obstacleLayer;
    [SerializeField] protected GameObject _needContainer;
    protected Need[] _needs;
    private readonly List<Food> _knownFoods = new List<Food>();
    private readonly List<WaterSource> _knownWaterSources = new List<WaterSource>();
    private readonly List<RestArea> _knownRestAreas = new List<RestArea>();
    public Need[] Needs => _needs;
    public LayerMask ObstacleLayer => _obstacleLayer;

    public bool HasNeeds => _needs != null && _needs.Length > 0;
    public bool NeedsSatisfied => _needs != null && !_needs.Any(need => !need.IsSatisfied);

    protected virtual void Awake()
    {
        _needs = _needContainer != null
            ? _needContainer.GetComponents<Need>()
            : GetComponents<Need>();
    }

    public void Remember(Food food)
    {
        if (food != null && !_knownFoods.Contains(food))
            _knownFoods.Add(food);
    }

    public void Remember(WaterSource waterSource)
    {
        if (waterSource != null && !_knownWaterSources.Contains(waterSource))
            _knownWaterSources.Add(waterSource);
    }

    public void Remember(RestArea restArea)
    {
        if (restArea != null && !_knownRestAreas.Contains(restArea))
            _knownRestAreas.Add(restArea);
    }

    public void ForgetUnavailableResources()
    {
        _knownFoods.RemoveAll(food => food == null || !food.isActiveAndEnabled);
        _knownWaterSources.RemoveAll(waterSource => waterSource == null || !waterSource.isActiveAndEnabled);
        _knownRestAreas.RemoveAll(restArea => restArea == null || !restArea.isActiveAndEnabled);
    }

    public IEnumerable<Food> GetKnownFood(Animal animal)
    {
        foreach (var food in _knownFoods)
        {
            if (food == null || !food.isActiveAndEnabled)
                continue;

            var preferences = animal.FoodPreferences;
            if (preferences != null && preferences.Count > 0 && !preferences.Contains(food.GetFoodType()))
                continue;

            yield return food;
        }
    }

    public IEnumerable<WaterSource> GetKnownWaterSources() => _knownWaterSources;

    public IEnumerable<RestArea> GetKnownRestAreas() => _knownRestAreas;

    public bool HasKnownResource(NeedType needType, Animal animal)
    {
        return needType switch
        {
            NeedType.Hunger => GetKnownFood(animal).Any(food =>
                RaycastUtils.IsInLineOfSight(animal.transform.position, food.transform.position, _obstacleLayer)),
            NeedType.Thirst => GetKnownWaterSources().Any(waterSource => waterSource != null && waterSource.isActiveAndEnabled),
            NeedType.Fatigue => GetKnownRestAreas().Any(restArea => restArea != null && restArea.isActiveAndEnabled),
            _ => false
        };
    }

    public bool IsInFrontOfObject(GameObject objectInFront)
    {
        RaycastHit hit;
        Vector3 direction = (objectInFront.transform.position - transform.position).normalized;
        LayerMask layerMask = 1 << objectInFront.layer;
        if (Physics.Raycast(transform.position, direction, out hit, _detectionDistance, layerMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Hit object: " + hit.collider.gameObject.name);
            return hit.collider.gameObject == objectInFront;
        }

        return false;
    }
}
