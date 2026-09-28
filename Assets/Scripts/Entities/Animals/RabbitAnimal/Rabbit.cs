using UnityEngine;

public class Rabbit : Animal
{
	[SerializeField] private GameObject _droppingsPrefab;
	[SerializeField] private Vector2 _droppingsInterval = new Vector2(8f, 18f);

	private float _nextDroppingsTime;
	private bool _hasScheduledDroppings;

	public override void PerformWanderActivity()
	{
		if (_droppingsPrefab == null)
			return;

		if (!_hasScheduledDroppings)
		{
			ScheduleNextDroppings();
			return;
		}

		if (Time.time < _nextDroppingsTime)
			return;

		Vector3 spawnPosition = transform.position - transform.forward * 0.35f;
		Instantiate(_droppingsPrefab, spawnPosition, Quaternion.Euler(0f, Random.Range(0f, 360f), 0f));
		ScheduleNextDroppings();
	}

	private void ScheduleNextDroppings()
	{
		float minimumInterval = Mathf.Max(1f, _droppingsInterval.x);
		float maximumInterval = Mathf.Max(minimumInterval, _droppingsInterval.y);
		_nextDroppingsTime = Time.time + Random.Range(minimumInterval, maximumInterval);
		_hasScheduledDroppings = true;
	}
}