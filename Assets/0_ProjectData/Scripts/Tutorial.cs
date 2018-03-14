using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Tutorial allows to add some text when the level starts.
/// The text will appear letter by letter or, when Return is pressed, it will appear entirely.
/// </summary>
[RequireComponent(typeof(Animator))]
public class Tutorial : MonoBehaviour
{
    // Public

    /// <summary>
    /// TextMesh filled by tutorial texts.
    /// </summary>
    [Header("Scene References")]
    public TextMesh tutorialTextComponent;
    /// <summary>
    /// SpriteRenderer that indicates the skip function.
    /// </summary>
    public SpriteRenderer tutorialSkipButtonComponent;

    /// <summary>
    /// Array of strings that define the entire tutorial for the level.
    /// </summary>
    [Header("Tutorial Texts")]
    [Tooltip("Use \"=\" to add a new line.")]
    public string[] tutorialTexts;

    /// <summary>
    /// List of actions to execute when the tutorial finishes.
    /// Mostly filled by the GameManager.
    /// </summary>
    public event Action ClosedTutorialCallback;

    // Private
    
    /// <summary>
    /// Tutorial line index.
    /// </summary>
    private int tutorialTextsIndex = 0;
    
    /// <summary>
    /// True if there is a tutorial text in writing phase, otherwise False.
    /// </summary>
    private bool isWriting = false;
    
    /// <summary>
    /// Animator that handles the closure animation.
    /// </summary>
    private Animator tutorialAnimator;

    private void Awake()
    {
        tutorialAnimator = GetComponent<Animator>();
    }

    private void Start()
    {
        StartCoroutine(StartTextAfterAnimation());
    }

    private IEnumerator StartTextAfterAnimation()
    {
        // Waiting the completition of the first animation step.
        while(tutorialAnimator.GetCurrentAnimatorStateInfo(0).IsName("TutorialOpen"))
        {
            yield return null;
        }
        
        tutorialSkipButtonComponent.DOFade(1, 1);
        StartCoroutine(AnimateText());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (!isWriting)
            {
                tutorialTextsIndex++;
                if (tutorialTextsIndex >= tutorialTexts.Length)
                {
                    CloseTutorial();
                }
                else
                { 
                    StartCoroutine(AnimateText());
                }
            }
            else
            {
                StopAllCoroutines();
                tutorialTextComponent.text = tutorialTexts[tutorialTextsIndex].Replace("=", "\n");
                isWriting = false;
            }
        }
    }

    private IEnumerator AnimateText()
    {
        isWriting = true;
        for (int i = 0; i < (tutorialTexts[tutorialTextsIndex].Length + 1); i++)
        {
            tutorialTextComponent.text = tutorialTexts[tutorialTextsIndex].Substring(0, i).Replace("=", "\n");
            yield return new WaitForSeconds(.03f);
        }

        isWriting = false;
        yield return null;
    }

    /// <summary>
    /// Closes the tutorial and starts the closure animation.
    /// </summary>
    public void CloseTutorial()
    {
        tutorialTextComponent.gameObject.SetActive(false);
        tutorialSkipButtonComponent.gameObject.SetActive(false);

        tutorialAnimator.SetTrigger("Close");

        StartCoroutine(CloseTutorialCoRoutine());
    }
    private IEnumerator CloseTutorialCoRoutine()
    {
        yield return new WaitForSeconds(1.1f);

        // Execute the callbacks, if defined.
        if (ClosedTutorialCallback != null)
        {
            ClosedTutorialCallback();
        }

        gameObject.SetActive(false);

        yield return null;
    }
}
