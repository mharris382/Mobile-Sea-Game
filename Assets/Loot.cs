using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(Rigidbody2D))]
public class Loot : MonoBehaviour
{
    [Tooltip("How much the loot is worth"), Min(0)]
    public int value = 1;

    public bool doParticleEffect = true;
    public GameObject particleEffectPrefab;

    private SpriteRenderer _spriteRenderer;
    private Collider2D _collider;


    private void Awake()
    {
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _collider = GetComponent<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(!other.CompareTag("Player")) 
            return;
        GameManager.Instance.PickupLoot(value);
        _spriteRenderer.enabled = false;
        _collider.enabled = false;
        if (doParticleEffect)
        {
            if(particleEffectPrefab == null) 
                return;
            var particles = GameObject.Instantiate(particleEffectPrefab, this.transform).GetComponent<ParticleSystem>();
            particles.Emit(value);
        }
    }
}
