﻿using HarmonyLib;
using LBoLEntitySideloader.ReflectionHelpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace LBoLEntitySideloader
{
    public class Sequence
    {
        private int counter = 0;

        public Sequence(int startingPoint = 0)
        {
            this.counter = startingPoint;
        }

        public void SetCounter(int counter)
        {
            this.counter = counter;
        }

        public int Counter { get => counter; }

        public int Next()
        {
            return counter++;
        }

    }
}
