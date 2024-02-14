using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace scene.game.ingame.puzzlegame
{
    public class FieldTileElement : ObjectBase
    {
        public enum DirtType
		{
            None = -1,
            Milk,
            Art01,
            Art02,
            Art03,
        }

        [SerializeField]
        private SpriteRenderer m_dirtSpriteRenderer;

        [SerializeField]
        private Sprite[] m_dirtSprites;

        [SerializeField]
        private Texture m_selectedFieldTileTexture;

        [SerializeField]
        private Texture m_unselectFieldTileTexture;



        private Material m_material;

        private Color m_defaultColor;

        private Grid m_grid;

        private UnityAction<Grid> m_cleanEvent;

		public void Initialize(Grid grid, Color color, UnityAction<Grid> cleanEvent)
		{
            m_material = m_fbx.FBXObject.GetComponent<MeshRenderer>().material;
            m_defaultColor = color;
            m_material.color = m_defaultColor;
            m_grid = grid;
            m_cleanEvent = cleanEvent;

            base.Initialize(OnEvent);
        }

		public void Setting(DirtType dirtType)
		{
            if (dirtType != DirtType.None)
            {
                int index = (int)dirtType;
                m_dirtSpriteRenderer.gameObject.SetActive(true);
                m_dirtSpriteRenderer.sprite = m_dirtSprites[index];
            }
            else
            {
                m_dirtSpriteRenderer.gameObject.SetActive(false);
            }
        }

        public void SetSelected(bool value)
		{
            Texture tex = value ? m_selectedFieldTileTexture : m_unselectFieldTileTexture;
            m_material.mainTexture = tex;
            Color color = value ? Color.white : m_defaultColor;
            m_material.color = color;
        }

        private void OnEvent(string param)
        {
            string[] actionStrings = param.Split('_');
            switch (actionStrings[0])
            {
                case "Clean":
                    {
                        m_cleanEvent(m_grid);
                        return;
                    }
            }
        }
    }
}