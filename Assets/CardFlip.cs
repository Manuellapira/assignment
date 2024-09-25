using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardFlipWithChildShake : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Button flipButton;            // Reference to the Button component
    public Image frontImage;             // Reference to the Front Image
    public Image backImage;              // Reference to the Back Image (this will shake as a child)
    public float hoverScale = 1.2f;      // Scale multiplier when hovering
    public float rotationSpeed = 500f;   // Speed of the card rotation
    public float shakeDuration = 0.5f;   // Duration of the shake effect
    public float shakeAmount = 0.1f;     // Intensity of the shake effect

    private Vector3 originalScale;
    private bool isRotating = false;
    private bool showingFront = true;    // Track whether the front is showing

    void Start()
    {
        // Store the original scale of the card
        originalScale = transform.localScale;

        // Register the OnClick event handler for the button
        flipButton.onClick.AddListener(OnFlipButtonClick);

        // Ensure the front image is shown and the back image is hidden initially
        frontImage.gameObject.SetActive(true);
        backImage.gameObject.SetActive(false);
    }

    // Called when the mouse pointer enters the card's area
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Enlarge the card when mouse enters
        transform.localScale = originalScale * hoverScale;
    }

    // Called when the mouse pointer exits the card's area
    public void OnPointerExit(PointerEventData eventData)
    {
        // Revert the card size when mouse exits
        transform.localScale = originalScale;
    }

    // Triggered when the button is clicked
    void OnFlipButtonClick()
    {
        if (!isRotating)
        {
            StartCoroutine(RotateCard());
        }
    }

    private IEnumerator RotateCard()
    {
        isRotating = true;
        float targetRotation = transform.eulerAngles.y + 180f;  // Rotate 180 degrees
        float currentRotation = transform.eulerAngles.y;

        // Rotate the card smoothly
        while (Mathf.Abs(currentRotation - targetRotation) > 0.1f)
        {
            currentRotation = Mathf.MoveTowards(currentRotation, targetRotation, rotationSpeed * Time.deltaTime);
            transform.eulerAngles = new Vector3(0f, currentRotation, 0f);

            // Switch between front and back images at the halfway point (90 degrees)
            if (currentRotation > 90f && showingFront)
            {
                frontImage.gameObject.SetActive(false); // Hide the front image
                backImage.gameObject.SetActive(true);  // Show the back image
                showingFront = false;
            }
            else if (currentRotation < 90f && !showingFront)
            {
                frontImage.gameObject.SetActive(true);  // Show the front image
                backImage.gameObject.SetActive(false);  // Hide the back image
                showingFront = true;
            }

            yield return null;
        }

        // Ensure the final rotation is exactly 180 degrees
        transform.eulerAngles = new Vector3(0f, targetRotation, 0f);

        // After rotation, trigger the shake effect on the back image
        StartCoroutine(ShakeBackImage());

        isRotating = false;
    }

    // Shake the back image (child) after the rotation is complete
    private IEnumerator ShakeBackImage()
    {
        Vector3 originalPosition = backImage.rectTransform.localPosition;  // Store the original position of the back image
        float elapsedTime = 0f;

        // Shake the back image for the specified duration
        while (elapsedTime < shakeDuration)
        {
            // Generate random offsets for X and Y within the shakeAmount range
            float xOffset = Random.Range(-shakeAmount, shakeAmount);
            float yOffset = Random.Range(-shakeAmount, shakeAmount);

            // Apply the random offsets to the back image's local position
            backImage.rectTransform.localPosition = new Vector3(originalPosition.x + xOffset, originalPosition.y + yOffset, originalPosition.z);

            // Increment the elapsed time by the time of this frame
            elapsedTime += Time.deltaTime;

            // Wait until the next frame
            yield return null;
        }

        // Ensure the back image returns to its original position after shaking
        backImage.rectTransform.localPosition = originalPosition;
    }
}
