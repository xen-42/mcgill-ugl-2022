using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IComponentReveiver<T> where T : Component
{
    public T AddComponent(T component);
}