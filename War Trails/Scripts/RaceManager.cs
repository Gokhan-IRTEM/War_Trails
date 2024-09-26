using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.InputSystem;


namespace Aircraft
{

    public class RaceManager : MonoBehaviour
    {
        [Tooltip("Number of laps for this race")]
        public int numLaps = 2;

        [Tooltip("Maximum time to reach the next checkpoint")]
        public float checkpointBonusTime = 15;
        public List<DifficultyModel> difficultyModels;

        /// <summary>
        /// The agent beeing followed by the camera
        /// </summary>
        public AircraftAgents FollowAgent { get; private set; }
        public Camera ActiveCamera { get; private set; }
        private CinemachineVirtualCamera virtualCamera;
        private PauseController pauseMenu;
        private CountDownUIController countDownUI;
        private HUDController hud;
        private GameOverUIController gameOverUI;
        private AircraftArea aircraftArea;
        private AircraftPlayer aircraftPlayer;
        private List<AircraftAgents> sortedAircraftAgents;

        private float lastResumeTime = 0f;
        private float previousElapsedTime = 0f;

        private float lastPlaceUpdate = 0f;
        private Dictionary<AircraftAgents, AircraftStatus> aircraftStatuses;

        private void Awake()
        {
            hud = FindObjectOfType<HUDController>();
            countDownUI = FindObjectOfType<CountDownUIController>();
            pauseMenu = FindObjectOfType<PauseController>();
            gameOverUI = FindObjectOfType<GameOverUIController>();
            virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
            aircraftArea = FindObjectOfType<AircraftArea>();
            ActiveCamera = FindObjectOfType<Camera>();
            

            
        }
        private void Start()
        {
            GameManager.Instance.OnStateChange += OnStateChange;

            //Choose a default agent for the default camer to follow in case we can't find a player
            FollowAgent = aircraftArea.AircraftAgents[0];
            if (FollowAgent == null) throw new System.Exception("No FollowAgent");

            foreach (AircraftAgents agent in aircraftArea.AircraftAgents)
            {
                agent.FreezeAgent();
                if (agent.GetType() == typeof(AircraftPlayer))
                {
                    FollowAgent = agent;
                    aircraftPlayer = (AircraftPlayer)agent;
                    aircraftPlayer.pauseInput.performed += PauseInput_Performed;

                }
                else
                {
                    Debug.Log("Game difficulty: " + GameManager.Instance.GameDifficulty.ToString());
                    
                    //Set the difficulty
                    agent.SetModel(GameManager.Instance.GameDifficulty.ToString(),
                        difficultyModels.Find(x => x.difficulty == GameManager.Instance.GameDifficulty).model);
                }
            }

            Debug.Assert(virtualCamera != null, "Virtual Camer was not specified");
            virtualCamera.Follow = FollowAgent.transform;
            virtualCamera.LookAt = FollowAgent.transform;
            hud.FollowAgent = FollowAgent;
            //Hide ui
            hud.gameObject.SetActive(false);
            countDownUI.gameObject.SetActive(false);
            gameOverUI.gameObject.SetActive(false);
            pauseMenu.gameObject.SetActive(false);
            StartCoroutine(StartRace());


        }

        private IEnumerator StartRace()
        {
            countDownUI.gameObject.SetActive(true);
            yield return countDownUI.StartCountdown();

            aircraftStatuses = new Dictionary<AircraftAgents, AircraftStatus>();
            foreach (AircraftAgents agent in aircraftArea.AircraftAgents)
            {
                AircraftStatus status = new AircraftStatus();
                status.lap = 1;
                status.timeRemaining = checkpointBonusTime;
                aircraftStatuses.Add(agent, status);
            }

            GameManager.Instance.GameState = GameState.Playing;

        }
        //React to state changes
        

        private void FixedUpdate()
        {
            if (GameManager.Instance.GameState == GameState.Playing)
            {
                if(lastPlaceUpdate + 0.5f < Time.fixedTime)
                {
                    lastPlaceUpdate = Time.fixedTime;
                    if (sortedAircraftAgents == null)
                    {
                        //Get a copy of the list for sorting
                        sortedAircraftAgents = new List<AircraftAgents>(aircraftArea.AircraftAgents);
                    }
                    //Recalculate race places
                    sortedAircraftAgents.Sort((a, b) => PlaceComparer(a, b));
                    for (int i =0; i< sortedAircraftAgents.Count;i++)
                    {
                        aircraftStatuses[sortedAircraftAgents[i]].place = i + 1;
                    }
                }

                foreach (AircraftAgents agent in aircraftArea.AircraftAgents)
                {
                    AircraftStatus status = aircraftStatuses[agent];
                    if (status.checkpointIndex != agent.NextCheckpointIndex)
                    {
                        status.checkpointIndex = agent.NextCheckpointIndex;
                        status.timeRemaining = checkpointBonusTime;

                        if (status.checkpointIndex == 0)
                        {
                            status.lap++;
                            if (agent == FollowAgent && status.lap > numLaps)
                            {
                                GameManager.Instance.GameState = GameState.Gameover;
                            }
                        }
                    }
                    status.timeRemaining = Mathf.Max(0f, status.timeRemaining - Time.fixedDeltaTime);
                    if (status.timeRemaining ==0f)
                    {
                        aircraftArea.ResetAgentPosition(agent, aircraftArea.trainingMode);
                        status.timeRemaining = checkpointBonusTime;
                    }
                }
            }
        }              
        
    }
}