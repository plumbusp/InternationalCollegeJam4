using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
public class HamsterMovement : MonoBehaviour, ISoundMaker
{
	public event Action<Vector3> OnLoudSoundMade;
	public event Action<Vector3> OnQuiteSoundMade;

	[SerializeField] private float moveSpeed = 5f;
	[SerializeField] private float loudSpeedStarts = 4f;
	[SerializeField] private float quiteSpeedStarts = 2f;

	[SerializeField] private SpriteRenderer _noiseSpriteRenderer;
	[SerializeField] private Sprite _loudNoiseWave;
	[SerializeField] private Sprite _quiteNoiseWave;

	private bool makingQuiteNoise = false;
	private bool makingLoudNoise = false;
	private bool notMakingNoise = false;

	private bool inCarpetZone = false;

	private Rigidbody2D rb;

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
		HandleNoiseMaking();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if(collision.tag == "Carpet")
			inCarpetZone = true;

	}
    private void OnTriggerExit2D(Collider2D collision)
    {
		if (collision.tag == "Carpet")
			inCarpetZone = false;
	}

    private void HandleNoiseMaking()
    {
		if(movement.normalized == Vector2.zero)
        {
            if (!notMakingNoise)
            {
				_noiseSpriteRenderer.sprite = null;
				notMakingNoise = true;
				makingLoudNoise = false;
				makingQuiteNoise = false;
			}
			return;
        }

        if (inCarpetZone)
        {
            if (!makingQuiteNoise)
            {
				_noiseSpriteRenderer.sprite = _quiteNoiseWave;
				makingQuiteNoise = true;
				notMakingNoise = false;
				makingLoudNoise = false;
			}
			OnQuiteSoundMade?.Invoke(transform.position);
        }
        else
        {
			if (!makingLoudNoise)
			{
				_noiseSpriteRenderer.sprite = _loudNoiseWave;
				makingLoudNoise = true;
				makingQuiteNoise = false;
				notMakingNoise = false;
			}
			OnLoudSoundMade?.Invoke(transform.position);
		}
	}
}