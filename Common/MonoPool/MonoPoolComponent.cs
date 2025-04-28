using System.Collections;
using System.Collections.Generic;
using Dungeon.Common.MonoPool;
using GameFramework.ObjectPool;
using UnityEngine;

namespace Dungeon
{
	public class MonoPoolComponent : MonoBehaviour
	{
		[SerializeField] private MonoPoolItem m_ItemTemplate = null;
		[SerializeField] private Transform m_InstanceRoot = null;
		[SerializeField] private int m_InstancePoolCapacity = 5;

		private IObjectPool<MonoPoolObject> m_ItemObjectPool = null;

		public void Init(string name, MonoPoolItem templete, Transform root, int capacity)
		{
			m_ItemTemplate = templete;
			m_InstanceRoot = root;
			m_InstancePoolCapacity = capacity;
			m_ItemObjectPool = GameEntry.ObjectPool.CreateSingleSpawnObjectPool<MonoPoolObject>(name, m_InstancePoolCapacity); //TODO(vanish): find if exist
		}

		public MonoPoolItem GetItem(object data)
		{
			var item = GenItem(data);
			item.OnSpawn(data);
			return item;
		}

		public void ReturnItem(MonoPoolItem item)
		{
			item.Reset();
			m_ItemObjectPool.Unspawn(item);
		}

		private MonoPoolItem GenItem(object data)
		{
			MonoPoolItem cellItem;
			MonoPoolObject cellItemObject = m_ItemObjectPool.Spawn();
			if (cellItemObject != null)
			{
				cellItem = (MonoPoolItem)cellItemObject.Target;
			}
			else
			{
				cellItem = Instantiate(m_ItemTemplate);
				cellItem.FirstInit(data, this);
				Transform transf = cellItem.GetComponent<Transform>();
				transf.SetParent(m_InstanceRoot);
				transf.localScale = Vector3.one;
				m_ItemObjectPool.Register(MonoPoolObject.Create(cellItem), true);
			}
			cellItem.gameObject.SetActive(true);
			return cellItem;
		}
	}
}


/*
Usage Case:

```csharp
monoPool = gameObject.AddComponent<MonoPoolComponent>();
monoPool.Init(<poolName>, <prefab>, <rootTransform>, <capacity>);

var go = monoPool.GetItem(data);
```

*/

