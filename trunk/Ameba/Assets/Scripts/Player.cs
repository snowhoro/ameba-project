using UnityEngine;
using System.Collections;

public class Player : Base
{

    public float moveSpeed = 10f;
    private float moveX;
    private float moveY;
    private Vector2 faceDir;

    private bool moving = false;
    private bool attacking = false;

    public Vector3 target;
    private Animator anim;

    private static Player _instance;
    public static Player instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<Player>();

                DontDestroyOnLoad(_instance.gameObject);
            }

            return _instance;
        }
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            if (this != _instance)
                Destroy(this.gameObject);
        }
    }

    void Start()
    {        
        target = transform.position;
        anim = gameObject.GetComponent<Animator>();
        Position = transform.position;
    }

    void Update()
    {
        moveX = moveY = 0;
        if (Turns.instance.StartTurn(this) && !moving && !attacking)
        {
			SkipTurn();
            Attack();
            Move();
        }        

        if (moving && transform.position == target)
        {
            moving = false;
            Position = transform.position;
            Stamina -= MoveCost;
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

	public void SkipTurn()
	{
        if (Input.GetKeyDown(KeyCode.Space))
            Stamina = 0;
	}

    public override void Move()
    {
        if (Stamina >= MoveCost)
        {
            if (Input.GetButton("Horizontal"))
            {
                moveX = Input.GetAxisRaw("Horizontal");
                CheckNextStep();
            }
            else if (Input.GetButton("Vertical"))
            {
                moveY = Input.GetAxisRaw("Vertical");
                CheckNextStep();
            }
        }
    }
    public void CheckNextStep()
    {
        faceDir = new Vector2(moveX, moveY);
        FaceDirection();

        RaycastHit2D[] hits;
        hits = Physics2D.RaycastAll(transform.position, faceDir , 1.0f);
        bool wall = false;
        foreach (RaycastHit2D hit in hits)
        {
            if (hit.distance > 0 && !hit.collider.isTrigger)
                wall = true;
        }

        if (!wall)
        {
            target = new Vector3(Position.x + moveX, Position.y + moveY, transform.position.z);
            moving = true;
        }
    }

    private void FaceDirection()
    {
        anim.SetFloat("MoveX", moveX);
        anim.SetFloat("MoveY", moveY);
    }

    public override void Attack()
    {
		if (Input.GetKeyDown (KeyCode.LeftControl)) 
		{
			RaycastHit2D[] hits;
			hits = Physics2D.RaycastAll(transform.position, faceDir , 1.0f);
			foreach (RaycastHit2D hit in hits)
			{
				if (hit.collider.tag == "Enemy" && !hit.collider.isTrigger)
				{
					anim.SetTrigger("AttackSide");
					Combat.DealDamage(hit.collider.GetComponent<Base>(),this);
					Stamina -= AttackCost;
				}
			}
		}


    }

	private void AttackStamina()
	{
		//Stamina -= AttackCost;
	}
}
