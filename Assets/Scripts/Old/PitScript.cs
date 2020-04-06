using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PitScript : MonoBehaviour
{
	public float latency;
	float lastSoul;
	int currentScore;

	// Start is called before the first frame update
	void Start()
	{
		lastSoul = Time.time;
		currentScore = 0;
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time - lastSoul > latency && currentScore > 0)
		{
			Main.Score += currentScore * currentScore;
			currentScore = 0;
		}
	}

	private void OnCollisionEnter2D(Collision2D collider)
	{
		if (collider.gameObject.tag == "Soul")
		{
			Destroy(collider.gameObject);
			currentScore++;
			lastSoul = Time.time;
		}
	}
}
