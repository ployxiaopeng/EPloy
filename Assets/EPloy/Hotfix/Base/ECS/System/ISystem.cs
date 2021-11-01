using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EPloy
{
    public interface ISystem
    {
        int Priority { get; } //待定

        bool IsPause { get; set; }

        void Start();

        void Update();

        void OnDestroy();
    }
}
