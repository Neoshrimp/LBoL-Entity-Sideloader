using LBoL.EntityLib.Cards.Character.Reimu;
using LBoL.EntityLib.Cards.Neutral.NoColor;
using LBoL.EntityLib.Exhibits.Common;
using LBoL.EntityLib.Exhibits.Shining;
using LBoLEntitySideloader.Entities;
using System.Collections.Generic;


namespace Random_Examples
{
    public static class CustomLoadouts
    {


        public static void AddLoadouts()
        {

            var cards = new List<string>() {
                        nameof(Shoot), nameof(Shoot), nameof(Boundary), nameof(Boundary),
                        nameof(ReimuAttackR),
                        nameof(ReimuAttackR),
                        nameof(ReimuAttackW),
                        nameof(ReimuBlockW),
                        nameof(ReimuBlockW),
                        nameof(ReimuBlockR),
                        // sunlight prayer
                        nameof(QiqingYishi)
                };

            PlayerUnitTemplate.AddExtraLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoU),
                cards,
                3
                );

            PlayerUnitTemplate.AddExtraLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoG),
                cards,
                1
                );


            PlayerUnitTemplate.AddExtraLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(Tuanzi),
                cards,
                2
                );

            PlayerUnitTemplate.AddExtraLoadout(
                "Reimu",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoG),
                cards,
                1
                );

            PlayerUnitTemplate.AddExtraLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );

            PlayerUnitTemplate.AddExtraLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );

            PlayerUnitTemplate.AddExtraLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );

            PlayerUnitTemplate.AddExtraLoadout(
            "Reimu",
            new ReimuUltRJabDef().UniqueId,
            nameof(CirnoG),
            cards,
            1
            );



            PlayerUnitTemplate.AddExtraLoadout(
                "Marisa",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoU),
                cards,
                3
                );

            PlayerUnitTemplate.AddExtraLoadout(
                "Sakuya",
                new ReimuUltRJabDef().UniqueId,
                nameof(CirnoU),
                cards,
                3
                );

        }



       
    }
  
}
