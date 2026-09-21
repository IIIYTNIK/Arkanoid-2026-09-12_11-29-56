using UnityEngine;
using UI = UnityEngine.UI;

/// <summary>
/// Повесьте на UI-объект Text (Canvas → UI → Text - Legacy, или замените поле
/// на TMP_Text, если используете TextMeshPro). Подписывается на ScoreManager
/// и не содержит игровой логики — только отображение.
/// </summary>
[RequireComponent(typeof(UI.Text))]
public sealed class ScoreUI : MonoBehaviour
{
    [SerializeField] private string scoreFormat = "SCORE: {0:000000}";

    private UI.Text label;

    private void Awake()
    {
        label = GetComponent<UI.Text>();
    }

    private void OnEnable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged += HandleScoreChanged;
            HandleScoreChanged(ScoreManager.Instance.CurrentScore);
        }
    }

    private void OnDisable()
    {
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.OnScoreChanged -= HandleScoreChanged;
        }
    }

    private void HandleScoreChanged(int newScore)
    {
        label.text = string.Format(scoreFormat, newScore);
    }
}