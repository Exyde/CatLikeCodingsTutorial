using UnityEngine;

public class Graph : MonoBehaviour
{
	[SerializeField]
	Transform pointPrefab = default;
	Transform[] points;
	float duration;
	bool transitioning;

	FunctionLibrary.FunctionName transitionFunction;

	[SerializeField]
	[Range(10, 100)] int resolution  = 10;

	[SerializeField]
	FunctionLibrary.FunctionName function = default;

	public enum TransitionMode { Cycle, Random }

	[SerializeField]
	TransitionMode transitionMode;

	//Auto switch
	[SerializeField, Min(0f)]
	float functionDuration = 1f, transitionDuration = 1f;

	private void Awake()
	{
		Transform _transform = transform;
		points = new Transform[resolution * resolution];

		float step = 2f / resolution;
		var scale = Vector3.one * step;

		for (int i = 0; i < points.Length; i++)
		{
			Transform point = Instantiate(pointPrefab);
			point.localScale = scale;
			point.SetParent(transform, false);
			points[i] = point;
		}
	}

	void Update()
	{
		duration += Time.deltaTime;

		if (transitioning) {
			if (duration >= transitionDuration) {
				duration -= transitionDuration;
				transitioning = false;
			}
		}

		else if (duration >= functionDuration) {
			duration = 0f;
			transitioning = true;
			transitionFunction = function;
			PickNextFunction();
		}

		if (transitioning) {
			UpdateFunctionTransition();
		}
		else {
			UpdateFunction();
		}	}

	private void UpdateFunction()
	{
		FunctionLibrary.Function f = FunctionLibrary.GetFunction(function);

		float time = Time.time;
		float step = 2f / resolution;
		float v = .5f * step - 1;


		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
		{
			if (x == resolution)
			{
				x = 0;
				z++;
				v = (z + .5f) * step - 1;
			}

			float u = (x + .5f) * step - 1;

			points[i].localPosition = f(u, v, time);

		}
	}

		private void UpdateFunctionTransition ()
	{
		FunctionLibrary.Function
			from = FunctionLibrary.GetFunction(transitionFunction),
			to = FunctionLibrary.GetFunction(function);
		float progress = duration / transitionDuration;
		float time = Time.time;
		float step = 2f / resolution;
		float v = 0.5f * step - 1f;

		for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
		{
			if (x == resolution)
			{
				x = 0;
				z++;
				v = (z + .5f) * step - 1;
			}

			float u = (x + .5f) * step - 1;

			points[i].localPosition = FunctionLibrary.Morph(
				u, v, time, from, to, progress
			);

		}
	}

	void PickNextFunction () {
		function = transitionMode == TransitionMode.Cycle ?
			FunctionLibrary.GetNextFunctionName(function) :
			FunctionLibrary.GetRandomFunctionNameOtherThan(function);
	}
}

 