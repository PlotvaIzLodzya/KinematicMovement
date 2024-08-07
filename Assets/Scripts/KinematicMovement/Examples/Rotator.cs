﻿using UnityEngine;

namespace PlotvaIzLodzya.KinematicMovement.Examples
{
    public class Rotator : MonoBehaviour
    {
        private void FixedUpdate()
        {
            transform.rotation *= Quaternion.Euler(Vector3.up * 10 * Time.fixedDeltaTime);
        }
    }
}