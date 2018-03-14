using PiratesNS;
using UnityEngine;

public class EndLevel : MonoBehaviour
{
    void Start()
    {
        GameManager.instance.GoToNextScene(5, 0);
    }
}
