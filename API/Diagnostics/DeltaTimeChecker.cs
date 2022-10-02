// -----------------------------------------------------------------------
// <copyright file="DeltaTimeChecker.cs" company="Mistaken">
// Copyright (c) Mistaken. All rights reserved.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using JetBrains.Annotations;
using UnityEngine;

namespace Mistaken.API.Diagnostics
{
    [PublicAPI]
    internal class DeltaTimeChecker : MonoBehaviour
    {
        private readonly List<double> deltaTimes = new();

        private void Start()
        {
            this.StartCoroutine(this.SaveLoop());
        }

        private IEnumerator SaveLoop()
        {
            while (this.enabled)
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
