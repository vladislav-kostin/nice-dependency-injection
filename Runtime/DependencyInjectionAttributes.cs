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

// Dynamic

// [AttributeUsage(AttributeTargets.Class)]
// public class InjectableAttribute : Attribute
// {
// 	public InjectionType InjectionType;
//
// 	public InjectableAttribute()
// 	{
// 		InjectionType = InjectionType.Static;
// 	}
//
// 	public InjectableAttribute(InjectionType injectionType)
// 	{
// 		InjectionType = injectionType;
// 	}
//
// 	public bool IsDynamic => InjectionType == InjectionType.Dynamic;
// }
//
// public enum InjectionType
// {
// 	/// (Default) Produces an error if null when injected or if assigned multiple times.
// 	Static,
//
// 	/// Does not produce an error if null when injected or if assigned multiple times.
// 	Dynamic
// }

// Dynamic, Required, Override

// [AttributeUsage(AttributeTargets.Class)]
// public class InjectableAttribute : Attribute
// {
// 	public Type BindingType;
// 	public InjectionType InjectionType;
//
// 	public InjectableAttribute()
// 	{
// 		InjectionType = InjectionType.RequiredStatic;
// 	}
//
// 	public InjectableAttribute(InjectionType injectionType, Type bindingType = null)
// 	{
// 		InjectionType = injectionType;
// 		BindingType = bindingType;
// 	}
//
// 	public InjectableAttribute(Type bindingType)
// 	{
// 		BindingType = bindingType;
// 	}
//
// 	internal bool IsRequired()
// 	{
// 		return ((int)InjectionType & (int)InjectionFlags.Required) != 0;
// 	}
//
// 	internal bool IsStatic()
// 	{
// 		return ((int)InjectionType & (int)InjectionFlags.Static) != 0;
// 	}
//
// 	internal bool IsOverridable()
// 	{
// 		return ((int)InjectionType & (int)InjectionFlags.Overridable) != 0;
// 	}
// }

// [Flags]
// internal enum InjectionFlags
// {
// 	/// Will produce error if not bound when requested
// 	Required = 1 << 0,
//
// 	/// Will produce error if changed to null or another value
// 	Static = 1 << 1,
//
// 	/// Will not produce error if assigned while having a non-null value
// 	Overridable = 1 << 2
// }
//
// public enum InjectionType
// {
// 	/// Required injection will produce error if not bound when injected.
// 	/// Static can only be bound once.
// 	RequiredStatic = InjectionFlags.Required | InjectionFlags.Static,
//
// 	/// Required injection will produce error if not bound when injected.
// 	/// Dynamic injection can be rebound or unbound after it is bound initially.
// 	RequiredDynamic = InjectionFlags.Required,
//
// 	/// Required injection will produce error if not bound when injected.
// 	/// Dynamic injection can be rebound or unbound after it is bound initially.
// 	/// Overridable injection will not produce error if assigned while having a non-null value.
// 	RequiredDynamicOverridable = InjectionFlags.Required | InjectionFlags.Overridable,
//
// 	/// Optional injection can be null or not bound when injected.
// 	/// Static can only be bound once.
// 	OptionalStatic = InjectionFlags.Static,
//
// 	/// Optional injection value can be null or not bound when injected.
// 	/// Dynamic injection can be rebound or unbound after it is bound initially.
// 	OptionalDynamic = 0,
//
// 	/// Optional injection value can be null or not bound when injected.
// 	/// Dynamic injection can be rebound or unbound after it is bound initially.
// 	/// Overridable injection will not produce error if assigned while having a non-null value.
// 	OptionalDynamicOverridable = InjectionFlags.Overridable
// }

// public class InjectableReference<T> : InjectableReference
// {
// 	internal InjectableReference(T value)
// 	{
// 		Value = value;
// 	}
//
// 	public T Value { get; internal set; }
// }
//
// public class InjectableReference
// {
// }