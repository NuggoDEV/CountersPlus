using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Reflection;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    public class ProgressCounter : MonoBehaviour
    {

        TextMeshPro _timeMesh;
        AudioTimeSyncController _audioTimeSync;
        Image _image;
        private ProgressConfigModel settings;

        bool useTimeLeft = false;

        IEnumerator WaitForLoad()
        {
            yield return new WaitUntil(() => Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().Any());
            _audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
            Init();
        }

        void Awake()
        {
            settings = CountersController.settings.progressConfig;
            transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            useTimeLeft = settings.ProgressTimeLeft;
            if (settings.Mode == ICounterMode.BaseGame && gameObject.name != "SongProgressPanel")
                StartCoroutine(YeetToBaseCounter());
            else if (settings.Mode != ICounterMode.BaseGame)
                StartCoroutine(WaitForLoad());
        }

        IEnumerator YeetToBaseCounter()
        {
            yield return new WaitUntil(() => GameObject.Find("SongProgressPanel") != null);
            GameObject.Find("SongProgressPanel").AddComponent<ProgressCounter>();
            Plugin.Log("Progress Counter has been moved to the base game counter!");
            Destroy(gameObject);
        }

        void Init()
        {
            if (settings.Mode == ICounterMode.Original)
            {
                _timeMesh = this.gameObject.AddComponent<TextMeshPro>();
                _timeMesh.text = "0:00";
                _timeMesh.fontSize = 4;
                _timeMesh.color = Color.white;
                _timeMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
                _timeMesh.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
                _timeMesh.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);

                var image = ReflectionUtil.GetPrivateField<Image>(
                    Resources.FindObjectsOfTypeAll<ScoreMultiplierUIController>().First(), "_multiplierProgressImage");

                GameObject g = new GameObject();
                Canvas canvas = g.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.WorldSpace;
                CanvasScaler cs = g.AddComponent<CanvasScaler>();
                cs.scaleFactor = 10.0f;
                cs.dynamicPixelsPerUnit = 10f;
                GraphicRaycaster gr = g.AddComponent<GraphicRaycaster>();
                g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 1f);
                g.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 1f);

                GameObject g2 = new GameObject();
                _image = g2.AddComponent<Image>();
                g2.transform.parent = g.transform;
                g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
                g2.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
                g2.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);

                _image.sprite = image.sprite;
                _image.type = Image.Type.Filled;
                _image.fillMethod = Image.FillMethod.Radial360;
                _image.fillOrigin = (int)Image.Origin360.Top;
                _image.fillClockwise = true;


                GameObject g3 = new GameObject();
                var bg = g3.AddComponent<Image>();
                g3.transform.parent = g.transform;
                g3.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0.5f);
                g3.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 0.5f);
                g3.transform.localScale = new Vector3(2.3f, 2.3f, 2.3f);

                bg.sprite = image.sprite;
                bg.CrossFadeAlpha(0.05f, 1f, false);

                g.GetComponent<RectTransform>().SetParent(this.transform, false);
                g.transform.localPosition = new Vector3(-0.25f, .25f, 0f);
                transform.position += new Vector3(0.5f, 0, 0);
            }else if (settings.Mode == ICounterMode.Percent)
            {
                _timeMesh = this.gameObject.AddComponent<TextMeshPro>();
                _timeMesh.text = "0.00%";
                _timeMesh.fontSize = 4;
                _timeMesh.color = Color.white;
                _timeMesh.font = Resources.Load<TMP_FontAsset>("Teko-Medium SDF No Glow");
                _timeMesh.alignment = TextAlignmentOptions.Center;
            }
            StartCoroutine(UpdatePosition());
        }

        IEnumerator UpdatePosition()
        {
            while (true)
            {
                transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
                yield return new WaitForSeconds(10);
            }
        }

        void Update()
        {
            if (GameObject.Find("SongProgressPanel") != null && settings.Mode != ICounterMode.BaseGame) Destroy(GameObject.Find("SongProgressPanel"));
            if (_audioTimeSync == false)
            {
                _audioTimeSync = Resources.FindObjectsOfTypeAll<AudioTimeSyncController>().FirstOrDefault();
                return;
            }

            var time = 0f;
            if (useTimeLeft)
                time = _audioTimeSync.songLength - _audioTimeSync.songTime;
            else
                time = _audioTimeSync.songTime;

            if (time <= 0f)
                return;
            if (settings.Mode == ICounterMode.Original)
            {
                _timeMesh.text = $"{Math.Floor(time / 60):N0}:{Math.Floor(time % 60):00}";
                _image.fillAmount = (useTimeLeft ? 1 : 0) - _audioTimeSync.songTime / _audioTimeSync.songLength * (useTimeLeft ? 1 : -1);
            }else if (settings.Mode == ICounterMode.Percent)
            {
                _timeMesh.text = $"{((useTimeLeft ? 1 : 0) - ((_audioTimeSync.songTime / _audioTimeSync.songLength) * 100) * (useTimeLeft ? 1 : -1)).ToString("00")}%";
            }
        }
    }
}
