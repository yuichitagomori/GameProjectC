using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

	public static IEnumerator TransformLerpCoroutine(
		Transform transform,
		Vector3 beforePosition,
		Quaternion beforeRotation,
		Vector3 afterPosition,
		Quaternion afterRotation,
		float time,
		UnityAction callback)
	{
		float nowTime = 0.0f;
		var curve = AnimationCurve.EaseInOut(0.0f, 0.0f, 1.0f, 1.0f);
		while (nowTime < time)
		{
			nowTime += Time.deltaTime;
			float t = curve.Evaluate(nowTime / time);
			TransformLerp(
				transform,
				beforePosition,
				beforeRotation,
				afterPosition,
				afterRotation,
				t);
			yield return null;
		}

		if (callback != null)
		{
			callback();
		}
	}

	public static void TransformLerp(
		Transform transform,
		Vector3 beforePosition,
		Quaternion beforeRotation,
		Vector3 afterPosition,
		Quaternion afterRotation,
		float value)
	{
		Vector3 position = Vector3.Lerp(beforePosition, afterPosition, value);
		Quaternion rotation = Quaternion.Lerp(beforeRotation, afterRotation, value);
		transform.localPosition = position;
		transform.localRotation = rotation;
	}
}
