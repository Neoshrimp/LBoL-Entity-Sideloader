using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class AudioInfo : IResourceProvider<AudioClip>
    {

        public AudioClip main;


        public AudioClip Load()
        {
            return main;
        }

        public Dictionary<string, AudioClip> LoadMany()
        {
            throw new NotImplementedException();
        }
    }
}
