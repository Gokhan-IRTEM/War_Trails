using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Aircraft
{

    public class HUDController : MonoBehaviour
    {
        [Tooltip("The place in the race")]
        public TMP_Text placeText;
        [Tooltip("The remaining time to reach the next checkpoint")]
        public TMP_Text timeText;
        [Tooltip("The lap in the race")]
        public TMP_Text laptext;

        public Image checkpointIcon;
        public Image checkpointArrow;

        public float indicatorLimit = 0.7f;

        public AircraftAgents FollowAgent { get;  set; }

        private RaceManager raceManager;

        // Start is called before the first frame update
        private void Awake()
        {
            raceManager = FindObjectOfType<RaceManager>();
        }
        private void Update()
        {
            if (FollowAgent != null)
            {
                UpdacePlaceText();
                UpdateTimeText();
                UpdateLapText();
                UpdateArrow();
            }
            else
                Debug.Log("Follow Agent eq null");

        }
   
    }
}
