using Core.Services.PlayFab;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.UI
{
    public class GameSessionMenuUIController : MonoBehaviour
    {
        [SerializeField] private Transform _gameOverInterface;
        [SerializeField] private Transform _loadingScreen;
        [SerializeField] private Transform _errorScreen;
        
        
        private void Start()
        {
            PlayFabManager.Instance.LeaderboardUpdated += ShowGameOverScreen;
            PlayFabManager.Instance.ErrorOccured += ShowErrorMessage;
        }

        private void OnDestroy()
        {
            PlayFabManager.Instance.LeaderboardUpdated -= ShowGameOverScreen;
            PlayFabManager.Instance.ErrorOccured -= ShowErrorMessage;
        }
        
        public void RestartGame()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            
            SceneManager.LoadScene(currentSceneIndex);
        }

        public void ReturnToMainMenu()
        {
            int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

            int mainMenuSceneIndex = currentSceneIndex - 1;
            if (mainMenuSceneIndex < 0)
                return;

            SceneManager.LoadScene(mainMenuSceneIndex); 
        }
        
        public void TryAvoidError()
        {
            PlayFabManager.Instance.RepeatServerActions();
        }

        private void ShowGameOverScreen()
        {
            _loadingScreen.gameObject.SetActive(false);
            _gameOverInterface.gameObject.SetActive(true);
        }
        
        private void ShowErrorMessage()
        {
            _loadingScreen.gameObject.SetActive(false);
            _errorScreen.gameObject.SetActive(true);
        }
    }
}
