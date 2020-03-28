using UnityEngine;

namespace CountersPlus.Utils
{
    class OopsAllAprilFools : MonoBehaviour
    {
        private float t = 0;
        private Vector3 defaultPos = Vector3.zero;
        private bool firstUpdate = true;

        private void Update()
        {
            t += Time.deltaTime;
            if (t >= 60)
            {
                if (firstUpdate)
                {
                    firstUpdate = false;
                    defaultPos = transform.position;
                }
                Vector3 ohNo = new Vector3(0, ((t - 60f) / 60f) * Mathf.Sin(t), 0);
                transform.position = defaultPos +
                    (transform.right * ohNo.x) +
                    (transform.up * ohNo.y) +
                    (transform.forward * ohNo.z);
            }
        }
    }
}
