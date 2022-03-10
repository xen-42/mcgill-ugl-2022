using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// For Non-Mono Behaviours which can hold a reference to Component <typeparamref name="T"/>
/// </summary>
/// <typeparam name="T">The type of the Component</typeparam>
public interface IComponentReveiver<T> where T : Component
{
    public T AddComponent(T component);
}