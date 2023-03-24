using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// 進行補助
/// </summary>
public class Progress : MonoBehaviour
{
	public IEnumerator SetupCoroutine(
		float beforeValue,
		float afterValue,
		float time,
		AnimationCurve curve,
		UnityAction<float> update,
		UnityAction callback)
	{
		float nowTime = 0.0f;
		while (nowTime < time)
		{
			nowTime += Time.deltaTime;

			float par = Mathf.Min((nowTime / time), 1.0f);
			if (curve != null)
			{
				par = curve.Evaluate(par);
			}
			float value = beforeValue - ((beforeValue - afterValue) * par);
			if (update != null)
			{
				update(value);
			}

			yield return null;
		}
		if (callback != null)
		{
			callback();
		}
	}
}
