using UnityEngine;

namespace Graph
{
    public class Graph : MonoBehaviour
    {
        [SerializeField] private Transform pointPrefab;
    
        [SerializeField, Range(10, 100)]
        int resolution = 10;
    
        Transform[] points;
    
        void Awake()
        {
            points = new Transform[resolution];
            var step = 2f / resolution;
            var position = Vector3.zero;
            var scale = Vector3.one * step;
            for (var i = 0; i < resolution; i++) {
                var point = points[i] = Instantiate(pointPrefab, transform, true);
                position.x = (i + 0.5f) * step - 1f;
                point.localPosition = position;
                point.localScale = scale;
                position.x = (i + 0.5f) * step - 1f;
            }
        }
    
        void Update () {
            float time = Time.time;
            for (int i = 0; i < points.Length; i++)
            {
                Transform point = points[i];
                Vector3 position = point.localPosition;
                position.y = Mathf.Sin(Mathf.PI * (position.x + time));
                point.localPosition = position;
            }
        }
    }
}