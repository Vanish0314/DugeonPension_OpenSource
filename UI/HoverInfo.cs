using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Dungeon
{
    public class HoverInfo : MonoBehaviour
    {
        [SerializeField] private InputReader inputReader;

        [SerializeField] private GameObject infoPanel;
        [SerializeField] private float hoverDelay = 0.3f;

        private Graphic _graphic;
        private Vector2 _mousePosition;
        private bool _isHovering;
        private float _hoverTimer;

        private void Awake()
        {
            _graphic = GetComponent<Graphic>();
            if (_graphic == null)
            {
                _graphic = gameObject.AddComponent<Image>();
                _graphic.color = Color.clear;
            }
        }

        private void OnEnable()
        {
            inputReader.OnMouseMoveEvent += OnMouseMove;
        }

        private void OnDisable()
        {
            inputReader.OnMouseMoveEvent -= OnMouseMove;
        }

        private void Update()
        {
            bool nowHovering = IsPointerOverUIElement(_mousePosition);

            if (nowHovering != _isHovering)
            {
                _isHovering = nowHovering;
                _hoverTimer = _isHovering ? hoverDelay : 0f;

                if (!_isHovering)
                {
                    HideInfoPanel();
                }
            }

            if (_isHovering)
            {
                if (_hoverTimer > 0)
                {
                    _hoverTimer -= Time.deltaTime;
                    if (_hoverTimer <= 0)
                    {
                        ShowInfoPanel();
                    }
                }
            }
        }

        private bool IsPointerOverUIElement(Vector2 mousePosition)
        {
            // 使用GraphicRaycaster替代EventSystem
            GraphicRaycaster raycaster = GetComponentInParent<GraphicRaycaster>();
            if (raycaster == null) return false;

            PointerEventData eventData = new PointerEventData(EventSystem.current);
            eventData.position = mousePosition;

            List<RaycastResult> results = new List<RaycastResult>();
            raycaster.Raycast(eventData, results);

            foreach (var result in results)
            {
                if (result.gameObject == gameObject)
                {
                    return true;
                }
            }

            return false;
        }

        private void OnMouseMove(Vector2 obj)
        {
            _mousePosition = obj;
        }

        private void ShowInfoPanel()
        {
            infoPanel.SetActive(true);
        }

        private void HideInfoPanel()
        {
            infoPanel.SetActive(false);
        }
    }
}
