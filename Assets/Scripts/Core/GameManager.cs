using Arkanoid.Ball;
using Arkanoid.Core.Interfaces;
using UnityEngine;

namespace Arkanoid.Core
{
    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        LevelComplete,
        GameOver
    }

    /// <summary>
    /// Единственная точка, где менеджеры создаются и регистрируются в GameServices.
    /// Не содержит игровой логики сам — только оркестрацию жизненного цикла.
    /// Держим этот класс маленьким (см. правило п.24.1 ТЗ: не создавать монолитный GameManager).
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GameManager : MonoBehaviour
    {
        [SerializeField] private BallManager ballManager;
        [SerializeField] private LifeManager lifeManager;
        [SerializeField] private ScoreManager scoreManager;

        public GameState CurrentState { get; private set; } = GameState.MainMenu;

        private void Awake()
        {
            GameServices.Reset();
            GameServices.Register<IBallManager>(ballManager);
            GameServices.Register<ILifeManager>(lifeManager);
            GameServices.Register<IScoreManager>(scoreManager);
            // LevelManager и PowerUpManager регистрируются их владельцем (Developer 2),
            // используя тот же паттерн. GameManager не обязан знать об их существовании
            // на этапе Phase 1 — только контракт.
        }

        private void OnEnable()
        {
            lifeManager.OnGameOver += HandleGameOver;
        }

        private void OnDisable()
        {
            lifeManager.OnGameOver -= HandleGameOver;
        }

        public void StartGame()
        {
            CurrentState = GameState.Playing;
            Time.timeScale = 1f;
        }

        public void TogglePause()
        {
            if (CurrentState != GameState.Playing && CurrentState != GameState.Paused)
            {
                return;
            }

            CurrentState = CurrentState == GameState.Playing ? GameState.Paused : GameState.Playing;
            Time.timeScale = CurrentState == GameState.Paused ? 0f : 1f;
        }

        public void RestartLevel()
        {
            GameServices.Reset();
            Time.timeScale = 1f;
            UnityEngine.SceneManagement.SceneManager.LoadScene(
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
        }

        private void HandleGameOver()
        {
            CurrentState = GameState.GameOver;
            Time.timeScale = 0f;
        }
    }
}
