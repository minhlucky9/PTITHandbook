using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    // Reference to the plane's Renderer
    public AudioSource GOAL;
    Renderer planeRenderer;

    void Start()
    {
        // Get the Renderer component from the plane
        planeRenderer = GetComponent<Renderer>();

        // Create a new color (e.g., red)
        Color newColor = Color.red;

        // Assign the new color to the material
        planeRenderer.material.color = newColor;
    }
    void OnCollisionEnter(Collision collision)
    {
        // Ki?m tra n?u ??i t??ng va ch?m có tag là "END"
        if (collision.gameObject.tag == "Player")
        {
            // Phát âm thanh
            GOAL.Play();
        }
    }
}
