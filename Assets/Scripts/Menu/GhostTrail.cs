using UnityEngine;

public class GhostTrail : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private GameObject ghostPrefab;
    [SerializeField] private float spawnInterval = 0.1f;
    [SerializeField] private float ghostLifetime = 1f;

    private float timer;

    private void Update()
    {
        if (target == null || ghostPrefab == null)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            timer = 0f;
            SpawnGhost();
        }
    }

    private void SpawnGhost()
    {
        GameObject ghost = Instantiate(
            ghostPrefab,
            target.position,
            target.rotation
        );
    }
}