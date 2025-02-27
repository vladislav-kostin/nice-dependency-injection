using System;
using System.Collections.Generic;
using System.Reflection;
using NiceDependencyInjection.Utility;
using UnityEngine;
using Object = UnityEngine.Object;

namespace NiceDependencyInjection
{
	public class DependencyInjectionManager
	{
		private static readonly Dictionary<Type, object> _injectableObjectsByType = new();
		private static readonly Dictionary<Type, FieldInfo[]> _injectableFieldsByType = new();
		private static readonly List<FieldInfo> __injectableFieldsBuffer = new(8);

		public static bool IsInitialized { get; private set; }
		public static event Action OnInitialized;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		public static void Initialize()
		{
			_injectableObjectsByType.Clear();
			_injectableFieldsByType.Clear();
			ProcessScene();
		}

		public static void ProcessScene()
		{
			var allMonoBehaviours = new List<object>(64);
			allMonoBehaviours.AddRange(Object.FindObjectsOfType<MonoBehaviour>(true));
			InjectObjects(allMonoBehaviours);
			InjectIntoObjects(allMonoBehaviours);
			IsInitialized = true;
			OnInitialized?.Invoke();
			OnInitialized = null;
		}

		private static void InjectObjects(List<object> objects)
		{
			foreach (var @object in objects)
			{
				InjectObject(@object);
			}
		}

		private static void InjectIntoObjects(List<object> objects)
		{
			foreach (var @object in objects)
			{
				InjectIntoObject(@object);
			}
		}

		/// Perform before InjectIntoObject
		public static void InjectObject(object target)
		{
			var targetType = target.GetType();
			var typeProcessed = _injectableObjectsByType.TryGetValue(targetType, out var injectableObject);

			if (!typeProcessed)
			{
				if (Attribute.IsDefined(targetType, typeof(InjectableAttribute)))
				{
					injectableObject = target;
					_injectableObjectsByType[targetType] = injectableObject;
				}
				else
				{
					_injectableObjectsByType[targetType] = null;
				}
			}

			if (injectableObject != null && injectableObject != target)
			{
				Debug.LogError($"Conflict detected: Multiple instances of injectable type {targetType}");
			}
		}

		/// Perform after InjectObject
		public static void InjectIntoObject(object target)
		{
			var targetType = target.GetType();
			var typeProcessed = _injectableFieldsByType.TryGetValue(targetType, out var injectableFields);

			if (!typeProcessed)
			{
				__injectableFieldsBuffer.Clear();
				var fields = ReflectionUtility.GetCachedFieldInfo(targetType);
				foreach (var field in fields)
				{
					var attributes = field.GetCustomAttributes(typeof(InjectAttribute), true);
					if (attributes.Length > 0)
					{
						__injectableFieldsBuffer.Add(field);
					}
				}

				injectableFields = __injectableFieldsBuffer.ToArray();
				_injectableFieldsByType[targetType] = injectableFields;
			}

			foreach (var field in injectableFields)
			{
				var fieldType = field.FieldType;
				_injectableObjectsByType.TryGetValue(fieldType, out var injectableObject);

				if (injectableObject != null)
				{
					field.SetValue(target, injectableObject);
				}
				else
				{
					Debug.LogError($"Injectable object of type {fieldType} requested by {targetType} is missing!");
				}
			}
		}
	}
}