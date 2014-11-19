using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Enemy : Base
{
    protected bool moving = false;
    protected bool attacking = false;

    public Vector3 target;
    protected Vector2 nextStep;
    protected float moveSpeed = 3.0f;

    protected float left;
    protected float top;

    void Start()
    {
        target = transform.position;
        moving = false;
    }

    void Update()
    {
        if(Turns.instance.StartTurn(this) && !moving && !attacking )
        {

            Attack();
            Move();

            if (nextStep == Vector2.zero)
            {
                Turns.instance.EndTurn(this);
                Stamina = MaxStamina;
            }
        }
        
        if (moving && transform.position == target)
        {
            moving = false;
            Position = transform.position;
        }

        if (HitPoints <= 1)
        {
            Turns.instance.DeleteEnemy(this);
            GameObject.FindGameObjectWithTag("Enemies").GetComponent<EnemyGenerator>().DeleteEnemy(this);
            Destroy(this.gameObject);
        }

        if (!moving && Stamina == 0)
        {
            Turns.instance.EndTurn(this);
            Stamina = MaxStamina;
        }
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public override void Move()
    {
        if (!moving && Stamina >= MoveCost)
        {
            // COSTO DE MOVIMIENTO
            Stamina -= MoveCost;

            nextStep = Pathfinding.instance.NextStep(Player.instance.Position, transform.position);
            //print(nextStep);
            if (nextStep != Vector2.zero)
            {
                moving = true;
                target = nextStep;
            }
        }
        
    }

    public override void Attack()
    {
        if (Stamina >= AttackCost)
        {
            //MEJORAR
            Vector2 playerPos = Player.instance.Position;
            bool attk = false;
            if (playerPos == (Vector2)transform.position + new Vector2(1, 0))
            {
                attk = true;
            }
            if (playerPos == (Vector2)transform.position + new Vector2(-1, 0))
            {
                attk = true;
            }
            if (playerPos == (Vector2)transform.position + new Vector2(0, 1))
            {
                attk = true;
            }
            if (playerPos == (Vector2)transform.position + new Vector2(0, -1))
            {
                attk = true;
            }

            if (attk)
            {
                Stamina -= AttackCost;
                Combat.DealDamage(Player.instance, this);
            }
        }
    }


    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            //print("ADDING TO NEARBY ENEMIES");
            Turns.instance.AddEnemy(this);
        }
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.tag == "Player")
        {
            Turns.instance.DeleteEnemy(this);
        }
    }

}
