using UnityEngine;

namespace CountersPlus.Utils
{
    public class SoftParent : MonoBehaviour
    {
        private Transform parent;
        private Vector3 oldWorldPos;
        private Quaternion oldWorldRotation;

        private Vector3 posOffset;
        private Quaternion rotOffset;

        private void Awake()
        {
            oldWorldPos = transform.position;
            oldWorldRotation = transform.rotation;
        }

        private void Update()
        {
            if (parent == null) return;
            transform.SetPositionAndRotation(parent.position, parent.rotation);
            Vector3 side = parent.right * posOffset.x;
            Vector3 forward = parent.forward * posOffset.z;
            Vector3 total = side + forward;
            total = new Vector3(total.x, posOffset.y, total.z);
            transform.position -= total;
            transform.rotation *= Quaternion.Inverse(rotOffset);
        }

        public void AssignParent(Transform newParent)
        {
            parent = newParent;
            posOffset = parent.position - oldWorldPos;
            rotOffset = parent.rotation * Quaternion.Inverse(oldWorldRotation);
        }

        public void AssignOffsets(Vector3 positionOffset, Quaternion rotationOffset)
        {
            posOffset = positionOffset;
            rotOffset = rotationOffset;
        }
    }
}
