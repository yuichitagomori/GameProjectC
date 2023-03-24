using UnityEngine;
using System.Collections.Generic;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace data.resource
{
    /// <summary>
    /// リソースデータ基底クラス
    /// </summary>
    public abstract class ResourceDataBase<T> : ScriptableObject where T : ResourceDataBase<T>.DataBase 
    {
        [System.Serializable]
        public abstract class DataBase
		{
            [SerializeField, HideInInspector]
            protected string m_name = "";

            /// <summary>
            /// ID
            /// </summary>
            [SerializeField]
            protected int m_id;
            public int Id => m_id;
        }

        /// <summary>
		/// データリスト
		/// </summary>
		[SerializeField]
        private T[] m_datas;

        public T Find(int id)
		{
            return m_datas.FirstOrDefault(d => d.Id == id);
		}

#if UNITY_EDITOR
		public void Setup()
		{
			List<T> dataList = new List<T>();
			for (int i = 0; i < 1000; ++i)
			{
				int id = i + 1;
				var data = CreateData(id);
				if (data == null)
				{
					continue;
				}
				dataList.Add(data);
			}
			m_datas = dataList.ToArray();
		}

		/// <summary>
		/// 変換
		/// </summary>
		/// <param name="id"></param>
		public abstract T CreateData(int id);
#endif
	}
}