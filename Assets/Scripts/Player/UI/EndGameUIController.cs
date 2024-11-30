using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EndGameUIController : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private Image background;
    [SerializeField] private Image windowImage;
    [SerializeField] private TMP_Text storyFinishText;
    [SerializeField] private Button continueButton;
    [SerializeField] private TMP_Text continueButtonText;
    [SerializeField] private Image continueButtonImage;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TMP_Text gameFinishText;
    [SerializeField] private TMP_Text mainMenuButtonText;
    [SerializeField] private Image mainMenuButtonImage;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip backgroundMusic;

    private void Start()
    {
        // Initialize opacity for all UI elements to 0
        SetOpacityToZero();

        // Disable button interactions at the start
        continueButton.interactable = false;
        mainMenuButton.gameObject.SetActive(false);

        // Start the fade-in sequence
        StartCoroutine(FadeInSequence());
    }

    private void SetOpacityToZero()
    {
        // Set opacity of all images and texts to 0
        SetImageOpacity(background, 0f);
        SetImageOpacity(windowImage, 0f);
        SetImageOpacity(continueButtonImage, 0f);
        SetImageOpacity(mainMenuButtonImage, 0f);

        SetTextOpacity(storyFinishText, 0f);
        SetTextOpacity(continueButtonText, 0f);
        SetTextOpacity(gameFinishText, 0f);
        SetTextOpacity(mainMenuButtonText, 0f);
    }

    private void SetImageOpacity(Image image, float opacity)
    {
        if (image != null)
        {
            Color color = image.color;
            color.a = opacity;
            image.color = color;
        }
    }

    private void SetTextOpacity(TMP_Text text, float opacity)
    {
        if (text != null)
        {
            Color color = text.color;
            color.a = opacity;
            text.color = color;
        }
    }

    private IEnumerator FadeInSequence()
    {
        // Fade in background and audio
        StartCoroutine(FadeInImage(background, 3f));
        StartCoroutine(FadeInAudio(audioSource, backgroundMusic, 3f));
        yield return new WaitForSeconds(3f);

        // Fade in window, story text, and continue button
        StartCoroutine(FadeInImage(windowImage, 3f));
        StartCoroutine(FadeInText(storyFinishText, 3f));
        StartCoroutine(FadeInImage(continueButtonImage, 3f));
        StartCoroutine(FadeInText(continueButtonText, 3f));
        yield return new WaitForSeconds(3f);

        continueButton.interactable = true; // Make the button interactable
    }

    public void OnContinueButtonClicked()
    {
        StartCoroutine(FadeOutAndSwitchToMainMenu());
    }

    private IEnumerator FadeOutAndSwitchToMainMenu()
    {
        // Fade out story text and continue button
        StartCoroutine(FadeOutText(storyFinishText, 2f));
        StartCoroutine(FadeOutImage(continueButtonImage, 2f));
        StartCoroutine(FadeOutText(continueButtonText, 2f));
        yield return new WaitForSeconds(2f);

        storyFinishText.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);

        // Activate and fade in main menu button and game finish text
        mainMenuButton.gameObject.SetActive(true);
        gameFinishText.gameObject.SetActive(true);
        StartCoroutine(FadeInText(gameFinishText, 2f));
        StartCoroutine(FadeInImage(mainMenuButtonImage, 2f));
        StartCoroutine(FadeInText(mainMenuButtonText, 2f));
        yield return new WaitForSeconds(2f);

        mainMenuButton.interactable = true; // Make the button interactable
    }

    public void OnMainMenuButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }

    private IEnumerator FadeInImage(Image image, float duration)
    {
        Color color = image.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = 1f;
        image.color = color;
    }

    private IEnumerator FadeInText(TMP_Text text, float duration)
    {
        Color color = text.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 1f, elapsedTime / duration);
            text.color = color;
            yield return null;
        }

        color.a = 1f;
        text.color = color;
    }

    private IEnumerator FadeOutImage(Image image, float duration)
    {
        Color color = image.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            image.color = color;
            yield return null;
        }

        color.a = 0f;
        image.color = color;
    }

    private IEnumerator FadeOutText(TMP_Text text, float duration)
    {
        Color color = text.color;
        float startAlpha = color.a;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, elapsedTime / duration);
            text.color = color;
            yield return null;
        }

        color.a = 0f;
        text.color = color;
    }

    private IEnumerator FadeInAudio(AudioSource source, AudioClip clip, float duration)
    {
        source.clip = clip;
        source.volume = 0f;
        source.Play();

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            source.volume = Mathf.Lerp(0f, 0.5f, elapsedTime / duration);
            yield return null;
        }

        source.volume = 0.5f;
    }
}
