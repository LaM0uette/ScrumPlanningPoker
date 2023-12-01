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
}
