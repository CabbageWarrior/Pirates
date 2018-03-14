using PiratesNS;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public static bool Go = false;
    [Header("IMPOSTARE I VALORI A n.5")]
    public CustomVector2[] Proiettile;
    [Header("NON TOCCATE")]
    [SerializeField]
    private GameObject DeadZone;
    private Player player;
    [SerializeField] private GameObject PlayerREF;
    [SerializeField] private GameObject Object;
    [System.Serializable]
    public struct CustomVector2
    {
        public float x;
        public float y;
        public float Turni;

        public CustomVector2(float xx, float yy, float Turn)
        {
            x = xx;
            y = yy;
            Turni = Turn;
        }
    }

    private GameObject[] medusaFloors;
    private GameObject[] cannons;

    public AudioClip woodDestroyAudioClip;

    void Start()
    {
        Go = false;

        medusaFloors = GameObject.FindGameObjectsWithTag("MedusaFloor");
        cannons = GameObject.FindGameObjectsWithTag("Soda");

        for (int i = 0; i < Proiettile.Length; i++)
        {
            if ((int)Proiettile[i].Turni == 0)
            {
                Transform _Temp = Instantiate(DeadZone.transform, new Vector3(Proiettile[i].x, Proiettile[i].y, -1), Quaternion.identity);
                _Temp.position = new Vector3(Proiettile[i].x, Proiettile[i].y, -1);
            }
        }
    }
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Soda")
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Go)
        {
            for (int i = 0; i < Proiettile.Length; i++)
            {
                switch ((int)Proiettile[i].Turni)
                {
                    case 0:
                        SoundManager.instance.PlaySingle(woodDestroyAudioClip);

                        Transform _TempTentacle = Instantiate(Object.transform, this.transform.position, Quaternion.identity);
                        _TempTentacle.position = new Vector3(Proiettile[i].x, Proiettile[i].y, -1);
                        Proiettile[i].Turni -= 1;

                        foreach (GameObject item in medusaFloors)
                        {
                            if (item.transform.position.x >= Proiettile[i].x - .5f &&
                                item.transform.position.x <= Proiettile[i].x + .5f &&
                                item.transform.position.y >= Proiettile[i].y - .5f &&
                                item.transform.position.y <= Proiettile[i].y + .5f)
                            {
                                item.GetComponent<SpriteRenderer>().enabled = false;
                            }
                        }

                        foreach (GameObject item in cannons)
                        {
                            if (item != null)
                            {
                                if (item.transform.position.x >= Proiettile[i].x - .5f &&
                                    item.transform.position.x <= Proiettile[i].x + .5f &&
                                    item.transform.position.y >= Proiettile[i].y - .5f &&
                                    item.transform.position.y <= Proiettile[i].y + .5f)
                                {
                                    item.GetComponentInChildren<SpriteRenderer>().enabled = false;
                                }
                            }
                        }

                        break;
                    case 1:
                        Transform _Temp = Instantiate(DeadZone.transform, new Vector3(Proiettile[i].x, Proiettile[i].y, -1), Quaternion.identity);
                        _Temp.position = new Vector3(Proiettile[i].x, Proiettile[i].y, -1);
                        Proiettile[i].Turni -= 1;
                        break;
                    default:
                        Proiettile[i].Turni -= 1;
                        break;
                }
            }
            Go = false;
        }
    }
}
