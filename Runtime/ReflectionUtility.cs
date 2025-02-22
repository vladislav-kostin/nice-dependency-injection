using System;
using System.Collections.Generic;
using System.Reflection;

public static class ReflectionUtility
{
	private static readonly Dictionary<Type, TypeInfo> _typeInfoByType = new();
	private static readonly Dictionary<Type, FieldInfo[]> _fieldInfoByType = new();

	public static TypeInfo GetTypeInfo(Type type)
	{
		if (!_typeInfoByType.TryGetValue(type, out var typeInfo))
		{
			typeInfo = type.GetTypeInfo();
			_typeInfoByType[type] = typeInfo;
		}

		return typeInfo;
	}

	public static FieldInfo[] GetCachedFieldInfo(Type type)
	{
		if (!_fieldInfoByType.TryGetValue(type, out var fields))
		{
			fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
			_fieldInfoByType[type] = fields;
		}

		return fields;
	}
}