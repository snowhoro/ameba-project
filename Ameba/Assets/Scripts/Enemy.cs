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

    private float left;
    private float top;
    public Texture2D blackBar;
    public Texture2D redBar;
    public float healthBarWidth;
    public float healthBarHeight;

    void Start()
    {
        target = transform.position;
        moving = false;
    }

    void OnGUI()
    {
        
        Vector3 healthBarWorldPosition = transform.position + new Vector3(0.0f, 0.5f, 0.0f);
        Vector3 healthBarScreenPosition = Camera.main.WorldToScreenPoint(healthBarWorldPosition);

        left = healthBarScreenPosition.x - (healthBarWidth / 2);
        top = Screen.height - (healthBarScreenPosition.y);

        GUI.DrawTexture(new Rect(left, top, healthBarWidth, healthBarHeight), blackBar, ScaleMode.StretchToFill);
        GUI.DrawTexture(new Rect(left, top, (HitPoints * healthBarWidth) / MaxHitPoints, healthBarHeight), redBar, ScaleMode.StretchToFill);
        
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
        }

        if (HitPoints <= 1)
        {
            Turns.instance.DeleteEnemy(this);
            Destroy(this.gameObject);
        }

        if (Stamina == 0)
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
