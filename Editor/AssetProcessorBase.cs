using System;
using System.Collections.Generic;
using UnityEditor;
using Object = UnityEngine.Object;

namespace NiceDependencyInjection
{
	public class AssetProcessorBase : AssetPostprocessor
	{
		protected static List<string> _includedDirectories;
		protected static List<string> _excludedDirectories;
		protected static HashSet<Type> _includedTypes = new();
		protected static HashSet<Type> _excludedTypes = new();
		protected static Action<Object> _onMovedOut;
		protected static Action<Object> _onMovedIn;
		protected static Action<Object> _onMoved;
		protected static Action<Object> _onImported;
		protected static Action<Object> _onMovedInOrImported;
		protected static Action<Object> _onMovedOrImported;

		private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
		{
			foreach (var importedPath in importedAssets)
			{
				if (CheckPathMatch(importedPath))
				{
					var type = AssetDatabase.GetMainAssetTypeAtPath(importedPath);
					if (CheckTypeMatch(type))
					{
						var asset = AssetDatabase.LoadAssetAtPath<Object>(importedPath);
						_onImported?.Invoke(asset);
						_onMovedInOrImported?.Invoke(asset);
						_onMovedOrImported?.Invoke(asset);
					}
				}
			}

			for (var i = 0; i < movedAssets.Length; i++)
			{
				var newPath = movedAssets[i];
				var oldPath = movedFromAssetPaths[i];
				var type = AssetDatabase.GetMainAssetTypeAtPath(newPath);
				if (CheckTypeMatch(type))
				{
					var asset = AssetDatabase.LoadAssetAtPath<Object>(newPath);
					var oldPathIsInside = CheckPathMatch(oldPath);
					var newPathIsInside = CheckPathMatch(newPath);

					if (!oldPathIsInside && newPathIsInside)
					{
						_onMoved?.Invoke(asset);
						_onMovedIn?.Invoke(asset);
						_onMovedInOrImported?.Invoke(asset);
						_onMovedOrImported?.Invoke(asset);
					}
					else if (oldPathIsInside && !newPathIsInside)
					{
						_onMoved?.Invoke(asset);
						_onMovedOut?.Invoke(asset);
						_onMovedOrImported?.Invoke(asset);
					}
					else
					{
						_onMoved?.Invoke(asset);
						_onMovedOrImported?.Invoke(asset);
					}
				}
			}
		}

		private static bool CheckPathMatch(string assetPath)
		{
			if (_excludedDirectories != null)
			{
				foreach (var excludedDirectory in _excludedDirectories)
				{
					if (assetPath.Contains(excludedDirectory))
					{
						return false;
					}
				}
			}

			if (_includedDirectories != null)
			{
				foreach (var includedDirectory in _includedDirectories)
				{
					if (assetPath.Contains(includedDirectory))
					{
						return true;
					}
				}

				return false;
			}

			return true;
		}

		private static bool CheckTypeMatch(Type assetType)
		{
			if (_excludedTypes.Contains(assetType))
			{
				return false;
			}

			if (_includedTypes.Count > 0)
			{
				return _includedTypes.Contains(assetType);
			}

			return true;
		}
	}
}