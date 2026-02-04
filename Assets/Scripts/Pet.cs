using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class ForcePetSorting : MonoBehaviour
{
    void Start()
    {
        var sr = GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Default";
        sr.sortingOrder = 999; 
        var c = sr.color; c.a = 1f; 
        sr.color = c;
        transform.position = new Vector3(0, 0, 0); 
    }
}