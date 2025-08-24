using BepInEx.Logging;
using HarmonyLib;
using LBoL.Base;
using LBoL.Base.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using static Random_Examples.BepinexPlugin;

namespace Random_Examples
{

    public struct ColorContainer
    {
        public int id;
        public List<ManaColor> colors;

        public ColorContainer(int id, string colors)
        {
            if (!ManaGroup.TryParse(colors, out var mana))
                throw new Exception("fu");

            this.id = id;
            this.colors = mana.EnumerateColors().ToList();
        }

        public static implicit operator ColorContainer((int, string) tu)
        {
            return new ColorContainer(tu.Item1, tu.Item2);
        }

        public override string ToString()
        {
            return $"{id}:{string.Join("", colors.Select(c => c.ToShortName()))}";
        }



    }
    public class BipartiteColours
    {

        public static List<ColorContainer> CreateInput(List<string> colors)
        {
            return colors.Select((s, i) => new ColorContainer(i, s)).ToList();
        }

        public static void PrintGroups(List<ColorContainer>[] groups)
        {
            var cArray = Enum.GetValues(typeof(ManaColor)).Cast<ManaColor>().ToArray();

            foreach (var (i, g) in groups.WithIndices())
            {
                /*if (i == 0 || i > 7)
                    continue;*/

                var toLog = new StringBuilder();
                toLog.Append($"{cArray[i].ToShortName()}".PadRight(3));

                foreach (var cc in g)
                {
                    toLog.Append($"{cc};".PadRight(6));
                }

                log.LogInfo(toLog);
            }
            log.LogInfo("---------");

        }

        // first element of each colour group will be random element chosen for that colour
        public static List<ColorContainer>[] MakeGroupings(IEnumerable<ColorContainer> toGroup, RandomGen rng)
        {
            var subRng = new RandomGen(rng.NextULong());

            // tries to fill a valid empty group if possible, else choses a valid group at random
            int ChooseSlot(ColorContainer cc, List<ColorContainer>[] groups)
            {
                var possibleSlotIndexes = cc.colors.Select(c => (int)c).ToList();

                if (possibleSlotIndexes.Count == 0)
                    return -1;

                //log.LogInfo($"grouping element {cc}");

                var slotIndex = -1;
                var emptyGroupIndexes = possibleSlotIndexes.Where(i => groups[i].Count == 0).ToList();

                if (possibleSlotIndexes.Count == 1)
                {
                    slotIndex = possibleSlotIndexes[0];
                }
                else if (emptyGroupIndexes.Count > 0)
                {
                    //log.LogInfo($"emptyslotFound");
                    slotIndex = emptyGroupIndexes.Sample(subRng);
                }
                else
                {
                    //log.LogInfo($"using any slot");
                    slotIndex = possibleSlotIndexes.Sample(subRng);
                }
                return slotIndex;
            }
                
            var allGroups = toGroup.GroupBy(cl => cl.colors.Count).OrderBy(gp => gp.Key);

            var biasedGroups = Enumerable.Range(1, Enum.GetValues(typeof(ManaColor)).Length).Select(_ => new List<ColorContainer>()).ToArray();

            // finds bipartite solution and creates fallback sampling group
            // bipartite algo is not like the one in the article. it tries to match elements with the most stringent requirements,
            // i.e., single colour cards. idk it seems to work in my head
            foreach (var group in allGroups) 
            {
                //log.LogInfo(group.Key);
                var shuffledGroup = group.ToList();
                if (group.Key > 1)
                {
                    //log.LogInfo($"shuffling group {group.Key}");
                    //log.LogInfo($"before shuffle: {string.Join(";", group)}");
                    shuffledGroup.Shuffle(subRng);
                    //log.LogInfo($"after shuffle: {string.Join(";", group)}");
                }

                foreach (var cc in shuffledGroup) 
                {
                    var minSlotIndex = ChooseSlot(cc, biasedGroups);
/*                    if (possibleSlotIndexes.Count > 1 
                        && possibleSlotIndexes.All(i => biasedGroups[possibleSlotIndexes[0]].Count == biasedGroups[i].Count))
                    {
                        log.LogInfo($"all groups contain the same amount of elements. Grouping the element randomly");
                        minSlotIndex = possibleSlotIndexes.Sample(subRng); ;
                    }
                    else // add to group with least elements
                    { 
                        
                        var minSlot = int.MaxValue;
                        foreach (var si in possibleSlotIndexes)
                        {
                            if (biasedGroups[si].Count < minSlot)
                            { 
                                minSlotIndex = si;
                                minSlot = biasedGroups[si].Count;
                            }
                        }
                    }*/

                    if (minSlotIndex == -1)
                        throw new InvalidProgramException("valid group slot not found");

                    biasedGroups[minSlotIndex].Add(cc);
                }

                //log.LogInfo($"//////");
            }

            var bipartiteSolution = biasedGroups.Count(g => g.NotEmpty());
            log.LogInfo($"bipartiteSolution: {bipartiteSolution}");


            var bipartiteAttempt = 0;
            var attempts = 0;

            var allElements = toGroup.ToList();

            List<ColorContainer>[] unbiasedGroups = null;

            // tries to find groups with less biased sampling but might need several attempts to populate max possible groups
            // kinda of a monkey solution but it rarely seems to take more than 1 attempt?
            while (bipartiteAttempt < bipartiteSolution && attempts < 100)
            {
                bipartiteAttempt = 0;
                allElements.Shuffle(subRng);
                unbiasedGroups = Enumerable.Range(1, Enum.GetValues(typeof(ManaColor)).Length)
                    .Select(_ => new List<ColorContainer>()).ToArray();


                log.LogInfo(string.Join(";", allElements));

                foreach (var e in allElements)
                {
                    var slotIndex = ChooseSlot(e, unbiasedGroups);
                    if (slotIndex == -1)
                        throw new Exception("fu2");

                    if (unbiasedGroups[slotIndex].Empty())
                        bipartiteAttempt++;

                    unbiasedGroups[slotIndex].Add(e);
                }

                attempts++;
            }

            log.LogInfo($"attempts: {attempts}");
            log.LogInfo($"biA: {bipartiteAttempt}");

            if (bipartiteAttempt == bipartiteSolution)
            {
                log.LogInfo($"found unbiased grouping!!!");
                return unbiasedGroups;
            }

            biasedGroups.Do(g => g.Shuffle(subRng));
            return biasedGroups;
        }


    }
}
