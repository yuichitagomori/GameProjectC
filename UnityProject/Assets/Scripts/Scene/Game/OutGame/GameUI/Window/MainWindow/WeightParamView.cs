using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace scene.game.outgame.window.main
{
	public class WeightParamView : MonoBehaviour
	{
		[SerializeField]
		private CommonUI.TextExpansion m_weightParamText;
		
		[SerializeField]
		private Color m_normalColor;

		[SerializeField]
		private Color m_warningColor;

		[SerializeField]
		private Color m_errorColor;
		
		[SerializeField]
		private GameObject m_normalObject;

		[SerializeField]
		private GameObject m_warningObject;

		[SerializeField]
		private GameObject m_errorObject;



		private float m_weightParam;

		public void Initialize()
		{
			m_normalObject.SetActive(true);
			m_warningObject.SetActive(false);
			m_errorObject.SetActive(false);
			m_weightParamText.color = m_normalColor;

			StartCoroutine(UpdateCoroutine());
		}

		public void UpdateWeightParam(float weightParam)
		{
			m_weightParam = weightParam;
		}

		private IEnumerator UpdateCoroutine()
		{
			bool isWarning = false;
			bool isError = false;
			var waitTime = new WaitForSeconds(0.5f);
			while (true)
			{
				float param = m_weightParam + UnityEngine.Random.Range(-0.01f, 0.01f);
				if (param > 1.0f)
				{
					param = 1.0f;
				}
				else if (param < 0.0f)
				{
					param = 0.0f;
				}
				m_weightParamText.text = string.Format("{0}%", (param * 100).ToString("F2"));

				if (m_weightParam < 0.5f)
				{
					if (isWarning == true || isError == true)
					{
						m_normalObject.SetActive(true);
						m_warningObject.SetActive(false);
						m_errorObject.SetActive(false);
						m_weightParamText.color = m_normalColor;
						isWarning = false;
						isError = false;
					}
				}
				else if (m_weightParam < 0.8f)
				{
					if (isWarning == false)
					{
						m_normalObject.SetActive(false);
						m_warningObject.SetActive(true);
						m_errorObject.SetActive(false);
						m_weightParamText.color = m_warningColor;
						isWarning = true;
						isError = false;
					}
				}
				else
				{
					if (isError == false)
					{
						m_normalObject.SetActive(false);
						m_warningObject.SetActive(false);
						m_errorObject.SetActive(true);
						m_weightParamText.color = m_errorColor;
						isWarning = false;
						isError = true;
					}
				}

				yield return waitTime;
			}
		}
	}
}
