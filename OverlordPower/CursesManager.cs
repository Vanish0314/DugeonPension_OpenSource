using System.Collections;
using System.Collections.Generic;
using Dungeon.Character.Hero;
using Dungeon.Overload;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class CursesManager : MonoBehaviour
    {
        public InputReader inputReader;
        
        private CurseType m_SelectedCurse;
        
        private Vector3 m_CurrentMouseWorldPos;
        private HeroEntityBase m_SelectedHero;
        public static CursesManager Instance { get; private set; }
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            transform.position = Vector3.zero;
        }
        
        public void Initialize()
        {
            inputReader = Resources.Load<InputReader>("InputReader");
            inputReader.Subscribe();
        }

        #region PublicAPI

        public void SelectCurse(CurseType curseType)
        {
            var curse = DungeonGameEntry.DungeonGameEntry.OverloadPower.CurseList.Find(x => x.curseType == curseType);
            if (curse != null)
            {
                m_SelectedCurse = curseType;
                if (m_SelectedCurse == CurseType.Redeploy)
                {
                    DungeonGameEntry.DungeonGameEntry.OverloadPower.SpellCurse(null, m_SelectedCurse);
                    return;
                }
                StartCurseProcess();
            }
            else
            {
                GameFrameworkLog.Error($"找不到类型为 {curseType} 的诅咒");
            }
        }

        #endregion

        #region CurseLogic

        private void StartCurseProcess()
        {
            CancelCurse();
            inputReader.SetPlaceActions();
            RegisterInputEvents();
        }
        
        private void RegisterInputEvents()
        {
            inputReader.OnPlacePositionEvent += OnMouseMoved;
            inputReader.OnTryPlaceEvent += TryCurse;
            inputReader.OnCancelPlaceEvent += CancelCurse;
        }

        private void CancelCurse()
        {
            inputReader.OnPlacePositionEvent -= OnMouseMoved;
            inputReader.OnTryPlaceEvent -= TryCurse;
            inputReader.OnCancelPlaceEvent -= CancelCurse;
            
            inputReader.SetUIActions();
        }

        private void TryCurse()
        {
            m_SelectedHero = GetHeroAtPosition(m_CurrentMouseWorldPos);

            ExecuteCurse();
        }

        #endregion
        
        private HeroEntityBase GetHeroAtPosition(Vector3 worldPosition)
        {
            int heroLayer = LayerMask.GetMask("Hero");
            var hits = Physics2D.OverlapCircleAll(worldPosition, 0.5f, heroLayer);
    
            foreach (var hit in hits)
            {
                var hero = hit.GetComponent<HeroEntityBase>();
                if (hero != null) return hero;
            }
            return null;
        }

        private void ExecuteCurse()
        {
            if (m_SelectedHero == null) return;
            
            // 施放诅咒
            DungeonGameEntry.DungeonGameEntry.OverloadPower.SpellCurse(
                m_SelectedHero, 
                m_SelectedCurse
            );
            
            // 重置选择
            CancelCurse();
            GameEntry.UI.GetUIForm(EnumUIForm.CurseForm).Close();
        }
        
        private void OnMouseMoved(Vector2 mouseScreenPos)
        {
            Camera[] allCameras = FindObjectsOfType<Camera>(false);
            foreach (var activeCamera in allCameras)
            {
                if (activeCamera.isActiveAndEnabled)
                {
                    // 将屏幕坐标转换为世界坐标
                    m_CurrentMouseWorldPos = activeCamera.ScreenToWorldPoint(mouseScreenPos);
                }
            }
            m_CurrentMouseWorldPos.z = 0; // 确保z坐标为0
        }
    }
}
