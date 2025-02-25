using UnityEditor;
using UnityEngine;

namespace NiceDependencyInjection.Editor
{
	[CustomPropertyDrawer(typeof(InjectFromPrefabAttribute))]
	[CustomPropertyDrawer(typeof(InjectFromGameObjectAttribute))]
	public class InjectPropertyDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			EditorGUI.BeginDisabledGroup(true);

			if (property.isArray && property.propertyType != SerializedPropertyType.String)
			{
				// Handle collections (arrays and lists)
				var labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
				EditorGUI.LabelField(labelRect, label);

				if (property.arraySize == 0)
				{
					var sizeRect = new Rect(position.x, labelRect.yMax, position.width, EditorGUIUtility.singleLineHeight);
					EditorGUI.LabelField(sizeRect, "Empty");
				}
				else
				{
					// Draw each element without interactive controls
					for (var i = 0; i < property.arraySize; i++)
					{
						var elementRect = new Rect(position.x, labelRect.yMax + i * EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
						var element = property.GetArrayElementAtIndex(i);
						EditorGUI.ObjectField(elementRect, element, GUIContent.none);
					}
				}
			}
			else
			{
				// Handle single values
				EditorGUI.PropertyField(position, property, label);
			}

			EditorGUI.EndDisabledGroup();
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			if (property.isArray && property.propertyType != SerializedPropertyType.String)
			{
				// Height for label plus each element (or "Empty" message if array is empty)
				return EditorGUIUtility.singleLineHeight * (property.arraySize > 0 ? property.arraySize + 1 : 2);
			}

			return EditorGUIUtility.singleLineHeight;
		}
	}
}