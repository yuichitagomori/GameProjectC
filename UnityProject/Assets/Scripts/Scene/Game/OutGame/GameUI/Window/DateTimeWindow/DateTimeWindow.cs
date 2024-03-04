using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace scene.game.outgame.window
{
    [System.Serializable]
    public class DateTimeWindow : WindowBase
    {
		[System.Serializable]
		public class Data
		{
			[SerializeField]
			private int m_controllId;
			public int ControllId => m_controllId;

			[SerializeField]
			private string[] m_paramStrings;
			public string[] ParamStrings => m_paramStrings;
		}

		[System.Serializable]
		public class CalendarDayData
		{
			public enum Type
			{
				Normal,
				Worning,
			}

			[SerializeField]
			private int m_day;
			public int Day => m_day;

			[SerializeField]
			private Type m_dayType;
			public Type DayType => m_dayType;
		}

		[SerializeField]
		private Data[] m_contentsDatas;

		[SerializeField]
		private CalendarDayData[] m_calendarDayDatas;

		[SerializeField]
		private Common.AnimatorExpansion m_calendarAnime;

		[SerializeField]
		private Image[] m_calendarDayImages;

		[SerializeField]
		private Sprite m_calendarDayNormalSprite;

		[SerializeField]
		private Sprite m_calendarDayWorningSprite;

		[SerializeField]
		private CommonUI.TextExpansion m_dateText;

		[SerializeField]
		private CommonUI.TextExpansion m_timeText;



		private System.DateTime m_nowDateTime;

		private float m_addTime = 0.0f;

		public override void Go()
		{
			m_calendarAnime.Play("Default", null);
			StartCoroutine(PlayTimeMove());
		}

		public override void OnMovieStart(string[] paramStrings, UnityAction callback)
		{
			StartCoroutine(OnMovieStartCoroutine(paramStrings, callback));
		}

		private IEnumerator OnMovieStartCoroutine(string[] paramStrings, UnityAction callback)
		{
			int controllId = int.Parse(paramStrings[0]);
			var data = m_contentsDatas.FirstOrDefault(d => d.ControllId == controllId);

			//System.DateTime targetDateTime = System.DateTime.ParseExact(targetDateTimeStr, "MM/dd HH:mm:ss", null);
			//m_dateText.text = targetDateTime.ToString("MM/dd");

			//bool isDone = false;
			//m_calenderAnime.Play("MoveDate", () => { isDone = true; });
			//while (!isDone) { yield return null; }

			//yield return new WaitForSeconds(0.5f);

			//isDone = false;
			//m_calenderAnime.Play("MoveCalender", () => { isDone = true; });
			//while (!isDone) { yield return null; }

			//yield return new WaitForSeconds(0.5f);

			//isDone = false;
			//m_calenderAnime.Play("MoveTime", () => { isDone = true; });

			//string timeFormat = "HH:mm:ss";
			//while (!isDone)
			//{
			//	int randomAddSeconds = UnityEngine.Random.Range(0, 31104000);
			//	System.DateTime randomTime = targetDateTime.AddSeconds(randomAddSeconds);
			//	m_timeText.text = randomTime.ToString(timeFormat);
			//	yield return null;
			//}

			//System.DateTime startTime = targetDateTime.AddSeconds(-4);
			//float totalDeltaTime = 0;
			//int nowDateTimeStringCount = 0;
			//bool isWarningUpdate = false;
			//while (true)
			//{
			//	if (nowDateTimeStringCount < timeFormat.Length)
			//	{
			//		int randomAddSeconds = UnityEngine.Random.Range(0, 31104000);
			//		System.DateTime randomDateTime = targetDateTime.AddSeconds(randomAddSeconds);
			//		float t = totalDeltaTime > 0.0f ? (totalDeltaTime - 0.0f) : 0.0f;
			//		nowDateTimeStringCount = t > 0.0f ? (int)(t / 0.1f) : 0;
			//		var span = new System.TimeSpan(hours: 0, minutes: 0, seconds: (int)totalDeltaTime);
			//		string nowString = (startTime + span).ToString(timeFormat);
			//		m_timeText.text = string.Format(
			//			"{0}{1}",
			//			nowString.Substring(0, nowDateTimeStringCount),
			//			randomDateTime.ToString(timeFormat).Substring(nowDateTimeStringCount, timeFormat.Length - nowDateTimeStringCount));
			//	}
			//	else if (totalDeltaTime < 5.5f)
			//	{
			//		// 待機
			//		var span = new System.TimeSpan(hours: 0, minutes: 0, seconds: (int)totalDeltaTime);
			//		string nowString = (startTime + span).ToString(timeFormat);
			//		m_timeText.text = nowString;

			//		if ((startTime + span) >= targetDateTime && isWarningUpdate == false)
			//		{
			//			isWarningUpdate = true;
			//		}
			//	}
			//	else
			//	{
			//		break;
			//	}
			//	totalDeltaTime += Time.deltaTime;

			//	yield return null;
			//}

			//if (callback != null)
			//{
			//	callback();
			//}

			bool isDone = false;

			int paramStringsIndex = 0;
			while (paramStringsIndex < data.ParamStrings.Length)
			{
				string[] paramStrings2 = data.ParamStrings[paramStringsIndex].Split(',');
				switch (paramStrings2[0])
				{
					case "AnimationName":
						{
							string animationType = paramStrings2[1];
							string animationName = paramStrings2[2];
							if (animationType == "Play")
							{
								isDone = false;
								m_calendarAnime.Play(animationName, () => { isDone = true; });
								while (!isDone) { yield return null; }
							}
							else if (animationType == "PlayLoop")
							{
								m_calendarAnime.PlayLoop(animationName);
							}
							break;
						}
					case "WaitTime":
						{
							float time = float.Parse(paramStrings2[1]);
							yield return new WaitForSeconds(time);
							break;
						}
					case "DateTime":
						{
							string DateString = paramStrings2[1];
							m_nowDateTime = System.DateTime.ParseExact(DateString, "MM/dd HH:mm:ss", null);
							SetupCalendar();
							break;
						}
				}
				paramStringsIndex++;
			}

			if (callback != null)
			{
				callback();
			}
		}

		private IEnumerator PlayTimeMove()
		{
			while (true)
			{
				m_addTime += Time.deltaTime;
				if (m_nowDateTime == null)
				{
					yield return null;
					continue;
				}

				var now = m_nowDateTime.AddSeconds(m_addTime);
				m_dateText.text = now.ToString("MM/dd");
				m_timeText.text = now.ToString("HH:mm:ss");

				yield return null;
			}
		}

		private void SetupCalendar()
		{
			if (m_nowDateTime == null)
			{
				return;
			}

			int nowDay = m_nowDateTime.Day;
			for (int i = 0; i < m_calendarDayImages.Length; ++i)
			{
				int day = nowDay - 2 + i;
				var calendarDayData = m_calendarDayDatas.FirstOrDefault(d => d.Day == day);
				if (calendarDayData == null)
				{
					m_calendarDayImages[i].gameObject.SetActive(false);
					continue;
				}
				m_calendarDayImages[i].gameObject.SetActive(true);
				m_calendarDayImages[i].sprite = calendarDayData.DayType == CalendarDayData.Type.Normal ?
					m_calendarDayNormalSprite :
					m_calendarDayWorningSprite;
			}
		}
	}
}