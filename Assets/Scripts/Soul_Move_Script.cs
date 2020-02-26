using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Soul_Move_Script : MonoBehaviour
{
    private SpriteRenderer rend;

    public float pa;            // relative strength of repulsion from other agents - 2
	public float c;     // relative strength of attraction to the n nearest neighbours - 1.05
	public float ps;        // relative strength of repulsion from the shepherd - 1
	public float h;         // relative strength of proceeding in the previous direction - 0.5
	public float e;
	public float speed;
	public List<GameObject> agn;
	float init_e;
	

	Vector2 H = new Vector2(0, 0);
	Vector2 LCM;
	Vector2 A;
	Vector2 Rs;
	Vector2 C;
	Vector2 S;
	Vector2 Ra;
	Vector2 E;

	Rigidbody2D body;
	GameObject Demon;
	bool demon_near = false;

	// Start is called before the first frame update
	void Start()
	{
        rend = GetComponent<SpriteRenderer>();

        init_e = e;
		body = GetComponent<Rigidbody2D>();
		//transform.position = new Vector2(6f * (Random.value - 0.5f), 6f * (Random.value - 0.5f));        
	}

	// Update is called once per frame
	void FixedUpdate()
	{
        rend.sortingOrder = 30000 +  (int)(transform.position.y * -1000);

        //Local center of mass position
        LCM = new Vector2(0, 0);
		int counter = 0;
		int l = agn.Count;
		for (int i = 0; i < l; i++)
		{
			LCM += agn[i].GetComponent<Rigidbody2D>().position;
			counter++;
		}
		if (counter != 0)
			LCM /= counter;
		else
			LCM = GetComponent<Rigidbody2D>().position;
		
		// Agent and Shepherd position
		A = GetComponent<Rigidbody2D>().position;

		if (demon_near && Demon != null)
		{
			//Distance between Agent and Shepherd
			S = Demon.GetComponent<Rigidbody2D>().position;
			Rs = A - S;
			C = LCM - A;
		}
		else
		{
			Rs = Vector2.zero;
			C = Vector2.zero;				
		}

		// Local repulsion of Agent from nearest neighbours
		Ra = Vector2.zero;
		for (int i = 0; i < l; i++)
		{
			Ra += (GetComponent<Rigidbody2D>().position - agn[i].GetComponent<Rigidbody2D>().position).normalized;
		}

		//Error term
		E = new Vector2(10f * (Random.value - 0.5f), 10f * (Random.value - 0.5f));
		
		H = h * H.normalized + c * C.normalized + pa * Ra.normalized + ps * Rs.normalized + E * e;

		//Vector3 finalMovement = A + cur_speed * H.normalized + e * E.normalized;
		//transform.position = A + cur_speed * H.normalized + e * E.normalized;

		//body.velocity = H * speed;

		body.AddForce(H * speed);

	}

	private void OnTriggerStay2D(Collider2D collider)
	{

		if (collider.gameObject.tag == "Demon")
		{
			Demon = collider.gameObject;
			demon_near = true;
			e = 0;
		}
	}

	private void OnTriggerEnter2D(Collider2D collider)
	{ 
		if (collider.gameObject.tag == "Soul" && collider.isTrigger)
		{
			agn.Add(collider.gameObject);
		}
	}
	
	private void OnTriggerExit2D(Collider2D collider)
	{
		if (collider.gameObject.tag == "Demon")
		{
			Demon = null;
			demon_near = false;
			e = init_e;

			//body.velocity = new Vector2(0,0);
		}
		if (collider.gameObject.tag == "Soul" && collider.isTrigger)
		{
			agn.Remove(collider.gameObject);
		}

	}
	
	private void OnCollisionEnter2D(Collision2D collider)
	{
		/*
		if (collider.gameObject.tag == "Pit")
		{
			Destroy(gameObject);
			Main.Score += 1;
		}
		*/

		if (collider.gameObject.tag == "Killer")
		{
			Destroy(gameObject);
		}
		
	}
}

