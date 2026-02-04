using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ForceBgSorting : MonoBehaviour
{
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Background";
        sr.sortingOrder = -100;
        transform.position = new Vector3(0, 0, 0);
    }
}
