using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// アニメーターのアニメ終了時にコールバックを呼ぶ管理クラス
/// </summary>
[RequireComponent(typeof(Animator))]
public class AnimatorExpansion : MonoBehaviour
{
    /// <summary>
    /// アニメーター
    /// </summary>
    [SerializeField]
	private Animator m_animator;


	/// <summary>
	/// レイヤー番号
	/// </summary>
	private int m_layer = 0;

	/// <summary>
	/// アニメーション名
	/// </summary>
	private string m_animationName = "";

	/// <summary>
	/// アニメーションイベント
	/// </summary>
	private UnityAction<string> m_eventFucntion = null;

	/// <summary>
	/// アニメーション終了感知コルーチン
	/// </summary>
	private Coroutine m_playCoroutine = null;



	private void Reset()
	{
		if (m_animator == null)
		{
			m_animator = GetComponent<Animator>();
		}
		m_animator.enabled = false;
	}

	public void OnEvent(string eventStr)
	{
		if (m_eventFucntion != null)
		{
			m_eventFucntion(eventStr);
		}
	}

	/// <summary>
	/// アニメーション再生
	/// </summary>
	/// <param name="name"></param>
	/// <param name="callback"></param>
	/// <param name="eventFucntion"></param>
	/// <param name="time"></param>
	public void Play(
		string name,
		UnityAction callback = null,
		UnityAction<string> eventFucntion = null,
		float time = 0.0f)
	{
		m_animationName = name;
		m_eventFucntion = eventFucntion;
		if (m_playCoroutine != null)
		{
			StopCoroutine(m_playCoroutine);
		}
		m_playCoroutine = StartCoroutine(PlayColoutine(name, callback, time));
	}

	/// <summary>
	/// ループアニメーション再生
	/// </summary>
	/// <param name="name"></param>
	/// <param name="eventFucntion"></param>
	/// <param name="time"></param>
	public void PlayLoop(
		string name,
		UnityAction<string> eventFucntion = null,
		float time = 0.0f)
	{
		m_animationName = name;
		m_eventFucntion = eventFucntion;
		m_animator.enabled = true;
		m_animator.Play(name, m_layer, time);
	}

	/// <summary>
	/// アニメーション停止
	/// </summary>
	public void Stop()
	{
		m_animator.enabled = false;
	}

	/// <summary>
	/// アニメーションフレーム指定
	/// </summary>
	/// <param name="addTime"></param>
	public void UpdateFrame(float addTime)
	{
		m_animator.Update(addTime);
	}

	/// <summary>
	/// アニメーション名取得
	/// </summary>
	/// <returns></returns>
	public string GetAnimationName()
	{
		return m_animationName;
	}

	/// <summary>
	/// 待機
	/// </summary>
	/// <param name="name"></param>
	/// <param name="callback"></param>
	/// <param name="time"></param>
	/// <returns></returns>
	private IEnumerator PlayColoutine(string name, UnityAction callback, float time)
    {
		m_animator.enabled = true;
		m_animator.Play(name, m_layer, 0.0f);

		while (true)
		{
			var info = m_animator.GetCurrentAnimatorStateInfo(m_layer);
			if (info.normalizedTime <= 0.0f)
			{
				break;
			}

			// 遷移できていない、もしくは再生中
			yield return null;
		}

		while (true)
        {
			var info = m_animator.GetCurrentAnimatorStateInfo(m_layer);
			if (info.IsName(name) == true &&
				info.normalizedTime >= 1.0f)
            {
                break;
            }

            // 遷移できていない、もしくは再生中
            yield return null;
        }

		m_animator.enabled = false;

		// コールバックを呼ぶ
		if (callback != null)
        {
            callback();
        }
    }
}
