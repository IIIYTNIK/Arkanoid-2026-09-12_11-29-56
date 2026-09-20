using UnityEngine;

public class GhostCube : MonoBehaviour
{
    private static readonly Vector3[] Vertices =
    {
        new(-0.5f, -0.5f, -0.5f),
        new( 0.5f, -0.5f, -0.5f),
        new( 0.5f,  0.5f, -0.5f),
        new(-0.5f,  0.5f, -0.5f),

        new(-0.5f, -0.5f,  0.5f),
        new( 0.5f, -0.5f,  0.5f),
        new( 0.5f,  0.5f,  0.5f),
        new(-0.5f,  0.5f,  0.5f)
    };

    private static readonly int[,] Edges =
    {
        { 0, 1 }, { 1, 2 }, { 2, 3 }, { 3, 0 },
        { 4, 5 }, { 5, 6 }, { 6, 7 }, { 7, 4 },
        { 0, 4 }, { 1, 5 }, { 2, 6 }, { 3, 7 }
    };

    [SerializeField] private Material lineMaterial;
    [SerializeField] private float lifetime = 1f;
    [SerializeField, Range(0f, 1f)] private float startAlpha = 0.7f;

    private LineRenderer[] lines;

    private void Start()
    {
        CreateEdges();
        StartCoroutine(FadeOut());
    }

    private void CreateEdges()
    {
        lines = new LineRenderer[Edges.GetLength(0)];

        for (int i = 0; i < Edges.GetLength(0); i++)
        {
            var edgeObject = new GameObject($"Edge_{i}");
            edgeObject.transform.SetParent(transform, false);

            var line = edgeObject.AddComponent<LineRenderer>();

            line.useWorldSpace = false;
            line.positionCount = 2;

            line.SetPosition(0, Vertices[Edges[i, 0]]);
            line.SetPosition(1, Vertices[Edges[i, 1]]);

            line.startWidth = 0.03f;
            line.endWidth = 0.03f;

            line.material = lineMaterial;

            lines[i] = line;
        }
    }

    private System.Collections.IEnumerator FadeOut()
    {
        float elapsed = 0f;

        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;

            float progress = Mathf.Clamp01(elapsed / lifetime);
            float alpha = Mathf.Lerp(startAlpha, 0f, progress);

            foreach (var line in lines)
            {
                SetLineAlpha(line, alpha);
            }

            yield return null;
        }

        Destroy(gameObject);
    }

    private void SetLineAlpha(LineRenderer line, float alpha)
    {
        Color color = line.startColor;
        color.a = alpha;

        line.startColor = color;
        line.endColor = color;
    }
}