using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class RestartPanel : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        Time.timeScale = 1f;

        EnemySpawnProgression.Reiniciar();

        if (SpawnIntervalManager.Instance != null)
        {
            SpawnIntervalManager.Instance.Reiniciar();
        }

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}