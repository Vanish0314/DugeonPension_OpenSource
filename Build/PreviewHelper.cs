using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Dungeon
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class PreviewHelper : MonoBehaviour
    { 
        [SerializeField] private SpriteRenderer previewRenderer;
    
        private Color m_ValidColor = new Color(0, 1, 0, 0.6f);
        private Color m_InvalidColor = new Color(1, 0, 0, 0.6f);

        private void OnEnable()
        {
            if(previewRenderer == null)
                previewRenderer = GetComponent<SpriteRenderer>();
        }

        public void Initialize(Sprite templateSprite)
        {
            previewRenderer.sprite = templateSprite;
            previewRenderer.enabled = false;
        }

        public void UpdatePreview(Vector3 worldPos, bool isValid)
        {
            previewRenderer.transform.position = worldPos;
            previewRenderer.color = isValid ? m_ValidColor : m_InvalidColor;
            previewRenderer.enabled = true;
        }

        public void HidePreview()
        {
            previewRenderer.enabled = false;
        }
    }
}
