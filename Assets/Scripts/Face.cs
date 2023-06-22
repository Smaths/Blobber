using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Snarfum
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BlobFace", order = 1)]
    public class Face : ScriptableObject
    {
        public Texture Idleface, WalkFace, JumpFace, AttackFace, DamageFace;
    }
}

