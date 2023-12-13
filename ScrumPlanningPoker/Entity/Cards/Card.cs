namespace ScrumPlanningPoker.Entity.Cards;

public static class Card
{
    #region Statements

    public static readonly Dictionary<CardType, string> Cards;

    static Card()
    {
        Cards = new()
        {
            { CardType.Zero, "0" },
            { CardType.Half, "½" },
            { CardType.One, "1" },
            { CardType.Two, "2" },
            { CardType.Three, "3" },
            { CardType.Five, "5" },
            { CardType.Eight, "8" },
            { CardType.Thirteen, "13" },
            { CardType.Twenty, "20" },
            { CardType.Forty, "40" },
            { CardType.Hundred, "100" },
            { CardType.Question, "?" },
            { CardType.Coffee, "☕" }
        };
    }

    #endregion

    #region Functions

    public static string GetCardValueString(int? cardValue)
    {
        return cardValue == null ? "/" : Cards[(CardType) cardValue];
    }
    
    public static string CalculateAverage(IEnumerable<int?> votes)
    {
        var validVotes = votes.Where(v => v.HasValue).Select(v => (CardType)v!.Value);

        var cards = validVotes as CardType[] ?? validVotes.ToArray();
        if (cards.Length == 0)
        {
            return "/";
        }

        // Compter la fréquence de chaque vote
        var frequencyMap = cards.GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());

        // Trouver la fréquence maximale
        var maxFrequency = frequencyMap.Values.Max();

        // Sélectionner toutes les cartes qui ont la fréquence maximale
        var candidates = frequencyMap.Where(f => f.Value == maxFrequency)
            .Select(f => f.Key);

        return GetCardValueString((int)candidates.Max());
    }

    #endregion
}
