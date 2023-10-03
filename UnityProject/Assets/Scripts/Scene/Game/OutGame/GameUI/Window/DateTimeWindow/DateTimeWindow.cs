using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class DateTimeWindow : WindowBase
    {
		[SerializeField]
		protected Common.AnimatorExpansion m_calenderAnime = null;

		[SerializeField]
		private CommonUI.TextExpansion m_dateText;

		[SerializeField]
		private CommonUI.TextExpansion m_timeText;

		[SerializeField]
		private Color m_normalColor;

		[SerializeField]
		private Color m_warningColor;



		public new void Initialize(RectTransform area, UnityAction holdCallback)
		{
			m_calenderAnime.Play("Default");
			m_dateText.text = "";
			m_timeText.text = "";
			m_timeText.color = m_normalColor;
			base.Initialize(area, holdCallback);
		}

		public override void SetupEvent(string[] paramStrings, UnityAction callback)
		{
		}

		protected override void SetupInputKeyEvent()
		{

		}

		//public void Play(string targetDateTimeStr, UnityAction callback)
		//{
		//	StartCoroutine(PlayCoroutine(targetDateTimeStr, callback));
		//}

		public IEnumerator PlayCoroutine(string targetDateTimeStr)
		{
			System.DateTime targetDateTime = System.DateTime.ParseExact(targetDateTimeStr, "MM/dd HH:mm:ss", null);
			m_dateText.text = targetDateTime.ToString("MM/dd");

			bool isDone = false;
			m_calenderAnime.Play("MoveDate", () => { isDone = true; });
			while (!isDone) { yield return null; }

			yield return new WaitForSeconds(0.5f);

			isDone = false;
			m_calenderAnime.Play("MoveCalender", () => { isDone = true; });
			while (!isDone) { yield return null; }

			yield return new WaitForSeconds(0.5f);

			isDone = false;
			m_calenderAnime.Play("MoveTime", () => { isDone = true; });
			
			string timeFormat = "HH:mm:ss";
			while (!isDone)
			{
				int randomAddSeconds = UnityEngine.Random.Range(0, 31104000);
				System.DateTime randomTime = targetDateTime.AddSeconds(randomAddSeconds);
				m_timeText.text = randomTime.ToString(timeFormat);
				yield return null;
			}

			System.DateTime startTime = targetDateTime.AddSeconds(-4);
			float totalDeltaTime = 0;
			int nowDateTimeStringCount = 0;
			bool isWarningUpdate = false;
			while (true)
			{
				if (nowDateTimeStringCount < timeFormat.Length)
				{
					int randomAddSeconds = UnityEngine.Random.Range(0, 31104000);
					System.DateTime randomDateTime = targetDateTime.AddSeconds(randomAddSeconds);
					float t = totalDeltaTime > 0.0f ? (totalDeltaTime - 0.0f) : 0.0f;
					nowDateTimeStringCount = t > 0.0f ? (int)(t / 0.1f) : 0;
					var span = new System.TimeSpan(hours: 0, minutes: 0, seconds: (int)totalDeltaTime);
					string nowString = (startTime + span).ToString(timeFormat);
					m_timeText.text = string.Format(
						"{0}{1}",
						nowString.Substring(0, nowDateTimeStringCount),
						randomDateTime.ToString(timeFormat).Substring(nowDateTimeStringCount, timeFormat.Length - nowDateTimeStringCount));
				}
				else if (totalDeltaTime < 5.5f)
				{
					// 待機
					var span = new System.TimeSpan(hours: 0, minutes: 0, seconds: (int)totalDeltaTime);
					string nowString = (startTime + span).ToString(timeFormat);
					m_timeText.text = nowString;

					if ((startTime + span) >= targetDateTime && isWarningUpdate == false)
					{
						m_timeText.color = m_warningColor;
						isWarningUpdate = true;
					}
				}
				else
				{
					break;
				}
				totalDeltaTime += Time.deltaTime;

				yield return null;
			}

			//if (callback != null)
			//{
			//	callback();
			//}
		}
	}
}