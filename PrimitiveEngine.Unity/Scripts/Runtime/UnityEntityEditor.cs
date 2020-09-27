// ReSharper disable PossibleNullReferenceException
#if UNITY_EDITOR
namespace PrimitiveEngine.Unity
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEditor.Compilation;
	using UnityEngine;
	using Assembly = System.Reflection.Assembly;


	partial class UnityEntity
	{
		[CustomEditor(typeof(UnityEntity))]
		public class UnityEntityEditor : Editor
		{
			private GenericMenu componentSelectionMenu;


			public override void OnInspectorGUI()
			{
				if (Application.isPlaying)
				{
					UnityEntity unityEntity = this.target as UnityEntity;
					EditorGUILayout.LabelField($"Entity ID: {unityEntity.entity.Id}");
					EditorGUILayout.Space();
				}
				
				base.OnInspectorGUI();
				
				if (GUILayout.Button("Add EntityComponent"))
				{
					GenericMenu menu = GetEntityComponentsMenu();
					menu.ShowAsContext();
				}
			}


			private static IEnumerable<Assembly> GatherAssemblies()
			{
				List<Assembly> assemblies = new List<Assembly>();
				foreach (UnityEditor.Compilation.Assembly unityAssembly in CompilationPipeline.GetAssemblies())
				{
					Assembly assembly = Assembly.Load(unityAssembly.name);
					assemblies.Add(assembly);
				}

				return assemblies;
			}


			private static List<Type> GetEntityComponentTypes(IEnumerable<Assembly> assemblies)
			{
				List<Type> entityComponentTypes = new List<Type>();
				foreach (Assembly assembly in assemblies)
				{
					Type[] types = assembly.GetTypes();
					foreach (Type type in types)
					{
						if (type != typeof(IEntityComponent)
							&& typeof(IEntityComponent).IsAssignableFrom(type)
							&& !typeof(UnityEngine.Object).IsAssignableFrom(type))
						{
							entityComponentTypes.Add(type);
						}
					}
				}

				return entityComponentTypes;
			}


			private void AddEntityComponentMenuItems(List<Type> entityComponentTypes)
			{
				foreach (Type componentType in entityComponentTypes)
				{
					if (componentType.IsAbstract)
						continue;
					this.componentSelectionMenu.AddItem(
						content: new GUIContent(componentType.Name),
						on: false,
						func: () =>
							{
								UnityEntity unityEntity = this.target as UnityEntity;
								unityEntity.AddComponent(componentType);
							});
				}
			}


			private GenericMenu GetEntityComponentsMenu()
			{
				if (this.componentSelectionMenu != null)
					return this.componentSelectionMenu;
				
				this.componentSelectionMenu = new GenericMenu();
				IEnumerable<Assembly> assemblies = GatherAssemblies();
				List<Type> entityComponentTypes = GetEntityComponentTypes(assemblies);
				AddEntityComponentMenuItems(entityComponentTypes);

				return this.componentSelectionMenu;
			}
		}
	}
}
#endif