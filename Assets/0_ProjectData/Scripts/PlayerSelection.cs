using PiratesNS;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerSelection : MonoBehaviour
{
    private bool Selected;
    private Color Enabled = new Color(47, 47, 47);
    private Color Disabled = new Color(255, 255, 255);

    private Text gOText;
    private Player.Gender tmpGender;
    Animator An;

    private Vector3 defaultScale = new Vector3(5, 5, 1);
    private Vector3 finalScale = new Vector3(6, 6, 1);

    public AudioClip switchSelection;
    public AudioClip confirmSelection;

    void Start()
    {
        An = GetComponent<Animator>();
        SetSelectedCharacter(Player.Gender.Male);
    }

    void Update()
    {
        //Selected true = Male, selected false = Female
        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            SetSelectedCharacter(tmpGender == Player.Gender.Male ? Player.Gender.Female : Player.Gender.Male);
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            SoundManager.instance.PlaySingle(confirmSelection);

            Player.gender = tmpGender;

            SceneManager.LoadScene((int)PiratesEnumerators.Scenes.L0R1, LoadSceneMode.Single);
        }
    }
    void SetSelectedCharacter(Player.Gender gender)
    {
        tmpGender = gender;

        if (tmpGender == Player.Gender.Female)
        {
            Selected = false;
            An.SetTrigger("SelectFemale");
        }
        else
        {
            Selected = true;
            An.SetTrigger("SelectMale");
        }

        SoundManager.instance.PlaySingle(switchSelection);
    }
}
