using System.Collections;
using System.Collections.Generic;
using Dungeon.DungeonEntity;
using Dungeon.Vision2D;
using GameFramework;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Dungeon.DungeonEntity
{
    /// <summary>
    /// 继承VisibleEntity而非此接口
    /// </summary>
    public interface IVisible
    {
        public VisitInformation OnVisited(VisitInformation visitInfo);
        public VisitInformation OnUnvisited(VisitInformation visitInfo);
    }

    [RequireComponent(typeof(Collider2D))]
    public abstract class DungeonVisibleEntity : DungeonEntity, IVisible
    {
        /// <summary>
        /// Should be called by visitor entity.
        /// </summary>
        /// <param name="visiter"></param>
        /// <returns></returns>
        public abstract VisitInformation OnUnvisited(VisitInformation visiter);

        /// <summary>
        /// Should be called by visitor entity.
        /// </summary>
        /// <param name="visiter"></param>
        /// <returns></returns>
        public abstract VisitInformation OnVisited(VisitInformation visiter);
    }

    [RequireComponent(typeof(Viewer))]
    public abstract class DungeonVisitorEntity : DungeonEntity, IVisible
    {
        /// <summary>
        /// Init mViewer and mVision.
        /// should be called by subclass
        /// </summary>
        public abstract void InitViewer();

        public abstract VisitInformation OnUnvisited(VisitInformation visiter);

        public abstract VisitInformation OnVisited(VisitInformation visiter);

        protected Viewer mViewer;
        protected Vision mVision;
    }


    public struct VisitInformation
    {
        public GameObject visiter;
        public GameObject visited;

        public VisitInformation(GameObject visiter, GameObject visited = null)
        {
            this.visiter = visiter;
            this.visited = visited;
        }
    }
}
