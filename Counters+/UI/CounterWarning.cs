using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

namespace CountersPlus.UI
{
    class CounterWarning : MonoBehaviour
    {
        void Awake()
        {
            TextMeshPro name = gameObject.AddComponent<TextMeshPro>();
            name.text = "<u>Due to limitations, some counters may not reflect their true appearance in-game.</u>";
            name.fontSize = 1;
            name.color = Color.white;
            name.alignment = TextAlignmentOptions.Center;
            name.rectTransform.position = new Vector3(0, 2.1f, 2.25f);
            StartCoroutine(KillThyself());
        }

        IEnumerator KillThyself()
        {
            yield return new WaitForSeconds(7.5f);
            Destroy(gameObject);
        }
    }
}
