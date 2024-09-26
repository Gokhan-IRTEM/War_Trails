using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Aircraft
{    

    public delegate void OnStateChangeHandler();


    public class GameManager : MonoBehaviour
    {
        /// <summary>
        /// Event is called when game state changes
        /// </summary>
        public event OnStateChangeHandler OnStateChange;
        private GameState gameState;

        public GameState GameState
        {
            get {
                return gameState;
            }
            set
            {
                gameState = value;
                if (OnStateChange != null) OnStateChange();
            }
        }

        /// <summary>
        /// The singleton GameManger instance
        /// </summary>
        public static GameManager Instance
        {
            get; private set;
        }

        /// <summary>
        /// Manages the singleton and set fullscreen resolution
        /// </summary>
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                
                //Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height,true);
                this.GameDifficulty = GameDifficulty.HardDesert;
            }
            else
                Destroy(gameObject);
        }

    }
