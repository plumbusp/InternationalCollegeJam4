using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class HamsterMovement : MonoBehaviour
{
	private Rigidbody2D rb;
	public float moveSpeed;
	public Camera cam;

	Vector2 movement;
	Vector2 mousePos;

	Animator animator;

	void Start()
	{
		rb = GetComponent<Rigidbody2D>();
		animator = GetComponent<Animator>();
    }

	void Update()
	{
		// Input
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		animator.SetFloat("x", movement.x);
        animator.SetFloat("y", movement.y);

        /*animator.SetFloat("Horizontal",movement.x);
		animator.SetFloat("Vertical",movement.y);
		animator.SetFloat("Speed",movement.sqrMagnitude);*/
    }

	// Melhor para trabalhar com física	
	void FixedUpdate()
	{
		// Movement
		//rb.MovePosition(rb.position + movement * moveSpeed * Time.fixedDeltaTime);
		rb.MovePosition(rb.position + movement.normalized * moveSpeed * Time.fixedDeltaTime);
	}
}