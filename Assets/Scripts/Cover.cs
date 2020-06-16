using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D), typeof(Rigidbody2D))]
public class Cover : MonoBehaviour
{
    private static HashSet<Cover> _activeCovers = new HashSet<Cover>();
    
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _activeCovers.Add(this);
            UpdatePlayerHidden();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            _activeCovers.Remove(this);
            UpdatePlayerHidden();
        }
    }

    private void UpdatePlayerHidden()
    {
        GameManager.Instance.IsPlayerHidden = _activeCovers.Count > 0;
    }
}
