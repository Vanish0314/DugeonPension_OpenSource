using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow;
using UnityEngine;
using UnityEngine.Pool;

namespace Dungeon.Common.MonoPool
{
	public abstract class MonoPoolItem : MonoBehaviour
	{
		private MonoPoolComponent m_Owner;
		public void Init(object data,MonoPoolComponent owner)
		{
			m_Owner = owner;
			Init(data);
		}
        public void ReturnToPool()
		{
			m_Owner.ReturnItem(this);
		}
		public abstract void Init(object data);
		public abstract void Reset();
    }
}