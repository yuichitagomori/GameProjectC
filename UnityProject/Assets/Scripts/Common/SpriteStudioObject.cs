using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// SpriteStudioオブジェクト管理クラス
/// </summary>
[System.Serializable]
public class SpriteStudioObject : MonoBehaviour
{
	/// <summary>
	/// SetUserDataCallbackの返却文字列がjsonの場合に使用するクラス
	/// </summary>
	[System.Serializable]
	public class JsonUserData
	{
		[System.Serializable]
		public class Command
		{
			public string m_type = "";
			public string[] m_param = { };
		}

		private UnityAction<Command> m_callback = null;

		public JsonUserData(UnityAction<Command> _callback)
		{
			m_callback = _callback;
		}

		public void ReceiveCommand(string _string)
		{
			if (m_callback == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(_string) == true)
			{
				return;
			}
			Command command = (Command)JsonUtility.FromJson(_string, typeof(Command));
			if (command == null)
			{
				return;
			}
			m_callback(command);
		}
	}

	/// <summary>
	/// SSデータ追加先
	/// </summary>
	[SerializeField]
	private Transform m_ssParent;

	/// <summary>
	/// SpriteStudioRootデータ
	/// </summary>
	private Script_SpriteStudio6_Root m_ssr;

	private string m_fileName = "";
	public string FileName { get { return m_fileName; } }

	/// <summary>
	/// アニメーションID
	/// </summary>
	private int m_animId = -1;
	public int AnimId { get { return m_animId; } }

	/// <summary>
	/// ループアニメーション中であるかどうか
	/// </summary>
	private bool m_isLoop = false;
	public bool IsLoop { get { return m_isLoop; } }



	/// <summary>
	/// 設定
	/// </summary>
	/// <param name="_fileName"></param>
	public void Setup(string _fileName)
	{
		if (m_fileName == _fileName)
		{
			return;
		}

		// 再設定時の為に初期化
		Clear();

		GameObject prefab = GeneralRoot.Instance.SSData.Prefabs.Find(d => d.name == _fileName);
		if (prefab == null)
		{
			throw new System.Exception("指定の名前のSSデータが見つかりません:FILENAME = " + _fileName);
		}

		m_fileName = _fileName;
		GameObject ssrObject = GameObject.Instantiate(prefab);
        ssrObject.layer = m_ssParent.gameObject.layer;
        ssrObject.transform.SetParent(m_ssParent);
		ssrObject.transform.localPosition = Vector3.zero;
		ssrObject.transform.localRotation = Quaternion.identity;
		m_ssr = ssrObject.GetComponent<Script_SpriteStudio6_Root>();
		m_animId = -1;
	}

	/// <summary>
	/// リセット
	/// </summary>
	public void Clear()
	{
		if (m_ssr != null)
		{
			GameObject.Destroy(m_ssr.gameObject);
			m_ssr = null;
		}
		m_fileName = "";
		m_animId = -1;
	}

	/// <summary>
	/// アニメーション再生（1度だけ）
	/// </summary>
	/// <param name="_id"></param>
	/// <param name="_endCallback"></param>
	/// <param name="_isAutoDelete"></param>
	public void PlayOnce(
		int _id,
		UnityAction _endCallback = null,
		bool _isAutoDelete = false)
	{
		m_ssr.FunctionPlayEnd = (
			Script_SpriteStudio6_Root InstanceRoot,
			GameObject ObjectControl) =>
		{
			m_ssr.FunctionPlayEnd = null;
			if (_isAutoDelete == true)
			{
				m_fileName = "";
				m_animId = -1;
			}
			if (_endCallback != null)
			{
				_endCallback();
			}
			return !_isAutoDelete;
		};
		m_animId = _id;
		m_ssr.AnimationPlay(0, m_animId, 1);
		m_isLoop = false;
	}

	/// <summary>
	/// アニメーション再生（ループ）
	/// </summary>
	/// <param name="_id"></param>
	public void PlayLoop(int _id)
	{
		m_animId = _id;
		m_ssr.AnimationPlay(0, m_animId, 0);
		m_isLoop = true;
	}

	/// <summary>
	/// アニメーションユーザー設定コールバック設定
	/// </summary>
	/// <param name="_userDataCallback"></param>
	public void SetUserDataCallback(UnityAction<string> _userDataCallback)
	{
        //public delegate void FunctionUserData(
        //  Script_SpriteStudio6_Root scriptRoot,
        //  string nameParts,
        //  int indexParts,
        //  int indexAnimation,
        //  int frameDecode,
        //  int frameKeyData,
        //  ref Library_SpriteStudio6.Data.Animation.Attribute.UserData userData,
        //  bool flagWayBack);
        m_ssr.FunctionUserData = (
			Script_SpriteStudio6_Root scriptRoot,
			string nameParts,
			int indexParts,
			int indexAnimation,
			int FrameNoDecode,
			int FrameNoKeyData,
			ref Library_SpriteStudio6.Data.Animation.Attribute.UserData userData,
			bool FlagWayBack) =>
		{
			Debug.Log("userData.Text = " + userData.Text);
			if (_userDataCallback != null)
			{
				_userDataCallback(userData.Text);
			}
		};
	}

	/// <summary>
	/// 表示切替
	/// </summary>
	/// <param name="_value"></param>
	public void SetActive(bool _value)
	{
		if (m_ssParent.gameObject.activeInHierarchy != _value)
		{
			if (_value == false)
			{
				m_ssr.AnimationStop(m_animId);
			}
			else
			{
				m_ssr.AnimationPlay(m_animId);
			}
			m_ssParent.gameObject.SetActive(_value);
		}
	}

	/// <summary>
	/// パーツの非表示
	/// </summary>
	/// <param name="_partsName"></param>
	/// <param name="_active"></param>
	/// <param name="_activeChildren"></param>
	public void SetActiveParts(
		string _partsName,
		bool _active,
		bool _activeChildren = false)
	{
		int id = m_ssr.IDGetParts(_partsName);
		bool result = false;
		if (id > 0)
		{
			result = m_ssr.HideSet(id, !_active, true);
		}
		if (result == false)
		{
			Debug.LogWarning("Not Find IDGetParts");
		}
	}

	/// <summary>
	/// パーツの非表示（エフェクトパーツ用）
	/// </summary>
	/// <param name="_partsName"></param>
	/// <param name="_active"></param>
	public void SetActiveEffectParts(
		string _partsName,
		bool _active)
	{
		int id = m_ssr.IDGetParts(_partsName);
		bool result = false;
		if (id > 0)
		{
			result = m_ssr.HideSet(id, !_active, true);
		}
		if (result == false)
		{
			Debug.Log("Error!!:_partsName = " + _partsName);
		}

		//int id = m_ssr.IDGetParts(_partsName);
		//var cntParts = m_ssr.ControlGetParts(id);
		//if (cntParts != null)
		{
			//var ssre = cntParts.InstanceRootUnderControlEffect;
			//if (ssre != null)
			//{
			//	ssre.FlagHideForce = !_active;
			//}
			//else
			{
				Debug.LogWarning("Not Find ssre");
			}
		}
		//else
		{
			Debug.LogWarning("Not Find cntParts");
		}
	}

	/// <summary>
	/// パーツのTransform取得
	/// </summary>
	/// <param name="_partsName"></param>
	public Transform GetPartsTransform(string _partsName)
	{
		int id = m_ssr.IDGetParts(_partsName);
		Transform transform = null;
		if (id > 0)
		{
			Debug.Log("name = " + m_ssr.TableControlParts[id].InstanceGameObject.transform.name);
			transform = m_ssr.TableControlParts[id].InstanceGameObject.transform;
		}
		return transform;
	}

	/// <summary>
	/// テクスチャ変更
	/// </summary>
	/// <param name="_index"></param>
	/// <param name="_texture"></param>
	public void ChangeMaterialTexture(int _index, Texture2D _texture)
	{
        var materials = m_ssr.TableCopyMaterialDeep();
        Script_SpriteStudio6_Root.Material.TextureSet(materials, _index, _texture, false);
        m_ssr.TableSetMaterial(materials);
	}

	/// <summary>
	/// アニメーション数取得
	/// </summary>
	/// <returns></returns>
	public int GetAnimCount()
	{
		if (m_ssr == null)
		{
			return -1;
		}

		return m_ssr.CountGetAnimation();
	}
}
