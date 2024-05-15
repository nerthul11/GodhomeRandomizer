using FStats;
using FStats.StatControllers;
using FStats.Util;
using GodhomeRandomizer.Manager;
using GodhomeRandomizer.Modules;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GodhomeRandomizer.Interop
{
    public static class FStats_Interop
    {
        public static void Hook()
        {
            API.OnGenerateFile += GenerateStats;
        }

        private static void GenerateStats(Action<StatController> generateStats)
        {
            if (!GodhomeManager.GlobalSettings.Enabled)
                return;
            
            generateStats(new GodhomeStats());
        }
    }

    public class GodhomeStats : StatController
    {
        public override void Initialize() 
        {
            PantheonModule.Instance.OnAchievedPantheons += AddMarks;
            StatueModule.Instance.OnAchievedHallOfGods += AddMarks;
        }

        private void AddMarks(List<string> marks)
        {
            foreach (string mark in marks)
                if (!OneTimeCheck.Contains(mark))
                {
                    OneTimeCheck.Add(mark);
                    GodhomeMarks.Add(new GodhomeMark(mark, FStatsMod.LS.Get<Common>().CountedTime));
                }
        }

        public record GodhomeMark(string Mark, float Timestamp);
        public List<GodhomeMark> GodhomeMarks = [];
        public List<string> OneTimeCheck = [];

        public override IEnumerable<DisplayInfo> GetDisplayInfos()
        {
            List<string> rows = GodhomeMarks.OrderBy(x => x.Timestamp).Select(x => $"{x.Mark}: {x.Timestamp.PlaytimeHHMMSS()}").ToList();
            if (GodhomeMarks.Count == 0) 
                yield break;
            
            yield return new()
            {
                Title = "Godhome Randomizer Timeline",
                MainStat = $"{GodhomeMarks.Count}/15",
                StatColumns = Columnize(rows),
                Priority = BuiltinScreenPriorityValues.ExtensionStats
            };
        }
        private const int COL_SIZE = 10;

        private List<string> Columnize(List<string> rows)
        {
            int columnCount = (rows.Count + COL_SIZE - 1) / COL_SIZE;
            List<string> list = [];
            for (int i = 0; i < columnCount; i++)
            {
                list.Add(string.Join("\n", rows.Slice(i, columnCount)));
            }
            return list;
        }
    }
}