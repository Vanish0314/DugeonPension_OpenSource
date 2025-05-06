using UnityEngine;

namespace Dungeon
{
    // 数据类接口统一
    public interface IPlaceableData 
    {
        Vector2Int GetSize();
    }
}