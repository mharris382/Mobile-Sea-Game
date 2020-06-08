using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using TMPro;

public class LootCounter : MonoBehaviour
{
    private TextMeshProUGUI _text;

    private void Awake()
    {
        _text = GetComponent<TextMeshProUGUI>();
    }

    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnLootPickup.AddListener(UpdateCount);
    }

    private void UpdateCount(int treasureCount)
    {
        _text.text = treasureCount.ToString();
    }
}
