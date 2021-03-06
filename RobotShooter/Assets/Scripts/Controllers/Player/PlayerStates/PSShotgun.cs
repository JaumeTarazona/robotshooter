﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSShotgun : PlayerState
{
    public PSShotgun(PlayerController pc)
    {
        pc.shooting = true;
    }

    public override void CheckTransition(PlayerController pc)
    {
        if (pc.shotgunShotted)
        {
            pc.shooting = false;
            pc.ChangeState(new PSMovement(pc));
        } 
    }

    public override void FixedUpdate(PlayerController pc)
    {
        
    }

    public override void Update(PlayerController pc)
    {
        pc.Move();
        pc.Aim();
        pc.Shoot();
        pc.CheckHabilities();
    }
}
