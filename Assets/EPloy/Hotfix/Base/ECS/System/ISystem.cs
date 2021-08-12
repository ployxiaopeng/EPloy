using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISystem
{
    void Start();

    void Update();

    void OnDestroy();
}
