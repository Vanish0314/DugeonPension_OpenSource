using System;
using System.Collections;
using System.Collections.Generic;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class RoomLimitFormController : MonoBehaviour
    {
        [SerializeField] private RoomLimitForm roomLimitForm;
        [SerializeField] private InputReader inputReader;
        
        private Camera m_camera;
        private Vector3 _currentMousePosition;
        
        private void OnEnable()
        {
            Camera[] allCameras = FindObjectsOfType<Camera>(false);
            foreach (var activeCamera in allCameras)
            {
                if (activeCamera.isActiveAndEnabled)
                {
                    m_camera = activeCamera;
                    GameFrameworkLog.Info(m_camera);
                    break;
                }
            }
            
            inputReader.OnMouseMoveEvent += OnMouseMove;
        }

        private void OnDisable()
        {
            inputReader.OnMouseMoveEvent -= OnMouseMove;
        }

        private void OnMouseMove(Vector2 obj)
        {
            _currentMousePosition = m_camera.ScreenToWorldPoint(obj);
        }

        private void Update()
        {
            DungeonGameEntry.DungeonGameEntry.GridSystem.GetMonoRoomAt(_currentMousePosition,out var room);
            roomLimitForm.UpdateForm(room);
        }
    }
}
