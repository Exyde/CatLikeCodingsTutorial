using UnityEngine;

public class GPUGraph : MonoBehaviour
{
	[SerializeField]
	[Range(10, 200)] int resolution  = 10;

	FunctionLibrary.FunctionName transitionFunction;
	[SerializeField]
	FunctionLibrary.FunctionName function = default;

	public enum TransitionMode { Cycle, Random }
	[SerializeField]
	TransitionMode transitionMode;

	[SerializeField, Min(0f)]
	float functionDuration = 1f, transitionDuration = 1f;

	float duration;	
	bool transitioning;

	ComputeBuffer positionsBuffer;
	[SerializeField] ComputeShader computeShader;

	[SerializeField] Material material;
	[SerializeField] Mesh mesh;


	static readonly int positionsID = Shader.PropertyToID("_Positions");
	static readonly int resolutionID = Shader.PropertyToID("_Resolution");
	static readonly int stepID = Shader.PropertyToID("_Step");
	static readonly int timeID = Shader.PropertyToID("_Time");


	//Right after awake, every hot reload as well
	void OnEnable(){
		positionsBuffer = new ComputeBuffer(resolution * resolution, 3 * 4); //3 float * 4 bytes
		
	}

	void OnDisable(){
		positionsBuffer.Release();
		positionsBuffer = null;
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

		UpdateFunctionOnGpu();
	}

	void UpdateFunctionOnGpu(){
		float step = 2f / resolution;
		computeShader.SetInt(resolutionID, resolution);
		computeShader.SetFloat(stepID, step);
		computeShader.SetFloat(timeID, Time.time);
		computeShader.SetBuffer(0, positionsID, positionsBuffer);

		int groups = Mathf.CeilToInt(resolution / 8f);
		computeShader.Dispatch(0, groups, groups, 1);

		var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
		Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionsBuffer.count);
	}

	void PickNextFunction () {
		function = transitionMode == TransitionMode.Cycle ?
			FunctionLibrary.GetNextFunctionName(function) :
			FunctionLibrary.GetRandomFunctionNameOtherThan(function);
	}
}

 