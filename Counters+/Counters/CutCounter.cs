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
    public class CutCounter : MonoBehaviour
    {
        TMP_Text cutLabel;
        ScoreController _scoreController;

        private CutConfigModel settings;

        private List<int> cuts = new List<int>();

        GameObject _RankObject;
        TMP_Text cutCounter;

        void Awake()
        {
            settings = CountersController.settings.cutConfig;
            CountersController.ReadyToInit += Init;
        }

        private void Init(CountersData data)
        {
            _scoreController = data.ScoreController;
            Vector3 position = CountersController.determinePosition(gameObject, settings.Position, settings.Index);
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

        public void UpdateScore(NoteData data, NoteCutInfo info, int score)
        {
            if (data.noteType == NoteType.Bomb || !info.allIsOK) return;
            info.afterCutSwingRatingCounter.didFinishEvent += (v) =>
            {
                ScoreController.ScoreWithoutMultiplier(info, info.afterCutSwingRatingCounter, out int beforeCut, out int afterCut, out int why);
                cuts.Add(beforeCut+afterCut);
                cutCounter.text = $"{Math.Round(cuts.Average())}";
            };
        }
    }
}