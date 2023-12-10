namespace ScrumPlanningPoker.Services;

public class ThemeStateService
{
    public event Action OnChange;

    private bool _darkMode = false;
    public bool DarkMode
    {
        get => _darkMode;
        set
        {
            if (_darkMode != value)
            {
                _darkMode = value;
                CssDarkMode = _darkMode ? "dark-theme" : "";
                NotifyStateChanged();
            }
        }
    }

    public string CssDarkMode { get; private set; } = "";

    private void NotifyStateChanged() => OnChange?.Invoke();
}