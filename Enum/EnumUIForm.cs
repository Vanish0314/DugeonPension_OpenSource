using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Dungeon
{
    public enum EnumUIForm 
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,
        
        /// <summary>
        /// 测试界面
        /// </summary>
        ResourceFrom = 1001,
        
        /// <summary>
        /// 建造界面
        /// </summary>
        BuildForm = 1002,
        
        /// <summary>
        /// 倒计时
        /// </summary>
        TimelineForm = 1003,
        
        /// <summary>
        /// 游戏开始界面
        /// </summary>
        GameStartForm = 1004,
        
        /// <summary>
        /// 放置界面
        /// </summary>
        PlaceArmyForm = 1005,
        
        /// <summary>
        /// 勇者信息界面
        /// </summary>
        HeroInfoForm = 1006,
        
        /// <summary>
        /// 勇者进驻菜单界面
        /// </summary>
        HeroMenuForm = 1007,
        
        /// <summary>
        /// 战斗开始按钮
        /// </summary>
        StartFightButtonForm = 1008,
        
        /// <summary>
        /// 经营结算界面
        /// </summary>
        BusinessSettlementForm = 1009,
        
        /// <summary>
        /// 战斗结算界面
        /// </summary>
        FightSettlementForm = 1010,
        
        /// <summary>
        /// 返回经营按钮
        /// </summary>
        ReturnBusinessButton = 1011,
        
        /// <summary>
        /// 诅咒界面
        /// </summary>
        CurseForm = 1012,
    }
}
