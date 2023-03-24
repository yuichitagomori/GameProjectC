using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace scene.game.ingame.world
{
    [System.Serializable]
    public class EnemyController : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            [SerializeField]
            private int m_enemyID = -1;
            public int EnemyID => m_enemyID;

            [SerializeField]
            private Common.ElementList m_elementList;
            public Common.ElementList ElementList => m_elementList;
        }

        [SerializeField]
        private List<Data> m_dataList = null;

        private List<Enemy> m_enemyList = new List<Enemy>();

        public void Initialize(
            Transform ingameCameraTransform,
            Vector3[] navmeshPointList,
            UnityAction<string> enterCallback,
            UnityAction<string> exitCallback)
		{
            var enemyColorResource = GeneralRoot.Resource.EnemyColorResource;

            m_enemyList.Clear();
            for (int i = 0; i < m_dataList.Count; ++i)
            {
                var elements = m_dataList[i].ElementList.GetElements();
                for (int j = 0; j < elements.Count; ++j)
                {
                    world.Enemy enemy = elements[j].GetComponent<world.Enemy>();
                    int enemyId = m_dataList[i].EnemyID;
                    int controllId = m_enemyList.Count + 1;
                    int colorId = m_enemyList.Count + 1;
                    data.resource.EnemyColorResource.Data enemyData = enemyColorResource.Find(enemyId);
                    data.resource.EnemyColorResource.Data.ColorData colorData = enemyData.ColorDatas.Where(d => d.ColorId == colorId).First();
                    enemy.Initialize(
                        controllId,
                        enemyId,
                        ingameCameraTransform,
                        navmeshPointList,
                        colorData,
                        enterCallback,
                        exitCallback);
                    m_enemyList.Add(enemy);
                }
            }
        }

        public void SetSequenceTime(float value)
		{
            for (int i = 0; i < m_enemyList.Count; ++i)
			{
                var enemy = m_enemyList[i];
                enemy.SetSequenceTime(value);
            }
        }

        public world.Enemy GetEnemy(int controllId)
        {
            return m_enemyList.Find(d => (d.ControllId == controllId));
        }
    }
}