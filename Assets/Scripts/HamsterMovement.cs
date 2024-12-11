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
    public event Action<Vector3> OnSuperLoudSoundMade;

	private bool _InSafeSpot;
	public bool InSafeSpot
    {
        get
        {
			return _InSafeSpot;
		}
        set
        {
			if (value == true)
				foreach (var renderer in _SpriteRenderers)
					renderer.color = _shadowColor;

			else
				foreach (var renderer in _SpriteRenderers)
					renderer.color = _normalColor;

			_InSafeSpot = value;
		}
	}


	[Header("Movement")]
	[SerializeField] private float moveSpeed = 5f;

	[Header ("Noise")]
	[SerializeField] private bool _usesNoise = false;
	[SerializeField] private SpriteRenderer _noiseSpriteRenderer;
	[SerializeField] private Sprite _loudNoiseWave;
	[SerializeField] private Sprite _quiteNoiseWave;

	[Space(5f)]
	[Header("Shadow")]
	[SerializeField] private List<SpriteRenderer> _SpriteRenderers;
	[SerializeField] private Color _shadowColor;
	private Color _normalColor;

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
		_normalColor = _SpriteRenderers[0].color;
	}

	void Update()
	{
		// Input
		movement.x = Input.GetAxisRaw("Horizontal");
		movement.y = Input.GetAxisRaw("Vertical");

		animator.SetFloat("x", movement.x);
        animator.SetFloat("y", movement.y);

		Debug.Log(InSafeSpot);

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
        if (_usesNoise)
			HandleNoiseMaking();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
		if (!_usesNoise)
			return;

		if(collision.tag == "Carpet")
			inCarpetZone = true;

	}
    private void OnTriggerExit2D(Collider2D collision)
    {
		if (!_usesNoise)
			return;

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