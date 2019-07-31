using System;
using UnityEngine;
using TMPro;
using CountersPlus.Config;
using System.Collections.Generic;

namespace CountersPlus.Counters
{
    public class CutCounter : MonoBehaviour
    {
        private TMP_Text cutLabel;
        private ScoreController _scoreController;
        private GameObject _RankObject;
        private TMP_Text cutCounter;
        private CutConfigModel settings;
        private int totalCutCount = 0;
        private int totalScore = 0; // MaxScoreForNumberOfNotes() is an int. Don't expect scores over 2 billion.
        private Dictionary<SaberAfterCutSwingRatingCounter, NoteCutInfo> noteCutInfos = new Dictionary<SaberAfterCutSwingRatingCounter, NoteCutInfo>();

        void Awake()
        {
            settings = CountersController.settings.cutConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            _scoreController = data.ScoreController;
            Vector3 position = CountersController.DeterminePosition(gameObject, settings.Position, settings.Distance);
            TextHelper.CreateText(out cutLabel, position);
            cutLabel.text = "Average Cut";
            cutLabel.fontSize = 3;
            cutLabel.color = Color.white;
            cutLabel.alignment = TextAlignmentOptions.Center;

            _RankObject = new GameObject("Counters+ | Cut Label");
            _RankObject.transform.parent = transform;
            TextHelper.CreateText(out cutCounter, position - new Vector3(0, 0.4f, 0));
            cutCounter.text = "0";
            cutCounter.fontSize = 4;
            cutCounter.color = Color.white;
            cutCounter.alignment = TextAlignmentOptions.Center;
            
            if (_scoreController != null)
                _scoreController.noteWasCutEvent += UpdateScore;
        }

        void OnDestroy()
        {
            _scoreController.noteWasCutEvent -= UpdateScore;
            CountersController.ReadyToInit -= Init;
        }

        private void UpdateScore(NoteData data, NoteCutInfo info, int score)
        {
            if (data.noteType == NoteType.Bomb || !info.allIsOK) return;
            noteCutInfos.Add(info.afterCutSwingRatingCounter, info);
            info.afterCutSwingRatingCounter.didFinishEvent -= AfterCutSwingRatingCounter_didFinishEvent;
            info.afterCutSwingRatingCounter.didFinishEvent += AfterCutSwingRatingCounter_didFinishEvent;
        }

        private void AfterCutSwingRatingCounter_didFinishEvent(SaberAfterCutSwingRatingCounter v)
        {
            ScoreController.RawScoreWithoutMultiplier(noteCutInfos[v], v, out int beforeCut, out int afterCut, out int why);
            totalScore += beforeCut + afterCut;
            ++totalCutCount; // Should always be non-zero.
            cutCounter.text = $"{Math.Round( ((double)totalScore) / totalCutCount )}";
            noteCutInfos.Remove(v);
        }
    }
}