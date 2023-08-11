using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;

namespace scene.game.ingame.world
{
    [System.Serializable]
    public class NPCController : MonoBehaviour
    {
        [System.Serializable]
        public class Data
        {
            public enum NavmeshMoveType
            {
                WalkNear,   // 近い場所だけを歩く
                WalkFar,    // 遠い場所も歩く
            }

            [System.Serializable]
            public class Parameter
            {
                [SerializeField]
                private string m_actionEventParam;
                public string ActionEventParam => m_actionEventParam;

                [SerializeField]
                private Transform m_transformData;
                public Transform TransformData => m_transformData;
            }

            [SerializeField]
            private int m_characterId = -1;
            public int CharacterId => m_characterId;

            [SerializeField]
            private NavmeshMoveType m_moveType;
            public NavmeshMoveType MoveType => m_moveType;

            [SerializeField]
            private Parameter[] m_parameters;
            public Parameter[] Parameters => m_parameters;

            [SerializeField]
            private Common.ElementList m_elementList;
            public Common.ElementList ElementList => m_elementList;
        }

        [SerializeField]
        private List<Data> m_dataList = null;



        private List<NPC> m_characterList = new List<NPC>();

        private Transform m_ingameCameraTransform;

        private UnityAction<string> m_eventCallback;

        private Vector3[] m_navmeshPointList;



        public void Initialize(
            Transform ingameCameraTransform,
            Vector3[] navmeshPointList,
            UnityAction<string> eventCallback)
		{
            m_ingameCameraTransform = ingameCameraTransform;
            m_navmeshPointList = navmeshPointList;
            m_eventCallback = eventCallback;

            m_characterList.Clear();
        }

        public void SetSequenceTime(float value)
		{
            for (int i = 0; i < m_characterList.Count; ++i)
			{
                var character = m_characterList[i];
                character.SetSequenceTime(value);
            }
        }

        public world.NPC GetNPC(int controllId)
        {
            return m_characterList.Find(d => (d.ControllId == controllId));
        }

        public void DeleteNPC(int controllId)
        {
            var npc = GetNPC(controllId);
            if (npc == null)
			{
                return;
			}
            m_characterList.Remove(npc);
            npc.gameObject.SetActive(false);
        }

        public void AddNPC(int characterId, int colorId, int count, UnityAction callback)
		{
            StartCoroutine(AddNPCCoroutine(characterId, colorId, count, callback));
        }

        private IEnumerator AddNPCCoroutine(int characterId, int colorId, int count, UnityAction callback)
        {
            int doneCount = 0;
            for (int i = 0; i < count; ++i)
            {
                StartCoroutine(AddNPCCoroutine(characterId, colorId, () => { doneCount++; }));
            }

            while (doneCount < count)
			{
                yield return null;
			}

            if (callback != null)
			{
                callback();
			}
        }

        private IEnumerator AddNPCCoroutine(int characterId, int colorId, UnityAction callback)
		{
            var data = m_dataList.Find(d => d.CharacterId == characterId);
            data.ElementList.AddElement();
            var elements = data.ElementList.GetElements();
            int index = elements.Count - 1;
            world.NPC character = elements[index].GetComponent<world.NPC>();
            Data.Parameter param = (index < data.Parameters.Length) ? data.Parameters[index] : null;
            int controllId = m_characterList.Count + 1;
            character.Initialize(
                characterId,
                colorId,
                controllId,
                param,
                m_ingameCameraTransform,
                m_navmeshPointList,
                m_eventCallback);
            m_characterList.Add(character);

            float time = 0.0f;
            float sequenceTime = 4.0f;
            while (time < sequenceTime)
			{
                time += Time.deltaTime;
                float value = time / sequenceTime;
                character.SetSequenceTime(value);
                yield return null;
			}

            if (callback != null)
			{
                callback();
            }
        }
    }
}