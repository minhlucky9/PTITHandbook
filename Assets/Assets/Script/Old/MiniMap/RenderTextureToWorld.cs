using UnityEngine.EventSystems;
using UnityEngine;
using UnityEngine.UI;
public class RenderTextureToWorld : MonoBehaviour, IPointerClickHandler
{

    public Camera gridCamera; //Camera that renders to the texture
    private RectTransform textureRectTransform; //RawImage RectTransform that shows the RenderTexture on the UI

    private void Awake()
    {
        textureRectTransform = GetComponent<RectTransform>(); //Get the RectTransform
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //I get the point of the RawImage where I click
        RectTransformUtility.ScreenPointToLocalPointInRectangle(textureRectTransform, eventData.position, null, out Vector2 localClick);
        // Adjust the localClick to range (0, width/height)
        localClick.x = localClick.x + textureRectTransform.rect.width / 2f;
        localClick.y = localClick.y + textureRectTransform.rect.height / 2f;

        // Normalize the click coordinates to get viewport point
        Vector2 viewportClick = new Vector2(localClick.x / textureRectTransform.rect.width, localClick.y / textureRectTransform.rect.height);

        //I have a special layer for the objects I want to detect with my ray
        LayerMask layer = LayerMask.GetMask("Region");

        //I cast the ray from the camera which rends the texture
        Ray ray = gridCamera.ViewportPointToRay(new Vector3(viewportClick.x, viewportClick.y, 0));

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layer))
        {
            // Check if the hit object has a Button component
            Button hitButton = hit.collider.GetComponent<Button>();
            if (hitButton != null)
            {
                // Simulate a button click
                hitButton.onClick.Invoke();
                Debug.Log("Button clicked: " + hitButton.name);
            }
            else
            {
                Debug.Log("Hit: " + hit.collider.gameObject.name);
            }
        }
        else
        {
            Debug.Log("No hit detected.");
        }
    }
}
