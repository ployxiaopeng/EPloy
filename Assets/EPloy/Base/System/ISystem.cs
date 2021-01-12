using UnityEngine;
using UnityEditor;
using System;

namespace EPloy
{
    public interface ISystem
    {
        Type Type();

        void Run(IComponent component);
    }
}