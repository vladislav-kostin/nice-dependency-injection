using System;
using UnityEngine;

[AttributeUsage(AttributeTargets.Field)]
public class InjectAttribute : Attribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class InjectFromPrefabAttribute : PropertyAttribute
{
}

[AttributeUsage(AttributeTargets.Field)]
public class InjectFromGameObjectAttribute : PropertyAttribute
{
}

[AttributeUsage(AttributeTargets.Class)]
public class InjectableAttribute : Attribute
{
}