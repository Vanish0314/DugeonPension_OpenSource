using System.Collections;
using System.Collections.Generic;
using PlasticGui.WorkspaceWindow;
using UnityEngine;
using UnityEngine.Pool;

namespace Dungeon.Common.MonoPool
{
	/// <summary>
	/// 1. 初始化尽量使用OnEnable
	/// </summary>
	public abstract class MonoPoolItem : MonoBehaviour
	{
		public bool isInPool = true;
		protected MonoPoolComponent m_Owner;
		protected object mData;
		public MonoPoolItem Duplicate()
		{
			return m_Owner.GetItem(mData);
		}

		protected virtual void Awake() {OnAwake();} // 请不要使用Awake来初始化，请使用Init来初始化
		protected virtual void Start() { } // 请不要使用Start来初始化，请使用Init来初始化
		protected virtual void OnDestroy() { } // 请不要使用OnDestroy来清理，请使用ReturnToPool来清理
		public void FirstInit(object data, MonoPoolComponent owner)
		{
			m_Owner = owner;
			mData = data;
			isInPool = false;
		}
		public void ReturnToPool()
		{
			isInPool = true;
			mData = null;
			OnReturnToPool();
			gameObject.SetActive(false);
			m_Owner.ReturnItem(this);
		}
		public abstract void OnSpawn(object data); /// 初始化物体
		public abstract void Reset();

		/// 重置物体
		public virtual void OnReturnToPool()
		{
			this.gameObject.SetActive(false);
		}/// 物体回收时调用
		protected virtual void OnEnable(){}/// 物体激活时调用,而不是从池子中取出时调用
		protected virtual void OnDisable(){}/// 物体禁用时调用,而不是回到池子调用
		protected virtual void OnAwake(){}/// Awake时调用
	}
}