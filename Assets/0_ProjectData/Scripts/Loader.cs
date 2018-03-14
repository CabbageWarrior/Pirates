using UnityEngine;

namespace PiratesNS
{
    public class Loader : MonoBehaviour
    {
        // Public

        /// <summary>
        /// GameManager prefab to instantiate.
        /// </summary>
        public GameObject gameManager;

        /// <summary>
        /// SoundManager prefab to instantiate.
        /// </summary>
        public GameObject soundManager;

        /// <summary>
        /// Title of the scene.
        /// </summary>
        public string sceneTitle;

        /// <summary>
        /// Subtitle of the scene.
        /// </summary>
        public string sceneSubtitle;

        /// <summary>
        /// Diary infos about this area.
        /// </summary>
        public string sceneChapterText;

        /// <summary>
        /// Max steps for this room to win the golden coin.
        /// </summary>
        public int maxStepsForToken;

        /// <summary>
        /// Delay between each Player turn. Default: 0.1f.
        /// </summary>
        [Tooltip("Default: 0.1")]
        public float turnDelay = 0.1f;

        /// <summary>
        /// Customized text for when the player dies.
        /// </summary>
        public string deadText;

        void Awake()
        {
            //Check if a GameManager has already been assigned to static variable GameManager.instance or if it's still null
            if (GameManager.instance == null)

                //Instantiate gameManager prefab
                Instantiate(gameManager);

            //[Verza] Added level title in order to change the scene name dynamically on new scenes load.
            GameManager.instance.Title = (sceneTitle != null && sceneTitle != string.Empty ? sceneTitle : "Unnamed Level");
            GameManager.instance.Subtitle = (sceneSubtitle != null && sceneSubtitle != string.Empty ? sceneSubtitle : "Unnamed Room");
            GameManager.instance.ChapterText = (sceneChapterText != null && sceneChapterText != string.Empty ? sceneChapterText : "Untold Story.");
            GameManager.instance.MaxStepsForToken = maxStepsForToken;
            GameManager.instance.turnDelay = turnDelay;
            GameManager.instance.deadText = (deadText != null && deadText != string.Empty ? deadText : "Oh no, hai fallito!\nRiprova con \"R\" e ricorda\ndi stare attento!");
            GameManager.setRestartAvailable = false;
            //Check if a SoundManager has already been assigned to static variable GameManager.instance or if it's still null
            if (SoundManager.instance == null)

                //Instantiate SoundManager prefab
                Instantiate(soundManager);
        }
    }
}