﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerObjectMovement : MonoBehaviour
{

    public float speed = 5.0f;

    // Update is called once per frame
    void Update()
    {
        transform.position += Vector3.down * speed * Time.deltaTime;
    }
}