using UnityEngine;

public class Graph : MonoBehaviour
{
	[SerializeField]
	Transform pointPrefab = default;
	private Transform[] points;

	[SerializeField]
	[Range(10, 100)] int resolution  = 10;

	[SerializeField]
	FunctionLibrary.FunctionName function = default;

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

	private void Update()
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
}

 