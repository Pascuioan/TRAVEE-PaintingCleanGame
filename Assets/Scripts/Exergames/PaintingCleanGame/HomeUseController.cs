using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PaintingCleanGame
{
    public class HomeUseController : GameManagerBase
    {
        [SerializeField]
        private SpongeController _spongeController;
        [SerializeField]
        private PaintingController _paintingController;
        [SerializeField]
        private TimerController _timeController;
        [SerializeField]
        private ScoreController _scoreController;
        [SerializeField]
        private FinishExercisePanelsController _finishExercisePanelsController;

        private InputData _inputData;

        public bool Pause
        {
            get { return _timeController.Pause; }
            set {
                _timeController.Pause = value;
            }
        }

        public int Score
        {
            get { return _scoreController.Score; }
        }

        public void Init(InputData inputData)
        {
            _inputData = inputData;

            _paintingController.Init(_inputData, OnWindowCleaned);
            _spongeController.Init(_inputData);
            _timeController.Init(_inputData, OnTimeFinished);
            _scoreController.Init();

            StartIteration();
        }

        protected void StartIteration()
        {
            _finishExercisePanelsController.Hide(FinishExercisePanelType.PANEL_TYPE_HOME_USE);

            _paintingController.StartGame();
            _spongeController.StartGame();
            _scoreController.StartGame();

            _timeController.StartTime();
            _timerController.StartTimer();
        }

        protected void StopGame()
        {
            _timeController.StopTime();

            OnGameFinished();

            ExitGame();
        }

        protected void ExitGame()
        {
            SessionContainer.OnStop();
        }

        protected void OnWindowCleaned()
        {
            SessionContainer.OnIterationCompleted();

            _timeController.StopTime();

            OnGameFinished();
        }

        protected void OnTimeFinished ()
        {
            OnGameFinished();
        }

        protected void OnGameFinished()
        {
            _finishExercisePanelsController.Show(FinishExercisePanelType.PANEL_TYPE_HOME_USE);

            _timerController.StopTimer();

            _outputDataController.PushGameSession(
                new GameSessionOutputData() {
                    StartTime = _timerController.StartDateTime,
                    FinishTime = _timerController.FinishDateTime.Value,
                    BodySide = _inputData.BodySide.ToString(),
                    CleanRatio = _paintingController.GetCleanRatio(),
                }
            );
        }

        private SessionContainer SessionContainer
        {
            get {
                return GameObject.Find("SessionContainer").GetComponent<SessionContainer>();
            }
        }
    }
}
