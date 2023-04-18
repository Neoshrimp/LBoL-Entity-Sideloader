using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Resources
{
    public class CardImages
    {
        public Texture2D main;

        public List<Texture2D> subs = new List<Texture2D>();

        public CardImages(Texture2D main)
        {
            this.main = main;
        }

        public CardImages(Texture2D main, List<Texture2D> subs) : this(main)
        {
            this.subs = subs;
        }



    }
}
