using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Enemy : Base
{
    private bool moving = false;
    private bool attacking = false;

    private Vector3 target;
    private Vector2 nextStep;
    private float moveSpeed = 3.0f;
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
            if(nextStep == Vector2.zero)
                Turns.instance.EndTurn(this);
        }
        
        if (moving && transform.position == target)
        {
            moving = false;
            Position = transform.position;
            Turns.instance.EndTurn(this);
        }

		if (HitPoints <= 1) {
			Turns.instance.DeleteEnemy(this);
						Destroy (this.gameObject);

				}
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

    public void Move()
    {
        if (!moving)
        {
            nextStep = Pathfinding.instance.NextStep(Player.instance.Position, transform.position);

            if (nextStep != Vector2.zero)
            {
                moving = true;
                target = nextStep;
            }
        }
        
    }

    public void Attack()
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
			Base player = GameObject.FindGameObjectWithTag("Player").GetComponent<Base>();
			print ("ATTACKING");
			Combat.instance.DealDamage(player,this);
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
