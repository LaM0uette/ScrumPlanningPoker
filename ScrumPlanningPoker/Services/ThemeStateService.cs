namespace ScrumPlanningPoker.Services;

public class ThemeStateService(CookieService cookieService)
{
    public event Action? OnChange;

    public string CssDarkMode { get; private set; } = "";
    
    private bool _darkMode;
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
    
    public async Task InitializeDarkMode()
    {
        var cookieDarkMode = await cookieService.GetCookie(CookieService.CookieDarkMode);
        if (cookieDarkMode != null)
        {
            DarkMode = cookieDarkMode == "dark-theme";
        }
    }
    
    public async Task ToggleDarkMode()
    {
        DarkMode = !DarkMode;
        
        var cookieDarkMode = await cookieService.GetCookie(CookieService.CookieDarkMode);
        if (cookieDarkMode != null)
        {
            await cookieService.UpdateCookie(CookieService.CookieDarkMode, CssDarkMode);
        }
        
        await cookieService.SetCookie(CookieService.CookieDarkMode, CssDarkMode);
    }

    private void NotifyStateChanged() => OnChange?.Invoke();
}