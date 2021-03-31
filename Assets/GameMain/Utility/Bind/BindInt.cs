using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace EPloy
{
    /// <summary>
    /// 绑定整数
    /// </summary>
    public class BindInt : BindValue<int>
    {

        public BindInt()
        {

        }

        public BindInt(int value) : base(value)
        {

        }

    }
}

