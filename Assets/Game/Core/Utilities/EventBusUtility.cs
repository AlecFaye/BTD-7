using Assets.Game.Core.Events;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Game.Core.Utilities
{
    public static class EventBusUtility
    {
        public static IReadOnlyList<Type> EventTypes { get; set; }
        public static IReadOnlyList<Type> EventBusTypes { get; set; }

#if UNITY_EDITOR

        [InitializeOnLoadMethod]
        public static void InitializeEditor()
        {
            EditorApplication.playModeStateChanged -= EditorApplication_PlayModeStatChanged;
            EditorApplication.playModeStateChanged += EditorApplication_PlayModeStatChanged;
        }

        private static void EditorApplication_PlayModeStatChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.ExitingPlayMode)
                ClearAllBuses();
        }

#endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            EventTypes = PredefinedAssemblyUtility.GetTypes(typeof(IEvent));
            EventBusTypes = InitializeAllBuses();
        }

        public static void ClearAllBuses()
        {
            for (int i = 0; i < EventBusTypes.Count; i++)
            {
                Type busType = EventBusTypes[i];
                System.Reflection.MethodInfo clearMethod = busType.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic);
                clearMethod.Invoke(null, null);
            }
        }

        private static IReadOnlyList<Type> InitializeAllBuses()
        {
            List<Type> eventBusTypes = new();

            Type type = typeof(EventBus<>);

            foreach (Type eventType in EventTypes)
            {
                Type busType = type.MakeGenericType(eventType);
                eventBusTypes.Add(busType);
            }

            return eventBusTypes;
        }
    }
}
