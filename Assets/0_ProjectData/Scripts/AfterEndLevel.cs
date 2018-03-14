using PiratesNS;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class AfterEndLevel : MonoBehaviour
{
    public TextMesh stepsValueTextMesh;
    public SpriteRenderer endSceneFader;
    public SpriteRenderer returnToMenuButton;

    private bool isAuthorizedToReturnToMenu = false;

    void Start()
    {
        stepsValueTextMesh.text = stepsValueTextMesh.text.Replace("@", GameManager.instance.playerTotalMoney.ToString());

        endSceneFader.DOFade(0, 3).OnComplete(FaderFadeComplete);
    }

    void FaderFadeComplete()
    {
        StartCoroutine(EnableReturnButtonCoroutine());
    }

    IEnumerator EnableReturnButtonCoroutine()
    {
        yield return new WaitForSeconds(2);
        returnToMenuButton.DOFade(1, 1);
        isAuthorizedToReturnToMenu = true;
        yield return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (isAuthorizedToReturnToMenu)
                GameManager.instance.GoToScene(PiratesEnumerators.Scenes.MainMenu);
        }
    }
}
