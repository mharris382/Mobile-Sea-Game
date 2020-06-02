using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class Cover : MonoBehaviour
{
    private static HashSet<Cover> _activeCovers = new HashSet<Cover>();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Entered Cover");
            _activeCovers.Add(this);
            UpdatePlayerHidden();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Left Cover");
            _activeCovers.Remove(this);
            UpdatePlayerHidden();
        }
    }

    private void UpdatePlayerHidden()
    {
        GameManager.Instance.IsPlayerHidden = _activeCovers.Count > 0;
    }
}
