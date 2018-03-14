using PiratesNS;
using UnityEngine;

public class RangedEnemy : Enemy
{
    [SerializeField] protected GameObject Deadzone;
    [SerializeField] protected GameObject LaserDeadzone;
    [SerializeField] private TextMesh CountDownMesh;
    /// <summary>
    /// CountDown Tick.
    /// </summary>
    private int CDTick;

    protected override void Start()
    {
        //Force Enemy Type.
        enemyType = EnemyType.Ranged;
    
        //Call the start function of our base class Enemy.
        base.Start();

        //Call custom code for this type.
        ChangeSightAnimation(EnemyAimingWay);
        InstanceLaserDeadZone(EnemyAimingWay);
    }

    public override void CheckNextCell(out int xDir, out int yDir)
    {
        xDir = 0;
        yDir = 0;

        //Pattern RangedEnemy
        boxColliderEnemy.enabled = false;

        end = GetVectorDirection(EnemyAimingWay);

        tick++;
        CDTick = maxTicks - tick;
        int tickbeforechange = maxTicks - 1;
        if (CDTick == 0)
        {
            CDTick = maxTicks;
        }
        CountDownMesh.text = CDTick.ToString();

        if (tick == maxTicks)
        {
            ChangeAimingDirection(ref EnemyAimingWay);
            end = GetVectorDirection(EnemyAimingWay);

            CheckStoneRaycast(ref end, ref EnemyAimingWay);

            ChangeSightAnimation(EnemyAimingWay);
            tick = 0;
        }

        RaycastHit2D Bullet = Physics2D.Raycast(transform.position, end, 9f, blockingLayer);
        if (Bullet.collider == null)
        {
            // Check if hitting the exit door (X).
            Bullet = Physics2D.Raycast(transform.position, end, 9f, exitLayer);
        }

        if (Bullet.transform != null && Bullet.transform.tag == "Player")
        {
            //Set the attack trigger of animator to trigger Enemy attack animation.
            animator.SetTrigger("Attack");

            //Stop the background music.
            SoundManager.instance.musicSource.Stop();

            //Call the RandomizeSfx function of SoundManager passing in the two audio clips to choose randomly between.
            SoundManager.instance.RandomizeSfx(attackSound1);

            Bullet.transform.GetComponent<Player>().ExecuteGameOver();
        }
        boxColliderEnemy.enabled = true;

    }
    public void CheckStoneRaycast(ref Vector2 parEnd, ref LineOfSight parEnemyAimingWay)
    {
        // Check if disable and enable the box collider or, if already off, do nothing.
        bool isBoxColliderToManageHere = boxColliderEnemy.enabled;

        bool isStoneRaycasted;
        RaycastHit2D CheckBlockingLayerObject;
        int aimingDirectionCheck = 0;

        if (isBoxColliderToManageHere)
        {
            boxColliderEnemy.enabled = false;
        }

        do
        {
            CheckBlockingLayerObject = Physics2D.Raycast(transform.position, parEnd, 1f, blockingLayer);

            isStoneRaycasted = CheckBlockingLayerObject && CheckBlockingLayerObject.transform.tag == "Stone";
            if (isStoneRaycasted)
            {
                aimingDirectionCheck++;

                ChangeAimingDirection(ref parEnemyAimingWay);
                parEnd = GetVectorDirection(parEnemyAimingWay);

                // Only walls around this enemy!
                if (aimingDirectionCheck == 3)
                {
                    break;
                }
            }

        } while (isStoneRaycasted);

        if (isBoxColliderToManageHere)
        {
            boxColliderEnemy.enabled = true;
        }
    }

    public void InstanceLaserDeadZone(LineOfSight parEnemyAimingWay)
    {
        for (int i = 1; i < 9; i++)
        {
            Transform _TempLaserDeadZone = Instantiate(LaserDeadzone.transform, this.transform.position, Quaternion.identity);
            Vector3 _TempEndPosition = new Vector3();
            switch (parEnemyAimingWay)
            {
                case LineOfSight.down:
                    _TempEndPosition = _TempLaserDeadZone.position;
                    _TempEndPosition.y -= i;
                    _TempLaserDeadZone.position = _TempEndPosition;
                    _TempLaserDeadZone.GetChild(0).GetChild(0).Rotate(0, 0, 90);
                    break;
                case LineOfSight.left:
                    _TempEndPosition = _TempLaserDeadZone.position;
                    _TempEndPosition.x -= i;
                    _TempLaserDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.up:
                    _TempEndPosition = _TempLaserDeadZone.position;
                    _TempEndPosition.y += i;
                    _TempLaserDeadZone.GetChild(0).GetChild(0).Rotate(0, 0, 90);
                    _TempLaserDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.right:
                    _TempEndPosition = _TempLaserDeadZone.position;
                    _TempEndPosition.x += i;
                    _TempLaserDeadZone.position = _TempEndPosition;
                    break;
            }
            RaycastHit2D checkCollision;
            checkCollision = Physics2D.Linecast(_TempLaserDeadZone.position, _TempLaserDeadZone.position);
            if (checkCollision.transform != null)
            {
                if (checkCollision.transform.tag == "Stone" || checkCollision.transform.tag == "Enemy")
                {
                    Destroy(_TempLaserDeadZone.gameObject);
                    break;
                }
                else
                {
                    _TempLaserDeadZone.GetComponent<BoxCollider2D>().enabled = true;
                }
            }

            if (_TempLaserDeadZone != null)
            {
                _TempLaserDeadZone.position = _TempEndPosition;
                _LaserDeadZone.Add(_TempLaserDeadZone);
            }
        }
    }

    public void InstanceDeadZone(LineOfSight parEnemyAimingWay)
    {
        Vector3 _TempEndPosition = new Vector3();
        for (int i = 1; i < 9; i++)
        {
            Transform _TempDeadZone = Instantiate(Deadzone.transform, transform.position, Quaternion.identity);
            _TempEndPosition = new Vector3();
            switch (parEnemyAimingWay)
            {
                case LineOfSight.down:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.y -= i;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.left:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.x -= i;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.up:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.y += i;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
                case LineOfSight.right:
                    _TempEndPosition = _TempDeadZone.position;
                    _TempEndPosition.x += i;
                    _TempDeadZone.position = _TempEndPosition;
                    break;
            }
            RaycastHit2D checkCollision;
            checkCollision = Physics2D.Linecast(_TempDeadZone.position, _TempDeadZone.position);
            if (checkCollision.transform != null)
            {
                if (checkCollision.transform.tag == "Stone" || checkCollision.transform.tag == "Enemy")
                {
                    Destroy(_TempDeadZone.gameObject);
                    break;
                }
                else if (checkCollision.transform.tag == "DeadZone")
                {
                    Destroy(_TempDeadZone.gameObject);
                }
                else
                {
                    _TempDeadZone.GetComponent<BoxCollider2D>().enabled = true;
                }
            }

            if (_TempDeadZone != null)
            {
                _TempDeadZone.position = _TempEndPosition;
                _DeadZone.Add(_TempDeadZone);
            }
        }
    }
}
