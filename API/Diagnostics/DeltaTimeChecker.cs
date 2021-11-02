// -----------------------------------------------------------------------
// <copyright file="DeltaTimeChecker.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using UnityEngine;

namespace Mistaken.API.Diagnostics
{
    internal class DeltaTimeChecker : MonoBehaviour
    {
        private List<double> deltaTimes = new List<double>();

        private void Start()
        {
            this.StartCoroutine(this.SaveLoop());
        }

        private IEnumerator SaveLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(1);

                var array = this.deltaTimes.ToArray();
                this.deltaTimes.Clear();
                MasterHandler.LogTime("Diagnostics.Update", array.Average());
            }
        }

        private void Update()
        {
            this.deltaTimes.Add(Time.deltaTime);
        }

        private void OnDestroy()
        {
            Log.Warn("DeltaTimeChecker destroyed");
        }
    }
}
