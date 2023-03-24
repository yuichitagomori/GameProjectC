using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace data.master
{
    /// <summary>
    /// スクリプタブルオブジェクトデータ基底クラス
    /// </summary>
    public abstract class MasterDataBase<T> : ScriptableObject where T : MasterDataBase<T>.DataBase 
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

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="csv"></param>
        public void Load(string[][] csv)
		{
            List<T> dataList = new List<T>();
            foreach (string[] strings in csv)
            {
                var data = CreateData(strings);
                if (data == null)
                {
                    continue;
                }
                dataList.Add(data);
            }
            m_datas = dataList.ToArray();
        }

        /// <summary>
        /// 生成
        /// </summary>
        /// <param name="csvParam"></param>
        public abstract T CreateData(string[] csvParam);
    }
}