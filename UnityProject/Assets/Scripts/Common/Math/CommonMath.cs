using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommonMath : MonoBehaviour
{
	public static Vector3 GetEulerAngles(Transform transform)
	{
		return GetEulerAngles(transform.eulerAngles);
	}

	public static Vector3 GetEulerAngles(Vector3 angle)
	{
		if (angle.x < -180.0f) { angle.x += 360.0f; }
		if (angle.x > 180.0f) { angle.x -= 360.0f; }
		if (angle.y < -180.0f) { angle.y += 360.0f; }
		if (angle.y > 180.0f) { angle.y -= 360.0f; }
		if (angle.z < -180.0f) { angle.z += 360.0f; }
		if (angle.z > 180.0f) { angle.z -= 360.0f; }
		return angle;
	}
}
