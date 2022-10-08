using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy.ECS
{

    public enum BTResult
    {
        Inactive = 0,
        Failure = 1,
        Success = 2,
        Running = 3
    }

    public enum ComPositesType
    {
        Sequence, //Ë³Ðò
        Selector,//Ñ¡Ôñ
        Parallel//Æ½ÐÐ
    }
}