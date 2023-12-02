namespace ScrumPlanningPoker.Entity.Game;

public enum Card
{
    Zero = 0,
    Half = 1,
    One = 2,
    Two = 3,
    Three = 4,
    Five = 5,
    Eight = 6,
    Thirteen = 7,
    Twenty = 8,
    Forty = 9,
    Hundred = 10,
    Question = 11,
    Coffee = 12
}

public static class Cards
{
    public static readonly Dictionary<Card, string> CardValues = new()
    {
        { Card.Zero, "0" },
        { Card.Half, "½" },
        { Card.One, "1" },
        { Card.Two, "2" },
        { Card.Three, "3" },
        { Card.Five, "5" },
        { Card.Eight, "8" },
        { Card.Thirteen, "13" },
        { Card.Twenty, "20" },
        { Card.Forty, "40" },
        { Card.Hundred, "100" },
        { Card.Question, "?" },
        { Card.Coffee, "☕" }
    };
    
    public static string GetCardValueString(int? cardValue)
    {
        return cardValue == null ? "/" : CardValues[(Card) cardValue];
    }
    
    public static Card? CalculateAverage(IEnumerable<int?> votes)
    {
        // Filtrer les votes nuls et convertir les valeurs restantes en type Card
        var validVotes = votes.Where(v => v.HasValue).Select(v => (Card)v.Value);

        if (!validVotes.Any())
        {
            // Retourner null si aucun vote valide n'est présent
            return null;
        }

        // Compter la fréquence de chaque vote
        var frequencyMap = validVotes.GroupBy(v => v)
            .ToDictionary(g => g.Key, g => g.Count());

        // Trouver la fréquence maximale
        int maxFrequency = frequencyMap.Values.Max();

        // Sélectionner toutes les cartes qui ont la fréquence maximale
        var candidates = frequencyMap.Where(f => f.Value == maxFrequency)
            .Select(f => f.Key);

        // Retourner la carte avec la plus grande valeur parmi les candidats
        return candidates.Max();
    }
}
