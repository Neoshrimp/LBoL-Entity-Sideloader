using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace LBoLEntitySideloader.Resource
{
    public class Audio : IResourceProvider<AudioClip>
    {
        public AudioClip Load()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, AudioClip> LoadMany()
        {
            throw new NotImplementedException();
        }
    }
}
