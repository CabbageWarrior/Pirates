using UnityEngine;
using System.Collections.Generic;

namespace PiratesNS
{
    //Enemy inherits from MovingObject, our base class for objects that can move, Player also inherits from this.
    public class Enemy : MovingObject
    {
        protected BoxCollider2D boxColliderEnemy;
        protected Animator animator;                //Variable of type Animator to store a reference to the enemy's Animator component.
        private bool skipMove;                      //Boolean to determine whether or not enemy should skip a turn or move this turn.

        // Enumerators
        public enum EnemyType
        {
            Horizontal,     // 0 Movement A => B on X axis
            Vertical,       // 1 Movement A => B on Y axis
            Ranged,         // 2 Movement for Ranged Enemy that rotates

            CustomPatrol    // 4 Custom Movement defined by Inspector
        };

        //Attack Sounds
        [Header("Sounds")]
        public AudioClip attackSound1;                      //First of two audio clips to play when attacking the player.

        [Header("Enemy properties")]
        public EnemyType enemyType;
        public int hp = 1;                                  //Hit points for the enemy. Still not used.
        public Vector2 start;
        public Vector2 end;

        [Header("Horizontal/Vertical only")]
        private float step = 1f;
        public float pA;
        public float pB;
        public bool wayOfMovement;

        [Header("Ranged only")]
        public int maxTicks;
        public LineOfSight EnemyAimingWay;
        public int tick;

        protected List<Transform> _DeadZone = new List<Transform>();
        protected List<Transform> _LaserDeadZone = new List<Transform>();

        //Patrolling
        [Header("Patrolling only")]
        public Transform[] patrolPoints;
        protected int patrolIndex;

        //Start overrides the virtual Start function of the base class. 
        protected override void Start()
        {
            boxColliderEnemy = GetComponent<BoxCollider2D>();
            //Register this enemy with our instance of GameManager by adding it to a list of Enemy objects. 
            //This allows the GameManager to issue movement commands.
            GameManager.instance.AddEnemyToList(this);

            //Get and store a reference to the attached Animator component.
            animator = GetComponent<Animator>();

            if (enemyType == EnemyType.Horizontal)
            {
                pA = this.transform.position.x + 3f;
                pB = this.transform.position.x - 3f;
            }
            else if (enemyType == EnemyType.Vertical)
            {
                pA = this.transform.position.y + 3f;
                pB = this.transform.position.y - 3f;
            }
            //Call the start function of our base class MovingObject.
            base.Start();
            if (enemyType == EnemyType.Ranged)
            {
                ChangeSightAnimation(EnemyAimingWay);
            }
        }

        public virtual void CheckNextCell(out int xDir, out int yDir)
        {
            xDir = 0;
            yDir = 0;
            
            Vector2 newPos;

            switch (enemyType)
            {
                case EnemyType.CustomPatrol: //Patrol defined by level designers.

                    if (transform.position == patrolPoints[patrolIndex].position)
                    {
                        patrolIndex++;
                    }
                    if (patrolIndex >= patrolPoints.Length)
                    {
                        patrolIndex = 0;
                    }

                    xDir = (int)(patrolPoints[patrolIndex].position.x - transform.position.x);
                    yDir = (int)(patrolPoints[patrolIndex].position.y - transform.position.y);
                    break;

                case EnemyType.Horizontal: //Pattern AB_X_Axis

                    if (wayOfMovement == true)
                    {
                        xDir = (int)step;
                        newPos = new Vector2(this.transform.position.x + step, this.transform.position.y);
                    }
                    else
                    {
                        xDir = -(int)step;
                        newPos = new Vector2(this.transform.position.x - step, this.transform.position.y);
                    }
                    if (newPos.x == pA)
                    {
                        wayOfMovement = false;
                    }
                    if (newPos.x == pB)
                    {
                        wayOfMovement = true;
                    }
                    break;

                case EnemyType.Vertical: //Pattern AB_Y_Axis

                    if (wayOfMovement == true)
                    {
                        yDir = (int)step;
                        newPos = new Vector2(this.transform.position.x, this.transform.position.y + step);
                    }
                    else
                    {
                        yDir = -(int)step;
                        newPos = new Vector2(this.transform.position.x, this.transform.position.y - step);
                    }
                    if (newPos.y == pA)
                    {
                        wayOfMovement = false;
                    }
                    if (newPos.y == pB)
                    {
                        wayOfMovement = true;
                    }
                    break;
            }
        }

        public Vector2 GetVectorDirection(LineOfSight aimingDirection)
        {
            Vector2 direction = new Vector2();

            switch (aimingDirection)
            {
                case LineOfSight.down:
                    direction = -transform.up;
                    break;
                case LineOfSight.up:
                    direction = transform.up;
                    break;
                case LineOfSight.right:
                    direction = transform.right;
                    break;
                case LineOfSight.left:
                    direction = -transform.right;
                    break;
            }

            return direction;
        }

        //MoveEnemy is called by the GameManger each turn to tell each Enemy to try to move towards the player.
        public void MoveEnemy()
        {
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;

            CheckNextCell(out xDir, out yDir);

            AttemptMove<Player>(xDir, yDir);
        }
        
        public static void ChangeAimingDirection(ref LineOfSight position)
        {
            switch (position)
            {
                case LineOfSight.down:
                    position = LineOfSight.left;
                    break;
                case LineOfSight.left:
                    position = LineOfSight.up;
                    break;
                case LineOfSight.up:
                    position = LineOfSight.right;
                    break;
                case LineOfSight.right:
                    position = LineOfSight.down;
                    break;
            }
        }

        //OnCantMove is called if Enemy attempts to move into a space occupied by a Player, it overrides the OnCantMove function of MovingObject
        //and takes a generic parameter T which we use to pass in the component we expect to encounter, in this case Player
        protected override void OnCantMove<T>(T component)
        {
            //Declare hitPlayer and set it to equal the encountered component.
            Player hitPlayer = component as Player;

            //Set the attack trigger of animator to trigger Enemy attack animation.
            animator.SetTrigger("Attack");

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx(attackSound1);

            hitPlayer.ExecuteGameOver();
        }

        public void AttemptAttack(int xDir, int yDir, Player player, out bool isStillAlive)
        {
            //Hit will store whatever our linecast hits when Move is called.
            RaycastHit2D hit;
            Vector2 end;
            isStillAlive = true;

            //Set canMove to true if Move was successful, false if failed.
            bool canMove = CanMove(xDir, yDir, out hit, out end);

            //Check if nothing was hit by linecast
            if (hit.transform == null)
                //If nothing was hit, return and don't execute further code.
                return;

            //Get a component reference to the component of type T attached to the object that was hit
            Player hitComponent = hit.transform.GetComponent<Player>();

            //If canMove is false and hitComponent is not equal to null, meaning MovingObject is blocked and has hit something it can interact with.
            if (!canMove && object.Equals(hitComponent, player))
            {
                isStillAlive = false;

                //Call the OnCantMove function and pass it hitComponent as a parameter.
                OnCantMove(player);
            }
        }
        
        public void TryToKillPlayer(Player player, out bool isStillAlive)
        {
            //Declare variables for X and Y axis move directions, these range from -1 to 1.
            //These values allow us to choose between the cardinal directions: up, down, left and right.
            int xDir = 0;
            int yDir = 0;

            CheckNextCell(out xDir, out yDir);

            if (this is PatrollingEnemy)
            {
                AttemptAttack(xDir, yDir, player, out isStillAlive);
            }
            else
            {
                isStillAlive = player.isStillAlive;
            }
        }
    }
}
