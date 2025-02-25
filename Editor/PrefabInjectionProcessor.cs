using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NiceDependencyInjection.Editor
{
	public class PrefabInjectionProcessor : AssetProcessorBase
	{
		private static readonly Dictionary<Type, TypeInfo> _typeToTypeInfo = new();
		private static readonly Dictionary<GameObject, int> _gameObjectToCachedObjectCount = new();

		[InitializeOnLoadMethod]
		private static void Initialize()
		{
			_includedTypes.Add(typeof(GameObject));

			_onImported = asset =>
			{
				var prefab = asset as GameObject;
				var allComponents = prefab.GetComponentsInChildren<Component>();

				// Early exit if component count did not change
				// Might be unreliable in rare edge cases
				if (_gameObjectToCachedObjectCount.TryGetValue(prefab, out var cachedCount))
				{
					if (allComponents.Length == cachedCount)
					{
						return;
					}
				}
				else
				{
					_gameObjectToCachedObjectCount.Add(prefab, allComponents.Length);
				}

				var allComponentsDictionary = new Dictionary<Type, Component>();
				foreach (var component in allComponents)
				{
					if (!allComponentsDictionary.ContainsKey(component.GetType()))
					{
						allComponentsDictionary.Add(component.GetType(), component);
					}
				}

				foreach (var component in allComponents)
				{
					if (component is MonoBehaviour script)
					{
						var scriptType = script.GetType();

						// Initialize TypeInfo if not already cached
						if (!_typeToTypeInfo.ContainsKey(scriptType))
						{
							var newTypeInfo = new TypeInfo();
							var fields = scriptType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

							foreach (var field in fields)
							{
								// Handle fields with [InjectFromPrefab] attribute
								if (field.GetCustomAttribute<InjectFromPrefabAttribute>() != null)
								{
									if (field.FieldType.IsSubclassOf(typeof(Component)))
									{
										// Single component field
										newTypeInfo.SingleComponentFieldsWithInjectFromPrefab ??= new List<FieldInfo>();
										newTypeInfo.SingleComponentFieldsWithInjectFromPrefab.Add(field);
									}
									else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
									         field.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(Component)))
									{
										// List of components field
										newTypeInfo.ListFieldsWithInjectFromPrefab ??= new List<FieldInfo>();
										newTypeInfo.ListFieldsWithInjectFromPrefab.Add(field);
									}
									else if (field.FieldType.IsArray && field.FieldType.GetElementType().IsSubclassOf(typeof(Component)))
									{
										// Array of components field
										newTypeInfo.ArrayFieldsWithInjectFromPrefab ??= new List<FieldInfo>();
										newTypeInfo.ArrayFieldsWithInjectFromPrefab.Add(field);
									}
									else
									{
										Debug.LogError($"Field {field.Name} in {scriptType.Name} must be a Component, List<Component>, or Component[] to use InjectFromPrefab", prefab);
									}
								}
								// Handle fields with [InjectFromGameObject] attribute
								else if (field.GetCustomAttribute<InjectFromGameObjectAttribute>() != null)
								{
									if (field.FieldType.IsSubclassOf(typeof(Component)))
									{
										// Single component field
										newTypeInfo.SingleComponentFieldsWithInjectFromGameObject ??= new List<FieldInfo>();
										newTypeInfo.SingleComponentFieldsWithInjectFromGameObject.Add(field);
									}
									else if (field.FieldType.IsGenericType && field.FieldType.GetGenericTypeDefinition() == typeof(List<>) &&
									         field.FieldType.GetGenericArguments()[0].IsSubclassOf(typeof(Component)))
									{
										// List of components field
										newTypeInfo.ListFieldsWithInjectFromGameObject ??= new List<FieldInfo>();
										newTypeInfo.ListFieldsWithInjectFromGameObject.Add(field);
									}
									else if (field.FieldType.IsArray && field.FieldType.GetElementType().IsSubclassOf(typeof(Component)))
									{
										// Array of components field
										newTypeInfo.ArrayFieldsWithInjectFromGameObject ??= new List<FieldInfo>();
										newTypeInfo.ArrayFieldsWithInjectFromGameObject.Add(field);
									}
									else
									{
										Debug.LogError($"Field {field.Name} in {scriptType.Name} must be a Component, List<Component>, or Component[] to use InjectFromGameObject", prefab);
									}
								}
							}

							_typeToTypeInfo.Add(scriptType, newTypeInfo);
						}

						var typeInfo = _typeToTypeInfo[scriptType];

						// Process single component fields with [InjectFromPrefab]
						foreach (var field in typeInfo.SingleComponentFieldsWithInjectFromPrefab ?? Enumerable.Empty<FieldInfo>())
						{
							var fieldType = field.FieldType;
							if (allComponentsDictionary.TryGetValue(fieldType, out var prefabComponent))
							{
								field.SetValue(script, prefabComponent);
								EditorUtility.SetDirty(script);
							}
							else
							{
								Debug.LogError($"Prefab {prefab.name} does not have a component of type {fieldType.Name} required by field {field.Name}!", prefab);
							}
						}

						// Process list fields with [InjectFromPrefab]
						foreach (var field in typeInfo.ListFieldsWithInjectFromPrefab ?? Enumerable.Empty<FieldInfo>())
						{
							var elementType = field.FieldType.GetGenericArguments()[0];
							var method = typeof(GameObject).GetMethod("GetComponentsInChildren", new Type[] { }).MakeGenericMethod(elementType);
							var components = (Array)method.Invoke(prefab, null);
							var listType = field.FieldType;
							var list = Activator.CreateInstance(listType, components);
							field.SetValue(script, list);
							EditorUtility.SetDirty(script);
						}

						// Process array fields with [InjectFromPrefab]
						foreach (var field in typeInfo.ArrayFieldsWithInjectFromPrefab ?? Enumerable.Empty<FieldInfo>())
						{
							var elementType = field.FieldType.GetElementType();
							var method = typeof(GameObject).GetMethod("GetComponentsInChildren", new Type[] { }).MakeGenericMethod(elementType);
							var components = (Array)method.Invoke(prefab, null);
							field.SetValue(script, components);
							EditorUtility.SetDirty(script);
						}

						// Process single component fields with [InjectFromGameObject]
						foreach (var field in typeInfo.SingleComponentFieldsWithInjectFromGameObject ?? Enumerable.Empty<FieldInfo>())
						{
							var fieldType = field.FieldType;
							var gameObject = script.gameObject;
							var gameObjectComponent = gameObject.GetComponent(fieldType);
							if (gameObjectComponent != null)
							{
								field.SetValue(script, gameObjectComponent);
								EditorUtility.SetDirty(script);
							}
							else
							{
								Debug.LogError($"GameObject {gameObject.name} in prefab {prefab.name} does not have a component of type {fieldType.Name} required by field {field.Name}!", prefab);
							}
						}

						// Process list fields with [InjectFromGameObject]
						foreach (var field in typeInfo.ListFieldsWithInjectFromGameObject ?? Enumerable.Empty<FieldInfo>())
						{
							var elementType = field.FieldType.GetGenericArguments()[0];
							var gameObject = script.gameObject;
							var method = typeof(GameObject).GetMethod("GetComponents", new Type[] { }).MakeGenericMethod(elementType);
							var components = (Array)method.Invoke(gameObject, null);
							var listType = field.FieldType;
							var list = Activator.CreateInstance(listType, components);
							field.SetValue(script, list);
							EditorUtility.SetDirty(script);
						}

						// Process array fields with [InjectFromGameObject]
						foreach (var field in typeInfo.ArrayFieldsWithInjectFromGameObject ?? Enumerable.Empty<FieldInfo>())
						{
							var elementType = field.FieldType.GetElementType();
							var gameObject = script.gameObject;
							var method = typeof(GameObject).GetMethod("GetComponents", new Type[] { }).MakeGenericMethod(elementType);
							var components = (Array)method.Invoke(gameObject, null);
							field.SetValue(script, components);
							EditorUtility.SetDirty(script);
						}
					}
				}

				PrefabUtility.SavePrefabAsset(prefab);
			};
		}

		private struct TypeInfo
		{
			public List<FieldInfo> SingleComponentFieldsWithInjectFromPrefab;
			public List<FieldInfo> ListFieldsWithInjectFromPrefab;
			public List<FieldInfo> ArrayFieldsWithInjectFromPrefab;
			public List<FieldInfo> SingleComponentFieldsWithInjectFromGameObject;
			public List<FieldInfo> ListFieldsWithInjectFromGameObject;
			public List<FieldInfo> ArrayFieldsWithInjectFromGameObject;
		}
	}
}