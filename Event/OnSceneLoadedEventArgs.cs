using System.Collections;
using System.Collections.Generic;
using GameFramework;
using GameFramework.Event;
using UnityEngine;

namespace Dungeon
{
    public sealed class OnSceneLoadedEventArgs : GameEventArgs
    {
        public static readonly int EventId = typeof(OnSceneLoadedEventArgs).GetHashCode();
        
        public OnSceneLoadedEventArgs()
        {
        }
        
        public override int Id
        {
            get
            {
                return EventId;
            }
        }
        
        public int SceneID
        {
            get;
            private set;
        }

        public static OnSceneLoadedEventArgs Create(int sceneId)
        {
            OnSceneLoadedEventArgs onSceneLoadedEventArgs = ReferencePool.Acquire<OnSceneLoadedEventArgs>();
            onSceneLoadedEventArgs.SceneID = sceneId;
            return onSceneLoadedEventArgs;
        }
        
        public override void Clear()
        {
        }
    }
}
