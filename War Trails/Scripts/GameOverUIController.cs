using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Aircraft
{

    public class GameOverUIController : MonoBehaviour
    {
        // Start is called before the first frame update
        public TextMeshProUGUI placeText;

        private RaceManager raceManager;

        private void Awake()
        {
            raceManager = FindObjectOfType<RaceManager>();

        }
       
    }
}
