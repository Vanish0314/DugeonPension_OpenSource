using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public class DungeonEntityManager : MonoBehaviour
    {
        public void EnableAllDungeonEntities()
        {
            foreach (var entity in m_ManagedEntities)
            {
                entity.enabled = true;
            }
        }
        public void DisableAllDungeonEntities()
        {
            foreach (var entity in m_ManagedEntities)
            {
                entity.enabled = false;
            }
        }

        public void RegisterDungeonEntity(DungeonEntity.DungeonEntity entity)
        {
            m_ManagedEntities.Add(entity);
        }

        public void UnregisterDungeonEntity(DungeonEntity.DungeonEntity entity)
        {
            m_ManagedEntities.Remove(entity);
        }
        private List<DungeonEntity.DungeonEntity> m_ManagedEntities = new ();
    }
}
