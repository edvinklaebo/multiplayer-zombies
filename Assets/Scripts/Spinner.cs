using UnityEngine;

public class Spinner : MonoBehaviour
{
    [SerializeField] private RectTransform spinner;
    [SerializeField] private float speed = 200f;

    private void Update()
    {
        spinner.Rotate(0, 0, -speed * Time.deltaTime);
    }
}