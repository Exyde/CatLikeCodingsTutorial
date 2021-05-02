using UnityEngine;
using static UnityEngine.Mathf;

//Documentation : https://catlikecoding.com/unity/tutorials/basics/mathematical-surfaces/
public static class FunctionLibrary
{
	public delegate Vector3 Function(float u, float v, float t);

	public enum FunctionName { Wave, MultiWave, Ripple, Sphere};

	static Function[] functions =
	{
		Wave, MultiWave, Ripple, Sphere
	};

	public static Function GetFunction (FunctionName name)
	{
		return functions[(int)name];
	}
	public static Vector3 Wave(float u, float v, float t)
	{
		Vector3 p;
		p.x = u;
		p.y  = Sin(PI * (u + v + t));
		p.z = t;
		return p;
	}

	public static Vector3 MultiWave(float u, float v, float t)
	{
		Vector3 p;
		p.x = u;
		p.y = Sin(PI * (u +  .5f * t));
		p.y += .5f * Sin(2f * PI * (v + t));
		p.y += Sin(PI * (u + v + 0.25f * t));
		p.y *= 1f / 2.5f;
		p.z = v;
		return p;
	}

	public static Vector3 Ripple(float u, float v, float t)
	{
		float d = Sqrt(u * u + v * v);
		Vector3 p;
		p.x = u;
		p.y = Sin(4f * PI * d - t);
		p.y /= (1f + 10f * d);
		p.z = v;
		return p;
	}

	public static Vector3 Sphere(float u, float v, float t)
	{
		float r = 0.9f + 0.1f * Sin(PI * (6f * u + 4f * v + t));
		float s = r * Cos(.5f * PI * v);
		Vector3 p;
		p.x = s * Sin(PI * u);
		p.y = r * Sin(PI * .5f * v);
		p.z = s * Cos(PI * u);
		return p;
	}


}