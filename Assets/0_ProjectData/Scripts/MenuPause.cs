using PiratesNS;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MenuPause : MonoBehaviour
{
    // Public
    public Transform[] pause;
    public Player player;
    public float bookSpeed;

    [Header("MenuVoices")]
    public GameObject[] MenuVoices;
    public GameObject MenuPointer;

    [Header("MenuSounds")]
    public AudioClip switchSelection;
    public AudioClip confirmSelection;

    // Private but Inspector
    [SerializeField] SpriteRenderer[] Beasts;
    [SerializeField] TextMesh[] BeastsTexts;
    [SerializeField] SpriteRenderer[] MenuNavButtons;
    [SerializeField] TextMesh BackTextMesh;

    // Private
    private int menuIndex;

    private int pauseIndex;
    private MeshRenderer bookClosedMenuSubtitleMeshRenderer;

    private SpriteRenderer BestiarySpriteRenderer;
    private int bestiaryNavIndex;

    void Start()
    {
        transform.position = pause[0].position;
        pauseIndex = 0;

        bookClosedMenuSubtitleMeshRenderer = GameObject.Find("BookClosedMenuSubtitle").GetComponent<MeshRenderer>();
        BestiarySpriteRenderer = GameObject.Find("Bestiary").GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Open/Close Pause menu.
        if (
            GameManager.instance.state == GameManager.State.Play ||
            GameManager.instance.state == GameManager.State.Pause
            )
        {
            if (Input.GetKeyDown(KeyCode.P) && player.isStillAlive)
            {
                if (transform.position == pause[pauseIndex].position)
                {
                    OpenMenu();
                }
            }
            if (Input.GetKeyDown(KeyCode.P) && player.isStillAlive)
            {
                if (pauseIndex >= pause.Length)
                {
                    CloseMenu();
                }
            }
        }

        //Move into Pause menu.
        if (GameManager.instance.state == GameManager.State.Pause && (
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow)
            ))
        {
            menuIndex = Mathf.Clamp(menuIndex - (int)Input.GetAxisRaw("Vertical"), 0, MenuVoices.Length - 1);
            SetMenuVoice(menuIndex);
        }

        if (transform.position != pause[pauseIndex].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, pause[pauseIndex].position, bookSpeed * Time.deltaTime); // Move the object to Arrays
        }
        if (transform.position == pause[0].position)
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = true;
            if (menuIndex != 0)
            {
                menuIndex = 0;
                SetMenuVoice(menuIndex);
            }
        }
        else
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = false;
        }

        if (GameManager.instance.state == GameManager.State.Bestiary && Input.GetKeyDown(KeyCode.Backspace))
        {
            SoundManager.instance.PlaySingle(confirmSelection);
            CloseBestiary();
        }
        //Confirm selection into Pause menu.
        else if (GameManager.instance.state == GameManager.State.Pause && Input.GetKeyDown(KeyCode.Return))
        {
            SoundManager.instance.PlaySingle(confirmSelection);
            switch (MenuVoices[menuIndex].name)
            {
                case "MenuVoiceBestiary":
                    OpenBestiary();
                    break;
                case "MenuVoiceResume":
                    CloseMenu();
                    break;
                case "MenuVoiceRestartRoom":
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
                    break;
                case "MenuVoiceBackToMainMenu":
                    SceneManager.LoadScene((int)PiratesEnumerators.Scenes.MainMenu, LoadSceneMode.Single);
                    break;
            }
        }

        if (GameManager.instance.state == GameManager.State.Bestiary)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                NavigateBestiary(KeyCode.LeftArrow);
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                NavigateBestiary(KeyCode.RightArrow);
            }
        }

        //Move book from A to B.
        if (transform.position != pause[pauseIndex].position)
        {
            transform.position = Vector3.MoveTowards(transform.position, pause[pauseIndex].position, bookSpeed * Time.deltaTime); // Move the object to Arrays
        }
        if (transform.position == pause[0].position)
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = true;
            if (menuIndex != 0)
            {
                menuIndex = 0;
                SetMenuVoice(menuIndex);
            }
        }
        else
        {
            bookClosedMenuSubtitleMeshRenderer.enabled = false;
        }
    }
    List<GameObject> Child = new List<GameObject>();

    private void OpenBestiary()
    {
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Bestiary;

        foreach (Transform child in transform)
        {
            Child.Add(child.gameObject);
        }
        BestiarySpriteRenderer.DOFade(1, 1);
        for (int i = 0; i < Child.Count; i++)
        {
            if (Child[i].name == "Bestiary")
            {
                Child[i].SetActive(true);
                BestiarySpriteRenderer.DOFade(0, 1);
                NavigateBestiary();
            }
            else
            {
                Child[i].SetActive(false);
            }
        }
        for (int i = 0; i < MenuNavButtons.Length; i++)
        {
            MenuNavButtons[i].enabled = true;
        }
        BackTextMesh.gameObject.SetActive(true);
    }
    private void CloseBestiary()
    {
        BestiarySpriteRenderer.DOFade(1, 1);
        for (int i = 0; i < Child.Count; i++)
        {
            if (Child[i].name == "Bestiary")
            {
                Child[i].SetActive(false);
            }
            else
            {
                Child[i].SetActive(true);
                BestiarySpriteRenderer.DOFade(0, 1);
            }
        }
        for (int i = 0; i < MenuNavButtons.Length; i++)
        {
            MenuNavButtons[i].enabled = false;
        }
        BackTextMesh.gameObject.SetActive(false);
        BestiarySpriteRenderer.DOFade(0, 1);
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Pause;
    }

    private void OpenMenu()
    {
        pauseIndex++;
        GetComponent<AudioSource>().Play();
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Pause;
    }

    private void CloseMenu()
    {
        pauseIndex = 0;
        if (GameManager.instance != null) GameManager.instance.state = GameManager.State.Play;
    }

    private void SetMenuVoice(int menuIndex)
    {
        GameObject tempItem;
        Vector3 defaultLocalScale = new Vector3(1, 1, 1);
        Vector3 selectedLocalScale = new Vector3(1.1f, 1.1f, 1);

        for (int i = 0; i < MenuVoices.Length; i++)
        {
            tempItem = MenuVoices[i];

            if (i == menuIndex)
            {
                tempItem.transform.localScale = selectedLocalScale;
            }
            else if (tempItem.transform.localScale == selectedLocalScale)
            {
                tempItem.transform.localScale = defaultLocalScale;
            }
        }

        MenuPointer.transform.position = new Vector3(MenuPointer.transform.position.x, MenuVoices[menuIndex].transform.position.y, MenuPointer.transform.position.z);
        SoundManager.instance.PlaySingle(switchSelection);
    }

    private void NavigateBestiary()
    {
        for (int i = 0; i < Beasts.Length; i++)
        {
            Beasts[i].enabled = (bestiaryNavIndex == i);
            BeastsTexts[i].gameObject.SetActive((bestiaryNavIndex == i));
        }
    }
    private void NavigateBestiary(KeyCode In)
    {
        if (In == KeyCode.RightArrow)
        {
            if (++bestiaryNavIndex >= Beasts.Length) bestiaryNavIndex = 0;
        }
        if (In == KeyCode.LeftArrow)
        {
            if (--bestiaryNavIndex < 0) bestiaryNavIndex = Beasts.Length - 1;
        }

        NavigateBestiary();
    }
}


