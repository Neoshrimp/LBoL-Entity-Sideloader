using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;
using Mono.Cecil;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using UnityEngine;

namespace LBoLEntitySideloader.Resources
{
    public class CardImages : IResourceProvider<Texture2D>
    {
        public Texture2D main;

        public Dictionary<string, Texture2D> subs = new Dictionary<string, Texture2D>();


        IResourceSource source;

        Func<string, Texture2D> loadingAction;

        public CardImages(IResourceSource source)
        {
            this.source = source;
            loadingAction = (string id) => ResourceLoader.LoadTexture(id, this.source);
        }

        public CardImages(IResourceSource source, Texture2D main) : this(source)
        {
            this.main = main;
        }

        public CardImages(IResourceSource source, Texture2D main, Dictionary<string, Texture2D> subs) : this(source, main)
        {
            this.subs = subs;
        }

        public void AutoLoad(CardTemplate cardTemplate, string extension)
        {
            AutoLoad(cardTemplate.GetId(), extension, (List<string>)CardConfig.FromId(cardTemplate.UniqueId).SubIllustrator);
            
        }

        public void AutoLoad(string mainId, string extension, List<string> subIds = null)
        {

            main = loadingAction(mainId + extension);
            
            if(subIds != null)
                foreach (var sub in subIds)
                {
                    subs.Add(mainId + sub, loadingAction(mainId + sub + extension));
                }
        }

        public Texture2D Load() => main;


        public Dictionary<string, Texture2D> LoadMany() => subs;
    }
}
