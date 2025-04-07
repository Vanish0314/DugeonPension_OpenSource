using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;

namespace Dungeon.AgentLowLevelSystem
{
    public static class AgentBlackBoardEnum
    {
        #region 自身
        public static readonly string MonstersInDungeon = "MonstersInDungeon";
        public static readonly string CurrentHP = "CurrentHP";
        public static readonly string CurrentMP = "CurrentMP";
        public static readonly string HpMax = "HpMax";
        public static readonly string MpMax = "MpMax";
        public static readonly string CurrentSan = "CurrentSan";
        public static readonly string SanMax = "SanMax";
        public static readonly string AttackSpeed = "AttackSpeed";
        public static readonly string FireResistance = "FireResistance";
        public static readonly string IceResistance = "IceResistance";
        public static readonly string HolyResistance = "HolyResistance";
        public static readonly string PosionResistance = "PosionResistance";
        #endregion

        #region 环境感知

        #region 视野
        /// <summary>
        /// For runtime performance, we won't check whether it is valid 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Name like : "NumOfIVisibleInVision"</returns>
        public static string GetNameOfIVisibleCountInVision<T>() where T : IVisible
        {
#if UNITY_EDITOR
            var leafTypes = GetLeafTypesImplementingInterface<IVisible>();

            bool flag = true;
            foreach (var leafType in leafTypes)
            {
                if (leafType == typeof(T))
                    flag = false;
            }

            if (flag)
            {
                GameFrameworkLog.Error("[AgentBlackBoardEnum] 传入对象为非叶子类型");
            }
#endif

            var builder = new StringBuilder(30);
            builder.Append("NumOf").Append(typeof(T).Name).Append("InVision");

            return builder.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName">the leaf type name</param>
        /// <returns>Name like : "NumOfIVisibleInVision"</returns>
        public static string GetNameOfIVisibleCountInVision(string typeName)
        {
#if UNITY_EDITOR
            var leafTypes = GetLeafTypesImplementingInterface<IVisible>();

            bool flag = true;
            foreach (var leafType in leafTypes)
            {
                if (leafType.Name == typeName)
                    flag = false;
            }

            if (flag)
                GameFrameworkLog.Error("[AgentBlackBoardEnum] 传入对象为非叶子类型");
#endif

            var builder = new StringBuilder(30);
            builder.Append("NumOf").Append(typeName).Append("InVision");

            return builder.ToString();
        }

        /// <summary>
        /// For runtime performance, we won't check whether it is valid 
        /// </summary>
        /// <returns>Name like : "NumOfIDungeonBehaviorInMemory"</returns>
        [Obsolete("Don't use for that it's not considered fully")]
        public static string GetNameOfIVisibleCountInAgentBrainMemory<T>() where T : IVisible
        {
#if UNITY_EDITOR

            var leafTypes = GetLeafTypesImplementingInterface<IVisible>();

            bool flag = true;
            foreach (var leafType in leafTypes)
            {
                if (leafType == typeof(T))
                    flag = false;
            }

            if (flag)
            {
                GameFrameworkLog.Error("[AgentBlackBoardEnum] 传入对象为非叶子类型");
            }
#endif

            var builder = new StringBuilder(40);
            builder.Append("NumOf").Append(typeof(T).Name).Append("InMemory");

            return builder.ToString();
        }
        #endregion

#region Transform
        [Obsolete("Don't use for that it's not considered fully")]
        public static string GetNameOfIVisibleTransformInVision<T>()
        {
            throw new NotImplementedException();
        }
#endregion

        #endregion


        public static List<Type> GetLeafTypesImplementingInterface<TInterface>()
        {
            var allTypes = AppDomain.CurrentDomain
                .GetAssemblies()
                .SelectMany(a => a.GetTypes())
                .Where(t => typeof(TInterface).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract)
                .ToList();

            var baseTypes = new HashSet<Type>(allTypes.SelectMany(t => t.BaseType != null ? new[] { t.BaseType } : Array.Empty<Type>()));

            return allTypes.Where(t => !baseTypes.Contains(t)).ToList();
        }
    }
}
