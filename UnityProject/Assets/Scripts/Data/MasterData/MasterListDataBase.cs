using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace data.master
{
    /// <summary>
    /// スクリプタブルオブジェクトデータ基底クラス
    /// </summary>
    public class MasterListDataBase<TMaster, TData> : ScriptableObject
        where TMaster : MasterDataBase<TData>
        where TData : MasterDataBase<TData>.DataBase
    {
        [System.Serializable]
        public class Data : MasterDataBase<TData>.DataBase
        {
            [SerializeField]
            private TMaster m_master;
            public TMaster Master => m_master;

            public Data(int id, TMaster master)
			{
                m_name = id.ToString();
                m_id = id;
                m_master = master;
			}
        }

        /// <summary>
		/// データリスト
		/// </summary>
		[SerializeField]
        private Data[] m_datas;

        public TMaster Find(int id)
		{
            var data = m_datas.FirstOrDefault(d =>  d.Id == id);
            if (data == null)
			{
                Debug.LogError(string.Format("not found data id = {0}", id));
			}
            return data.Master;
		}

        /// <summary>
        /// 読み込み
        /// </summary>
        /// <param name="datas"></param>
        public void Load(Data[] datas)
		{
            if (datas == null)
			{
                return;
			}
            List<Data> dataList = new List<Data>();
            foreach (Data data in datas)
            {
                dataList.Add(data);
            }
            m_datas = dataList.ToArray();
        }
    }

}