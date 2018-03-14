using PiratesNS;
using System.Collections;
using UnityEngine;

public class BossLevel : MonoBehaviour
{
    [SerializeField]
    static public GameObject[] TNT;
    [SerializeField]
    private GameObject BossNotStatic;

    static int MaxCounter;
    public int Counter = 0;
    public Player player;

    private Animator BossNotStaticAnimator;
    private bool levelFinished = false;
    
    void Start()
    {
        TNT = GameObject.FindGameObjectsWithTag("Soda");
        MaxCounter = TNT.Length;

        BossNotStaticAnimator = BossNotStatic.GetComponent<Animator>();
    }

    void Update()
    {
        if (MaxCounter <= Counter)
        {
            if (!levelFinished)
            {
                levelFinished = true;
                Counter = 0;
                BossNotStaticAnimator.SetTrigger("Die");

                StartCoroutine(FinishWithDelay());
            }
        }
        else
            CannonTriggered();
    }

    public IEnumerator FinishWithDelay()
    {
        player.enabled = false;
        yield return new WaitForSeconds(1);
        player.LevelFinished();
        yield return null;
    }

    public void CannonTriggered()
    {
        Counter = 0;
        for (int i = 0; i < TNT.Length; i++)
        {
            if (TNT[i] == null)
            {
                Counter++;
            }
        }
    }
}
