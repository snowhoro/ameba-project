using UnityEngine;
using System.Collections;

public class RedAmeba : Enemy 
{
    private Animator anim;

    void Start()
    {
        target = transform.position;
        moving = false;
        anim = GetComponent<Animator>();
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
                anim.SetFloat("MoveX", (nextStep - (Vector2)transform.position).x);
                anim.SetFloat("MoveY", (nextStep - (Vector2)transform.position).y);
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
                anim.SetFloat("MoveX", 1);
                anim.SetFloat("MoveY", 0);
                attk = true;
            }
            if (playerPos == (Vector2)transform.position + new Vector2(-1, 0))
            {
                anim.SetFloat("MoveX", -1);
                anim.SetFloat("MoveY", 0);
                attk = true;
            }
            if (playerPos == (Vector2)transform.position + new Vector2(0, 1))
            {
                anim.SetFloat("MoveX", 0);
                anim.SetFloat("MoveY", 1);
                attk = true;
            }
            if (playerPos == (Vector2)transform.position + new Vector2(0, -1))
            {
                anim.SetFloat("MoveX", 0);
                anim.SetFloat("MoveY", -1);
                attk = true;
            }

            if (attk)
            {
                anim.SetTrigger("Attack");
                Stamina -= AttackCost;
                Combat.instance.DealDamage(Player.instance, this);
            }
        }
    }
}
