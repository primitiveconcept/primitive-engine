namespace PrimitiveEngine.Unity
{
	using UnityEngine;


	public partial class DebugView : MonoBehaviour
	{
		
	}
}

#if UNITY_EDITOR
namespace PrimitiveEngine.Unity
{
	using UnityEditor;
	using UnityEngine;


	public partial class DebugView
	{
		[CustomEditor(typeof(DebugView))]
		public class DebugViewEditor : UnityEditor.Editor
		{
			public override void OnInspectorGUI()
			{
				base.OnInspectorGUI();

				if (!Application.isPlaying)
					return;
				
				EditorGUILayout.LabelField($"Active Systems Count: {Game.EntityWorld.SystemManager.Systems.Count}");
				EditorGUILayout.LabelField($"Active Entities Count: {Game.EntityWorld.EntityManager.ActiveEntities.Count}");

				if (GUILayout.Button("Show Active Components"))
				{
					foreach (var entity in Game.EntityWorld.EntityManager.ActiveEntities)
					{
						foreach (var component in entity.Components)
						{
							Debug.Log($"{entity.Index}: {component.GetType().Name}");
						}
					}
				}
			}
		}
	}
}
#endif