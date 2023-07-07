using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/BlobFaces", order = 1)]
    public class FaceData : ScriptableObject
    {
        public Texture IdleFace;
        public Texture WalkFace;
        public Texture JumpFace;
        public Texture AttackFace;
        public Texture DamageFace;
    }
}

