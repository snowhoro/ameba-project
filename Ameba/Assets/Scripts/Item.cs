using UnityEngine;
using System.Collections;

public class Item : MonoBehaviour 
{

    public string itemName;

    public int maxNumber;
    public int number;

    public int dropRate;
    public int dropNumber;

    public Texture2D img;

    private bool pickup = false;

	public virtual void Use(){}
    public virtual void Discard() 
    {
        number--;
        //if()
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            pickup = true;
        }

    }

    void OnTriggerStay2D(Collider2D coll)
    {
        if(pickup)
        {
            Inventory inven = GameObject.FindGameObjectWithTag("Inventory").GetComponent<Inventory>();
            foreach (Item item in inven.itemList)
            {
                if(item.itemName == this.itemName && item.maxNumber != item.number)
                {
                    item.number++;
                    pickup = false;
                    Destroy(this.gameObject);
                    return;
                } 
            }
        }
    }
}
