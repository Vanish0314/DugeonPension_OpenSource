using System.Collections;
using System.Collections.Generic;
using Dungeon.Common;
using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;

namespace Dungeon.Common
{
    public class MonoPoolObject : ObjectBase
	{
		public static MonoPoolObject Create(object target)
		{
			MonoPoolObject cellItemObject = ReferencePool.Acquire<MonoPoolObject>();
			cellItemObject.Initialize(target);
			return cellItemObject;
		}
		protected override void Release(bool isShutdown)
		{
			MonoPoolItem cellItem = (MonoPoolItem)Target;
			if (cellItem == null)
				return;
			Object.Destroy(cellItem.gameObject);
		}
	}
}
