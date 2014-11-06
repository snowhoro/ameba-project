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
            anim.SetTrigger("BackToIdle");
            Turns.instance.EndTurn(this);
        }
    }

    void FixedUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
    }

	public void SkipTurn()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			Turns.instance.EndTurn (this);
	}

    public void Move()
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
        /*
        if (faceDir.x != 0)
            anim.SetFloat("MoveX", moveX);
        else if (faceDir.y != 0)
            anim.SetFloat("MoveY", moveY);*/
    }

    public void Attack()
    {
		if (Input.GetKeyDown (KeyCode.LeftControl)) 
		{
			// CAMBIA ESTO CACA
			Combat.instance.DealDamage(GameObject.FindGameObjectWithTag("Enemy").GetComponent<Base>(),this);
			Turns.instance.EndTurn (this);
		}


    }
}
