using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace scene.game.ingame.world
{
    [System.Serializable]
    public class ObjectController : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            public enum ObjectType
            {
                WantedLetter,   // 手配書
                Transfer        // 転送ポイント
            }

            [System.Serializable]
            public class Parameter
            {
                [SerializeField]
                private TransformData m_transformData;
                public TransformData TransformData => m_transformData;

                [SerializeField]
                private string m_actionEventParam;
                public string ActionEventParam => m_actionEventParam;
            }

            [SerializeField]
            private ObjectType m_type = ObjectType.WantedLetter;
            public ObjectType Type => m_type;

            [SerializeField]
            private Parameter[] m_parameters;
            public Parameter[] Parameters => m_parameters;

            [SerializeField]
            private Common.ElementList m_elementList;
            public Common.ElementList ElementList => m_elementList;
        }

        [SerializeField]
        private List<Data> m_dataList = null;



        private List<ObjectBase> m_objectList = new List<ObjectBase>();



        public void Initialize(UnityAction<string> eventCallback)
		{
            m_objectList.Clear();
            for (int i = 0; i < m_dataList.Count; ++i)
            {
                var data = m_dataList[i];
                Data.ObjectType type = data.Type;
                Data.Parameter[] parameters = data.Parameters;
                var elements = data.ElementList.GetElements();
                for (int j = 0; j < elements.Count; ++j)
                {
                    int controllId = m_objectList.Count + 1;
                    Data.Parameter param = (j < parameters.Length) ? parameters[j] : null;
                    world.ObjectBase objectBase = elements[j].GetComponent<world.ObjectBase>();
                    objectBase.Initialize(
                        type,
                        controllId,
                        param,
                        eventCallback);
                    m_objectList.Add(objectBase);
                }
            }
        }

		public world.ObjectBase GetObject(int controllId)
		{
			return m_objectList.Find(d => (d.ControllId == controllId));
		}
	}
}