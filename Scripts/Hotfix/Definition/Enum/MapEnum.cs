namespace ETHotfix
{
    /// <summary>
    /// 事件组类型
    /// </summary>
    public enum EventGroupType
    {
        QueEvent,
        GeneralEvent,
        TransferEvent
    }

    /// <summary>
    /// 触发条件类型
    /// </summary>
    public enum ConditionType
    {
        EventStatus,                             //指定事件的状态
        EventGroupStatus,                    //指定事件组的状态
        HasPropId,                                 //拥有指定道具ID
        KillEnemyId,                               //击杀敌人ID
        HasPartnerId,                             //拥有伙伴ID
        HasStateId,                                //拥有状态ID
        TakeInteraction,                          //发生交互
    }
    /// <summary>
    /// 地图事件类型
    /// </summary>
    public enum MapEventType
    {
        None,
        Plot,                                  //触发剧情 
        AnimePlot,                        //触发动画剧情 
        Lantern,                            //触发视野的变化
        Battle,                               //触发战斗  
        GetItem,                           //触发获得物品  
        GetPartner,                       //触发获得伙伴
        SelectUI,                           //触发选择交互
        Attrbute,                           //触发元素属性调整  
        EqualEvt,                          //触发平行事件
        DragEvt,                           //触发拖拽事件
    }
    /// <summary>
    /// 事件状态类型
    /// </summary>
    public enum EventStatus
    {
        State1 = 0,                //激活未完成中间状态
        State2 = 1,                //激活未完成中间状态
        State3,
        State4,
        State5,
        State8,
        NoActive,            //未激活状态
        ActiveCanRepeat,   //激活但已完成状态 可重复触发
        ActiveNoComplete, //激活但已完成状态
        Complete             //完成状态 不可重复触发
    }
    /// <summary>
    /// 事件属性调整类型
    /// </summary>
    public enum AttributeType
    {
        Pass,
        Direction,
        Res,
        Effect,
        Model,
        BackHome,
        EventStatus,
        EventGroupStatus,
        Event,
    }
    /// <summary>
    /// 事件操作类型
    /// </summary>
    public enum OperationEvt
    {
        None,
        Hand = 130212002,
        Eye = 130212001,
        key = 130212003,
        Axe = 130212004,
        Plot = 130212005,
        Battle = 130212006,
    }
    /// <summary>
    /// 剧情类别
    /// </summary>
    public enum PlotType
    {
        DialogMsg,
        SystemMsg,
        SpecialMsg
    }

    /// <summary>
    /// 传送类型
    /// </summary>
    public enum TransferType
    {
        OrientTransfer,
        MultipleTransfer,
        PathFindingTransfer
    }

    /// <summary>
    /// 拖拽物品类型
    /// </summary>
    public enum DragWeightType
    {
        Random,
        Queue,
    }
}