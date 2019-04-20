using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using TMPro;
using CountersPlus.Config;

namespace CountersPlus.Counters
{
    public class ScoreCounter : MonoBehaviour
    {
        TMP_Text _scoreMesh;

        private ScoreConfigModel settings;
        private ScoreController _scoreController;
        private GameplayModifiersModelSO gm;
        private GameplayCoreSceneSetupData gcssd;

        GameObject _RankObject;
        TMP_Text _RankText;
        int _maxPossibleScore = 0;
        int notes = 0;
        int decimalPrecision;
        int _currentScore;

        void Awake()
        {
            settings = CountersController.settings.scoreConfig;
            decimalPrecision = settings.DecimalPrecision;
            if (gameObject.name == "ScorePanel")
                PreInit();
            else
                StartCoroutine(YeetToBaseCounter());
        }

        IEnumerator YeetToBaseCounter()
        {
            GameObject baseCounter;
            while (true)
            {
                baseCounter = GameObject.Find("ScorePanel");
                if (baseCounter != null) break;
                yield return new WaitForSeconds(0.1f);
            }
            baseCounter.AddComponent<ScoreCounter>();
            Destroy(gameObject);
        }

        private void PreInit()
        {
            if (!(settings.Mode == ICounterMode.BaseGame || settings.Mode == ICounterMode.BaseWithOutPoints))
            {
                Destroy(GetComponent<ImmediateRankUIPanel>());
                for (var i = 0; i < transform.childCount; i++)
                {
                    Transform child = transform.GetChild(i);
                    if (child.gameObject.name != "ScoreText")
                    {
                        if (child.GetComponent<TextMeshProUGUI>() != null) Destroy(child.GetComponent<TextMeshProUGUI>());
                        Destroy(child.gameObject);
                    }
                }
                if (settings.Mode == ICounterMode.ScoreOnly) Destroy(GameObject.Find("ScoreText"));
                CountersController.ReadyToInit += Init;
            }else
                transform.position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
        }

        private void Init(CountersData data)
        {
            _scoreController = data.ScoreController;
            gm = data.ModifiersData;
            gcssd = data.GCSSD;

            transform.localScale = Vector3.one;
            transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().fontSize = 0.325f;
            GameObject scoreMesh = new GameObject("Counters+ | Score Percent");
            scoreMesh.transform.SetParent(transform, false);
            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
            TextHelper.CreateText(out _scoreMesh, position);
            _scoreMesh.text = "100.0%";
            _scoreMesh.fontSize = 3;
            _scoreMesh.color = Color.white;
            _scoreMesh.alignment = TextAlignmentOptions.Center;

            //transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().rectTransform.position = position + new Vector3(-6.425f, 7.67f, 0);
            transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().rectTransform.position = position + new Vector3(-0.01f, 7.77f, 0);
            if (settings.DisplayRank)
            {
                _RankObject = new GameObject("Counters+ | Score Rank");
                _RankObject.transform.SetParent(transform, false);
                TextHelper.CreateText(out _RankText, position);
                _RankText.text = "\nSSS";
                _RankText.fontSize = 4;
                _RankText.color = Color.white;
                _RankText.alignment = TextAlignmentOptions.Center;
            }
            if (_scoreController != null)
            {
                _scoreController.scoreDidChangeEvent += UpdateScore;
                _scoreController.noteWasCutEvent += OnNoteCut;
                _scoreController.noteWasMissedEvent += _OnNoteWasMissed;
            }
            if (settings.Mode == ICounterMode.LeavePoints || settings.Mode == ICounterMode.BaseWithOutPoints)
            {
                transform.Find("ScoreText").GetComponent<TextMeshProUGUI>().rectTransform.position = new Vector3(-3.2f,
                    0.35f + (settings.Mode == ICounterMode.LeavePoints ? 7.8f : 0), 7);
            }
        }
        
        void OnDestroy()
        {
            if (_scoreController != null)
            {
                _scoreController.scoreDidChangeEvent -= UpdateScore;
                _scoreController.noteWasCutEvent -= OnNoteCut;
                _scoreController.noteWasMissedEvent -= _OnNoteWasMissed;
            }
            CountersController.ReadyToInit -= Init;
        }

        public string GetRank(int score, float prec)
        {
            if (score >= _maxPossibleScore) return "SSS";
            if (prec > 0.9f) return "SS";
            if (prec > 0.8f) return "S";
            if (prec > 0.65f) return "A";
            if (prec > 0.5f) return "B";
            if (prec > 0.35f) return "C";
            if (prec > 0.2f) return "D";
            return "E";
        }

        private void _OnNoteWasMissed(NoteData data, int score)
        {
            if (data.noteType != NoteType.Bomb) notes++;
            UpdateScore(score);
        }

        private void OnNoteCut(NoteData data, NoteCutInfo info, int score)
        {
            if (info.allIsOK && data.noteType != NoteType.Bomb) notes++;
        }

        public void UpdateScore(int score)
        {
            StartCoroutine(DelayedUpdate(score)); //Give time for OnNoteCut to update local note hit counter.
        }

        private IEnumerator DelayedUpdate(int score)
        {
            yield return new WaitForEndOfFrame();
            _currentScore = score;
            _maxPossibleScore = ScoreController.MaxScoreForNumberOfNotes(notes);

            if (_scoreMesh != null)
            {
                if (_maxPossibleScore == 0)
                {
                    _scoreMesh.text = "100.0%";
                    if (settings.DisplayRank) _RankText.text = "\nSSS";
                }
                else
                {
                    float ratio = ScoreController.GetScoreForGameplayModifiersScoreMultiplier(score, gm.GetTotalMultiplier(gcssd.gameplayModifiers)) / (float)_maxPossibleScore;
                    //Force percent to round down to decimal precision
                    _scoreMesh.text = Math.Round(ratio * 100, settings.DecimalPrecision).ToString() + "%";
                    if (settings.DisplayRank) _RankText.text = "\n" + GetRank(score, ratio);
                }
            }
        }
    }
}
