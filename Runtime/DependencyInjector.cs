using System;
using System.Collections.Generic;
using NiceDependencyInjection.Utility;
using UnityEngine;

namespace NiceDependencyInjection
{
	public class DependencyInjector : MonoBehaviour
	{
		private static readonly Dictionary<Type, bool> _typeToIsInjectable = new();
		private static readonly Dictionary<Type, bool> _typeToHasInjectFields = new();

		[HideInInspector] [SerializeField] private List<MonoBehaviour> _componentsWithInjectFields;
		[HideInInspector] [SerializeField] private List<MonoBehaviour> _injectableComponents;

		private bool _isProcessed;

		private void Awake()
		{
			if (DependencyInjectionManager.IsInitialized)
			{
				Process();
			}
			else
			{
				DependencyInjectionManager.OnInitialized += Process;
			}
		}

		private void OnDestroy()
		{
			DependencyInjectionManager.OnInitialized -= Process;
		}

		public void OnValidate()
		{
			CacheComponents();
		}

		private void Process()
		{
			if (!_isProcessed)
			{
				InjectComponents();
				InjectIntoComponents();
				_isProcessed = true;
			}
		}

		private void CacheComponents()
		{
			_injectableComponents.Clear();
			_componentsWithInjectFields.Clear();
			var allMonoBehaviours = GetComponents<MonoBehaviour>();

			foreach (var target in allMonoBehaviours)
			{
				var targetType = target.GetType();

				if (!_typeToIsInjectable.TryGetValue(targetType, out var isInjectable))
				{
					isInjectable = Attribute.IsDefined(targetType, typeof(InjectableAttribute));
					_typeToIsInjectable[targetType] = isInjectable;
				}

				if (!_typeToHasInjectFields.TryGetValue(targetType, out var hasInjectableFields))
				{
					hasInjectableFields = false;

					var fields = ReflectionUtility.GetCachedFieldInfo(targetType);

					foreach (var field in fields)
					{
						if (field.IsDefined(typeof(InjectAttribute), true))
						{
							hasInjectableFields = true;
							break;
						}
					}

					_typeToHasInjectFields[targetType] = hasInjectableFields;
				}

				if (isInjectable)
				{
					_injectableComponents.Add(target);
				}

				if (hasInjectableFields)
				{
					_componentsWithInjectFields.Add(target);
				}
			}
		}

		private void InjectComponents()
		{
			foreach (var component in _injectableComponents)
			{
				DependencyInjectionManager.InjectObject(component);
			}
		}

		private void InjectIntoComponents()
		{
			foreach (var component in _componentsWithInjectFields)
			{
				DependencyInjectionManager.InjectIntoObject(component);
			}
		}
	}
}