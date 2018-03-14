using UnityEngine;
using System.Collections;
using DG.Tweening;

namespace PiratesNS
{
    //Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
    public class Player : MovingObject
    {
        // Enumeratori
        public enum Gender
        {
            Male,
            Female
        };

        [Header("Player Infos")]
        public float restartLevelDelay = 1f;        //Delay time in seconds to restart level.
        public int pointsPerFood = 10;              //Number of points to add to player food points when picking up a food object.
        public int pointsPerSoda = 20;              //Number of points to add to player food points when picking up a soda object.
        public int attackDamage = 1;                //How much damage a player does to a wall when chopping it.
        public static Gender gender;
        private bool hasSword = false;              //Check if the player has a sword. (REMOVED)
        private float swordDestroyMinScale = 1f;
        private float swordDestroyMaxScale = 4f;
        private float swordDestroyAnimationTime = 1.5f;

        [Header("Player Sounds")]
        public AudioClip moveSound1;                //1 of 2 Audio clips to play when player moves.
        public AudioClip moveSound2;                //2 of 2 Audio clips to play when player moves.
        public AudioClip eatSound1;                 //1 of 2 Audio clips to play when player collects a food object.
        public AudioClip eatSound2;                 //2 of 2 Audio clips to play when player collects a food object.
        public AudioClip drinkSound1;               //1 of 2 Audio clips to play when player collects a soda object.
        public AudioClip drinkSound2;               //2 of 2 Audio clips to play when player collects a soda object.
        public AudioClip gameOverSound;             //Audio clip to play when player dies.
        private TextMesh totalStepsText;            //UI Text to display current player money total.
        private TextMesh levelStepsText;            //UI Text to display current player steps total.

        [Header("Animators")]
        public RuntimeAnimatorController maleCharacterWithSwordAnimator;
        public RuntimeAnimatorController maleCharacterWithoutSwordAnimator;
        public RuntimeAnimatorController femaleCharacterWithSwordAnimator;
        public RuntimeAnimatorController femaleCharacterWithoutSwordAnimator;
        private Animator animator;                  //Used to store a reference to the Player's animator component.

        private int totalSteps;                     //Used to store player turns total during level.
        private int levelSteps;                     //Used to store player steps during level.
        [Header("Turns and Moves")]
        public KeyCode turnJumper = KeyCode.Space;
        public Vector2 old_Coordinate;
        public Vector2 new_Coordinate;

        public bool isStillAlive = true;

        [Header("Boss Level Only")]
        public AudioClip cannonShot;
        public AudioClip bossScream;

        //Start overrides the Start function of MovingObject
        protected override void Start()
        {
            GetComponent<BoxCollider2D>().enabled = true;
            //Get a component reference to the Player's animator component
            animator = GetComponent<Animator>();

            if (gender == Gender.Male)
            {
                animator.runtimeAnimatorController = (hasSword ? maleCharacterWithSwordAnimator : maleCharacterWithoutSwordAnimator);
            }
            else
            {
                animator.runtimeAnimatorController = (hasSword ? femaleCharacterWithSwordAnimator : femaleCharacterWithoutSwordAnimator);
            }

            //Get the current food point total stored in GameManager.instance between levels.
            totalSteps = GameManager.instance.playerTotalMoney;
            levelSteps = 0;

            GameObject totalStepsGameObject = GameObject.Find("TotalStepsText");
            if (totalStepsGameObject != null)
            {
                totalStepsText = totalStepsGameObject.GetComponent<TextMesh>();
                totalStepsText.text = totalSteps.ToString();
            }

            GameObject levelStepsGameObject = GameObject.Find("LevelStepsText");
            if (levelStepsGameObject != null)
            {
                levelStepsText = levelStepsGameObject.GetComponent<TextMesh>();
                levelStepsText.text = levelSteps.ToString();
            }

            //Call the Start function of the MovingObject base class.
            base.Start();
        }

        private void Update()
        {
            if (GameManager.instance == null)
            {
                Debug.LogError("Something broken in GameManager!");
                return;
            }

            //If it's not the player's turn, exit the function.
            if (!GameManager.instance.playersTurn) return;

            if (GameManager.instance.state == GameManager.State.Play && isStillAlive && (
                Input.GetKeyDown(KeyCode.UpArrow) ||
                Input.GetKeyDown(KeyCode.DownArrow) ||
                Input.GetKeyDown(KeyCode.LeftArrow) ||
                Input.GetKeyDown(KeyCode.RightArrow) ||
                Input.GetKeyDown(turnJumper)
                ))
            {
                if (Input.GetKeyDown(turnJumper))
                {
                    animator.SetTrigger("LongIdle");
                }

                int horizontal = 0;     //Used to store the horizontal move direction.
                int vertical = 0;       //Used to store the vertical move direction.

                //Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
                horizontal = (int)(Input.GetAxisRaw("Horizontal"));

                //Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
                vertical = (int)(Input.GetAxisRaw("Vertical"));
                old_Coordinate = this.transform.position;
                //Check if moving horizontally, if so set vertical to zero.
                if (animator.GetCurrentAnimatorStateInfo(0).IsName("LongIdle"))
                {
                    horizontal = 0;
                    vertical = 0;
                }
                else if (horizontal != 0)
                {
                    vertical = 0;
                }

                //Check if we have a non-zero value for horizontal or vertical
                if (horizontal != 0 || vertical != 0 || Input.GetKeyDown(turnJumper))
                {
                    bool isStillAlive = true;
                    bool proceedWithTheTurn = true;

                    //[Verza] Add check on enemies that can kill the player.
                    RaycastHit2D hit;
                    Vector2 end;
                    CanMove(horizontal, vertical, out hit, out end);
                    if (hit.transform != null)
                    {
                        if (hit.transform.GetComponent<Enemy>() != null)
                        {
                            foreach (Enemy enemy in GameManager.instance.enemies)
                            {
                                if (enemy is PatrollingEnemy)
                                {
                                    enemy.TryToKillPlayer(this, out isStillAlive);
                                }
                                if (!isStillAlive) break;
                            }
                        }
                        else if (hit.transform.CompareTag("Stone"))
                        {
                            proceedWithTheTurn = false;
                        }
                    }

                    if (isStillAlive && proceedWithTheTurn)
                    {
                        GameObject[] DestroyDeadZone;
                        GameObject[] DestroyLaserDeadZone;
                        DestroyDeadZone = GameObject.FindGameObjectsWithTag("DeadZone");
                        DestroyLaserDeadZone = GameObject.FindGameObjectsWithTag("LaserDeadZone");
                        if (DestroyLaserDeadZone.Length > 0 || DestroyDeadZone.Length > 0)
                        {
                            for (int i1 = 0; i1 < DestroyDeadZone.Length; i1++)
                            {
                                Destroy(DestroyDeadZone[i1].gameObject);
                            }
                            for (int i1 = 0; i1 < DestroyLaserDeadZone.Length; i1++)
                            {
                                Destroy(DestroyLaserDeadZone[i1].gameObject);
                            }
                        }

                        //Call AttemptMove passing in the generic parameter Enemy, since that is what Player may interact with if they encounter one (by attacking it)
                        //Pass in horizontal and vertical as parameters to specify the direction to move Player in.
                        AttemptMove<Enemy>(horizontal, vertical);
                    }
                }
            }
        }

        //AttemptMove overrides the AttemptMove function in the base class MovingObject
        //AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
        protected override void AttemptMove<T>(int xDir, int yDir)
        {
            //Every time player moves, subtract from food points total.
            levelSteps++;

            //Update food text display to reflect current score.
            levelStepsText.text = levelSteps.ToString();

            //Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
            base.AttemptMove<T>(xDir, yDir);

            new_Coordinate = transform.position;
            //Set the playersTurn boolean of GameManager to false now that players turn is over.

            GameManager.instance.playersTurn = false;
            TestScript.Go = true;
        }

        //OnCantMove overrides the abstract function OnCantMove in MovingObject.
        //It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
        protected override void OnCantMove<T>(T component)
        {
            if (isStillAlive && component is Enemy)
            {
                //Set hitWall to equal the component passed in as a parameter.
                Enemy hitEnemy = component as Enemy;

                hitEnemy.TryToKillPlayer(this, out isStillAlive);

                if (isStillAlive)
                {
                    Vector2 newEnemySight = transform.position - hitEnemy.transform.position;

                    ChangeSightAnimation((int)newEnemySight.x, (int)newEnemySight.y);
                    hitEnemy.AttemptAttack((int)newEnemySight.x, (int)newEnemySight.y, this, out isStillAlive);
                }
            }
        }

        //TNT =GameObject.FindGameObjectsWithTag("Soda");
        // MaxCounter = TNT.Length;
        //OnTriggerEnter2D is sent when another object enters a trigger collider attached to this object (2D physics only).
        private void OnTriggerEnter2D(Collider2D other)
        {
            //Check if the tag of the trigger collided with is Exit.
            if (other.tag == "Exit")
            {
                LevelFinished();
            }
            else if (other.tag == "Tentacle")
            {
                ExecuteGameOver();
            }
            if (other.tag == "Soda")
            {
                StartCoroutine(CannonCoroutine(other.gameObject));
            }
        }

        IEnumerator CannonCoroutine(GameObject otherGameObject)
        {
            otherGameObject.GetComponent<BoxCollider2D>().enabled = false;

            otherGameObject.GetComponentInChildren<Animator>().SetTrigger("Shoot");
            otherGameObject.GetComponentInChildren<AudioSource>().Play();

            yield return new WaitForSeconds(.5f);

            GameObject.Find("Placeholder10su10").GetComponent<Animator>().SetTrigger("TakeDamage");
            SoundManager.instance.PlaySingle(bossScream);

            yield return new WaitForSeconds(1);

            Destroy(otherGameObject);

            yield return null;
        }

        public void LevelFinished()
        {
            //Invoke the Restart function to start the next level with a delay of restartLevelDelay (default 1 second).
            GetComponent<BoxCollider2D>().enabled = false;

            GameManager.instance.playerTotalMoney += levelSteps;


            GameObject coinContainer = GameObject.Find("CoinContainer");
            Transform coinTransform;

            if (coinContainer != null)
            {
                Vector3 mainCameraPosition = GameObject.Find("Main Camera").transform.position;
                Vector3 coinFinalPosition = new Vector3(mainCameraPosition.x, mainCameraPosition.y, 0);
                if (levelSteps <= GameManager.instance.MaxStepsForToken)
                {
                    coinTransform = GameObject.Find("CoinGold").transform;
                }
                else
                {
                    coinTransform = GameObject.Find("CoinSilver").transform;
                }
                coinTransform.DOScale(1, 1);
                coinTransform.DOMove(coinFinalPosition, 1);

                new WaitForSeconds(2);
            }

            GameManager.instance.GoToNextScene(restartLevelDelay, levelSteps);

            //Disable the player object since level is over.
            enabled = false;
        }

        //ExecuteGameOver ends the game.
        public void ExecuteGameOver()
        {
            isStillAlive = false;
            StartCoroutine(ExecuteGameOverCoroutine());
        }

        private IEnumerator ExecuteGameOverCoroutine()
        {
            //[Verza]   If exists the CameraShake component in Main Camera
            //          then shake the camera when the player dies,
            //          otherwise no shake.
            CameraShake cameraShake = GameObject.Find("Main Camera").GetComponent<CameraShake>();
            if (cameraShake != null)
            {
                // Call al metodo di Shake.
                cameraShake.ShakeCamera(1f, 0.1f);
            }
            animator.SetTrigger("Die");

            yield return new WaitForSeconds(2f);

            //Call the PlaySingle function of SoundManager and pass it the gameOverSound as the audio clip to play.
            SoundManager.instance.PlaySingle(gameOverSound);

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the GameOver function of GameManager.
            GameManager.instance.GameOver();

            yield return null;
        }
        public void GetGender(bool SelectedGender)
        {
            if (SelectedGender)
            {
                gender = Gender.Male;
            }
            else
            {
                gender = Gender.Female;
            }
        }
    }
}
