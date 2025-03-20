using System.Collections;
using System.Collections.Generic;
using CrashKonijn.Agent.Core;
using UnityEngine;

namespace Dungeon
{
    public class TorchTransformTarget : ITarget
    {
        public Transform torchTransform;

        public TorchTransformTarget(Transform torchTransform)
        {
            this.torchTransform = torchTransform;
        }
        public Vector3 Position => torchTransform.position;

        public bool IsValid()
        {
            return torchTransform != null;
        }
    }
}
