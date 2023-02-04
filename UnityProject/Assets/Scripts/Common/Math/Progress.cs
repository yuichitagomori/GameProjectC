using UnityEngine;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// 進行補助
/// </summary>
public class Progress : MonoBehaviour
{
	public IEnumerator SetupCoroutine(
		float _before,
		float _after,
		float _time,
		AnimationCurve _curve,
		UnityAction<float> _update,
		UnityAction _callback)
	{
		float nowTime = 0.0f;
		while (nowTime < _time)
		{
			nowTime += Time.deltaTime;

			float par = Mathf.Min((nowTime / _time), 1.0f);
			if (_curve != null)
			{
				par = _curve.Evaluate(par);
			}
			float value = _before - ((_before - _after) * par);
			if (_update != null)
			{
				_update(value);
			}

			yield return null;
		}
		if (_callback != null)
		{
			_callback();
		}
	}
}
