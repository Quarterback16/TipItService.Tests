using InjectorMicroService;
using LaYumba.Functional;
using System.Diagnostics.CodeAnalysis;
using TipItService.Helpers;

namespace TipItService.Tests
{
    [TestClass]
    [ExcludeFromCodeCoverage]
    public class ServiceTests
    {
        private const int K_Season = 2025;

        private TipItService? _cut;

        [TestInitialize]
        public void Setup()
        {
            _cut = new TipItService("d:\\DropBox\\");
        }

        [TestMethod]
        public void TipItService_InstantiatesOk() 
        {
            Assert.IsNotNull(_cut);
        }

        [TestMethod]
        public void TipItService_CanGetResults()
        {
            var r = _cut?.GetResultJson();
            Console.WriteLine($"{r} Results found");
            Assert.IsTrue(r > 0);
        }

        [TestMethod]
        public void TipItService_CanLoadTippingState()
        {
            Console.WriteLine($"Using {_cut.TippingStateFileName}");
            var r = _cut?.LoadTippingState();
            Console.WriteLine($"{r} matches found");
            Assert.IsTrue(r > 0);
            var nrlMatches = _cut?.CurrentState.Matches
                .Where(m => m.League.Code == "NRL" && m.MatchDateTime.Year == K_Season)
                .ToList();
            var nNrlPlayed = nrlMatches
                .Count(m => m.Played());
            Console.WriteLine(
                $"For 2024 there are {nrlMatches?.Count} NRL matches {nNrlPlayed} have been played");
            var aflMatches = _cut?.CurrentState.Matches
                .Where(m => m.League.Code == "AFL" && m.MatchDateTime.Year == K_Season)
                .ToList();
            var nAflPlayed = aflMatches
                .Count(m => m.Played());
            Console.WriteLine(
                $"For 2024 there are {aflMatches?.Count} AFL matches {nAflPlayed} have been played");
            var aflMatches15 = _cut?.CurrentState.Matches
                .Where(m => m.League.Code == "AFL" && m.MatchDateTime.Year == K_Season)
                .Where(m => m.Round == 15)
                .ToList();
            Console.WriteLine(
                $"For 2024 there are {aflMatches15?.Count} AFL matches in Round 15");
            var aflMatches16 = _cut?.CurrentState.Matches
                .Where(m => m.League.Code == "AFL" && m.MatchDateTime.Year == K_Season)
                .Where(m => m.Round == 16)
                .ToList();
            Console.WriteLine(
                $"For 2024 there are {aflMatches16?.Count} AFL matches in Round 16");
            aflMatches16?.ForEach(m=>Console.WriteLine(m));
        }

        [TestMethod]
        public void TipItService_CanShowResultsOfRound()
        {
            var r = _cut?.LoadTippingState();
            Assert.IsTrue(r > 0);
            var theRound = _cut.GetRound(1, "AFL", K_Season);
            if (theRound.Count > 0)
                theRound.ForEach(m => Console.WriteLine(m.ToString()));
            else
                Console.WriteLine("No results found");
        }

        //  1. Update Results   /////////////////////////////////////////////////////////////////
        [TestMethod]
        public void TipItService_CanUpdateTippingState()
        {
            var r = _cut?.UpdateTippingState(
                DateTime.Now.AddDays(1));
            if (_cut?.NewResults.Count > 0)
            {
                _cut.NewResults.ForEach(nr => Console.WriteLine($"{nr}"));
            }
            else
                Console.WriteLine("No new results");
            Assert.IsTrue(r > 0);
        }

        [TestMethod]
        public void TipItService_CanDoTips()
        {
            var tips = _cut?.Tips();
            Assert.IsFalse(string.IsNullOrEmpty(tips.Value.Item1));
            Console.WriteLine(tips.Value);
        }

        [TestMethod]
        public void TipItService_CanDoTipsAsTipSet()
        {
            var tipset = _cut?.Tipset();
            Assert.IsNotNull(tipset);
        }

        [TestMethod]
        public void TipItService_MarkdownFromTipSet()
        {
            var tipset = _cut?.Tipset();
            var md = DashboardUtils.Tips(tipset,"NRL",0);
            Assert.IsFalse(string.IsNullOrEmpty(md));
            Console.WriteLine(md);
        }

        [TestMethod]
        public void TipItService_CanInjectNrlTips()
        {
            var mi = new MarkdownInjector(
                "d://dropbox//obsidian//ChestOfNotes//");
            var md = _cut?.Inject("NRL","nrl-tips",mi);

            Assert.IsFalse(string.IsNullOrEmpty(md));
            Console.WriteLine(md);
        }

        [TestMethod]
        public void TipItService_CanInjectAflTips()
        {
            var mi = new MarkdownInjector(
                "d://dropbox//obsidian//ChestOfNotes//");
            var md = _cut?.Inject("AFL", "afl-tips", mi);

            Assert.IsFalse(string.IsNullOrEmpty(md));
            Console.WriteLine(md);
        }

        //  2. Inject the Tips   ////////////////////////////////////////////////////////
        [TestMethod]
        public void TipItService_CanInjectAllTips()
        {
            var mi = new MarkdownInjector(
                "d://dropbox//obsidian//ChestOfNotes//");
            var md = _cut?.Inject("NRL", "nrl-tips", mi);
            Assert.IsFalse(string.IsNullOrEmpty(md));
            md = _cut?.Inject("AFL", "afl-tips", mi);
            Assert.IsFalse(string.IsNullOrEmpty(md));
        }

        [TestMethod]
        public void TipItService_CanGenerateEasyTipsState()
        {
            var md = _cut?.Easiest();
            Assert.IsFalse(string.IsNullOrEmpty(md));
            Console.WriteLine(md);
        }

        //  3.  Inject Easy Tips
        [TestMethod]
        public void TipItService_CanInjectEasyTipsState()
        {
            var mi = new MarkdownInjector(
                "d://dropbox//obsidian//ChestOfNotes//");
            var md = _cut?.Easiest();
            Assert.IsFalse(string.IsNullOrEmpty(md));
            Console.WriteLine(md);
            mi.InjectMarkdown(
                DashboardUtils.DashboardFile(K_Season),
                "easiest",
                md);
        }

        [TestMethod]
        public void TipItService_CanGenerate_RoundResults()
        {
            var roundResults = _cut?.RoundResults(K_Season);
            Assert.IsNotNull(roundResults);
            roundResults.ForEach(x => Console.WriteLine(x));
        }

        [TestMethod]
        public void TipItService_CanInjectNrlRankings()
        {
            var mi = new MarkdownInjector(
                "d://dropbox//obsidian//ChestOfNotes//");
            var md = _cut?.InjectRankings("NRL", "nrl-ranks", mi);

            Assert.IsFalse(string.IsNullOrEmpty(md));
            Console.WriteLine(md);
        }

        [TestMethod]
        public void TipItService_CanInjectAflRankings()
        {
            var mi = new MarkdownInjector(
                "d://dropbox//obsidian//ChestOfNotes//");
            var md = _cut?.InjectRankings("AFL", "afl-ranks", mi);

            Assert.IsFalse(string.IsNullOrEmpty(md));
            Console.WriteLine(md);
        }

        [TestMethod]
        public void TipItService_KnowsNextRound()
        {
            var testLeague = "AFL";
            var result = _cut?.TippingContext.NextRound(testLeague);
            Console.WriteLine($"Next Round for {testLeague} is {result}");
            Assert.AreEqual(1,result);
            testLeague = "NRL";
            result = _cut?.TippingContext.NextRound(testLeague);
            Console.WriteLine($"Next Round for {testLeague} is {result}");
            Assert.AreEqual(1, result);
        }

        [TestMethod]
        public void TipItService_KnowsMissingResultsNrl()
        {
            var result = _cut?.TippingContext.MissingResults(
                "NRL",
            new DateTime(2025, 4, 25, 0, 0, 0, DateTimeKind.Unspecified));
            Assert.IsNotNull(result);
            Console.WriteLine($"There are {result.Count} missing results");
            if (result.Count > 0)
                result.ForEach(m => Console.WriteLine(m));
        }

        [TestMethod]
        public void TipItService_KnowsMissingResultsAfl()
        {
            var result = _cut?.TippingContext.MissingResults(
                "AFL",
                new DateTime(2025,4,25,0,0,0,DateTimeKind.Unspecified));
            Assert.IsNotNull(result);
            Console.WriteLine($"There are {result.Count} missing results");
            if (result.Count > 0)
                result.ForEach(m => Console.WriteLine(m));
        }

    }
}
