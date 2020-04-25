﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SlotInfo : MonoBehaviour
{
    public string content;
    public int charges;
    public Text chargesText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeCharges(int moreCharges)
    {
        charges += moreCharges;
        chargesText.text = charges.ToString();
    }
}
