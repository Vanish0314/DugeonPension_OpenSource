using System;
using System.Reflection;
using System.Text.RegularExpressions;
using Dungeon.DungeonGameEntry;
using Dungeon.Gal;
using GameFramework;
using UnityEngine;

namespace Dungeon
{
    public class DtoMessageParser : MonoBehaviour
    {
        public void GalParser(string message)
        {
            if (string.IsNullOrWhiteSpace(message)) return;

            // 匹配格式:-FunctionName=123 or -FunctionName
            var matches = Regex.Matches(message, @"-(\w+)(?:=([\-]?\d+))?");

            if (matches.Count == 0)
            {
                GameFrameworkLog.Error($"[GalParser] 传入参数无法解析,请检查: {message}");
                return;
            }

            try
            {
                Type galSysType = typeof(GalSystem);
                var galSys = DungeonGameEntry.DungeonGameEntry.GalSystem;

                foreach (Match match in matches)
                {
                    string functionName = match.Groups[1].Value;
                    string valueStr = match.Groups[2].Value;

                    if (string.IsNullOrEmpty(functionName))
                        continue;

                    if (string.IsNullOrEmpty(valueStr))
                    {
                        var method = galSysType.GetMethod(functionName,
                            BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public
                        );
                        if (method != null)
                            method.Invoke(galSys, null);
                        else
                            GameFrameworkLog.Error($"[GalParser]: 未找到方法 {functionName}");
                    }
                    else
                    {
                        if (int.TryParse(valueStr, out int intValue))
                        {
                            var method = galSysType.GetMethod(
                                functionName,
                                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public,
                                null,
                                new[] { typeof(int) },
                                null
                            );

                            if (method != null)
                                method.Invoke(galSys, new object[] { intValue });
                            else
                                GameFrameworkLog.Error($"[GalParser]: 未找到方法 {functionName} 或参数不匹配");
                        }
                        else
                        {
                            GameFrameworkLog.Error($"[GalParser]: 参数不是有效整数: {valueStr}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GameFrameworkLog.Error($"[GalParser]: 执行指令出错: {ex.Message}");
            }
        }

    }
}
