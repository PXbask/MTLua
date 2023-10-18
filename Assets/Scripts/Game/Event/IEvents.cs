using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MT.Event
{
    public enum EventObjectType
    {
        None = 0,
        Empty = 1 << 0,
        Stair = 1 << 1,
        Enemy = 1 << 2,
        Item = 1 << 3,
        Door = 1 << 4,
    }


    interface IEventBase
    {
        int GetEntityType();
        bool Belong(EventObjectType t);
        bool CanMove();
        UnityEngine.GameObject GetGameObject();
        string GetAssetName();
        int GetItemID();
    }

    /// <summary>
    /// 一般的可行走区域
    /// </summary>
    interface IOverlay : IEventBase
    {
        /// <summary>
        /// 玩家即将进入此块
        /// </summary>
        void OnbeforeOverlay();
        /// <summary>
        /// 当玩家站在此块上
        /// </summary>
        void OnOverlay();
        /// <summary>
        /// 玩家离开此块
        /// </summary>
        void OnafterOverlay();
    }

    /// <summary>
    /// 传送点
    /// </summary>
    interface IChangeFloor : IEventBase
    {
        /// <summary>
        /// 玩家即将进入传送点
        /// </summary>
        void OnbeforeChangeFloor();
        /// <summary>
        /// 玩家进入传送点，通常执行传送逻辑
        /// </summary>
        void OnChangeFloor();
    }

    /// <summary>
    /// Enemy
    /// </summary>
    interface IBattleable : IEventBase
    {
        void OnbeforeBattle();
        void OnafterBattle();
        int GetHp();
        int GetAtk();
        int GetDef();
        bool CanBattle();
    }

    /// <summary>
    /// Item
    /// </summary>
    interface IGetItem : IEventBase
    {
        void OnbeforeGetItem();
        void OnafterGetItem();
        bool EffectAtOnce();
        void OnUseItem();
    }

    /// <summary>
    /// Door
    /// </summary>
    interface IOpenDoor : IEventBase
    {
        void OnbeforeOpenDoor();
        void OnafterOpenDoor();
    }

    interface IEventBasic : IOverlay, IChangeFloor, IBattleable, IGetItem, IOpenDoor { }
}
