using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Dungeon.Common.MonoPool;
using Dungeon.GridSystem;
using GameFramework;
using GameFramework.ObjectPool;
using UnityEngine;

namespace Dungeon.DungeonEntity
{

    /// <summary>
    /// 注意: 任何在场景中的实例对象都必须是叶子类型!!!
    /// 任何继承此接口的类都必须是DungeonBehavior子类
    /// 继承DungeonEntity而非此接口
    /// </summary>
    public interface IDungeonEntity
    {
    }
    /// <summary>
    /// 注意: 任何在场景中的实例对象都必须是叶子类型!!!
    /// </summary>
    public abstract class DungeonEntity : MonoPoolItem, IDungeonEntity
    {
        private Dictionary<ScriptableObject, ScriptableObject> originalSoMap = new();
        private List<FieldRecord> fieldRecords = new();

        private struct FieldRecord
        {
            public FieldInfo Field;
            public ScriptableObject OriginalSo;
            public object Component;

            public FieldRecord(FieldInfo field, ScriptableObject originalSo, object component)
            {
                Field = field;
                OriginalSo = originalSo;
                Component = component;
            }
        }

        public override void OnReturnToPool()
        {
            base.OnReturnToPool();
            RecoverySoToOriginal();
        }

        protected void RecoverySoToOriginal()
        {
            foreach (var record in fieldRecords)
            {
                var field = record.Field;
                var originalSo = record.OriginalSo;
                var component = record.Component;

                var clone = ScriptableObject.Instantiate(originalSo);
                field.SetValue(component, clone);

#if UNITY_EDITOR
                LogClonedSO(field.Name, originalSo, component);
#endif
            }
        }

        protected sealed override void OnAwake()
        {
            base.OnAwake();

            var components = GetComponents<MonoBehaviour>();
            foreach (var component in components)
            {
                if (component == null) continue;

                var type = component.GetType();
                var asmName = type.Assembly.GetName().Name;

                if (asmName == "DungeonScripts")
                {
                    DeepCloneScriptableObjects(component);
                }
            }
        }

        private void DeepCloneScriptableObjects(object target)
        {
            if (target == null) return;

            var fields = target.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var field in fields)
            {
                object value = field.GetValue(target);
                if (value == null) continue;

                // 1. ScriptableObject
                if (value is ScriptableObject so)
                {
                    var clone = GetOrClone(so, field.Name, target);
                    field.SetValue(target, clone);

                    originalSoMap[so] = so;
                    fieldRecords.Add(new FieldRecord(field, so, target));
                }
                // 2. List<ScriptableObject>
                else if (value is IList list && field.FieldType.IsGenericType)
                {
                    var elementType = field.FieldType.GetGenericArguments()[0];
                    if (typeof(ScriptableObject).IsAssignableFrom(elementType))
                    {
                        var clonedList = (IList)Activator.CreateInstance(field.FieldType);
                        foreach (var item in list)
                        {
                            if (item is ScriptableObject itemSO)
                            {
                                var clone = GetOrClone(itemSO, field.Name, target);
                                clonedList.Add(clone);
                            }
                            else
                            {
                                clonedList.Add(item);
                            }
                        }
                        field.SetValue(target, clonedList);
                    }
                }
                // 3. Dictionary<string, ScriptableObject>
                else if (value is IDictionary dict && field.FieldType.IsGenericType)
                {
                    var args = field.FieldType.GetGenericArguments();
                    var keyType = args[0];
                    var valueType = args[1];

                    if (keyType == typeof(string) && typeof(ScriptableObject).IsAssignableFrom(valueType))
                    {
                        var clonedDict = (IDictionary)Activator.CreateInstance(field.FieldType);
                        foreach (DictionaryEntry entry in dict)
                        {
                            if (entry.Value is ScriptableObject vso)
                            {
                                var clone = GetOrClone(vso, field.Name, target);
                                clonedDict[entry.Key] = clone;
                            }
                            else
                            {
                                clonedDict[entry.Key] = entry.Value;
                            }
                        }
                        field.SetValue(target, clonedDict);
                    }
                }
            }
        }

        private ScriptableObject GetOrClone(ScriptableObject original, string containerFieldName, System.Object component)
        {
            if (original == null) return null;

            if (originalSoMap.TryGetValue(original, out var existing))
                return existing;

            var clone = ScriptableObject.Instantiate(original);
            originalSoMap[original] = clone;

#if UNITY_EDITOR
            LogClonedSO(containerFieldName, original, component);
#endif

            DeepCloneScriptableObjects(clone);
            return clone;
        }

        private void LogClonedSO(string containerFieldName, ScriptableObject original, System.Object component)
        {
            string scriptName = component.GetType().Name;
            GameFrameworkLog.Info($"[Cloned SO] Name: {original.name}, Type: {original.GetType().Name}, From Field: {containerFieldName}, Script: {scriptName}", original);
            GameFrameworkLog.Info("[Cloned SO] SO拷贝只支持三种类型: ScriptableObject, List<ScriptableObject>, Dictionary<string, ScriptableObject>");
        }
    }
}