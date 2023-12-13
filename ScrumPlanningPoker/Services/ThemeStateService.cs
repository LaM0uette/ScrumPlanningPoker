namespace ScrumPlanningPoker.Services;

public class ThemeStateService(CookieService cookieService)
{
    #region Statements

    public string ClassCss { get; private set; } = "";
    
    public event Action? OnChange;
    
    private bool _darkMode;
    public bool DarkMode
    {
        get => _darkMode;
        private set
        {
            if (_darkMode == value)
            {
                return;
            }
            
            _darkMode = value;
            ClassCss = _darkMode ? "dark-theme" : "";
                
            NotifyStateChanged();
        }
    }

    #endregion

    #region Functions

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
            await cookieService.UpdateCookie(CookieService.CookieDarkMode, ClassCss);
        }
        
        await cookieService.SetCookie(CookieService.CookieDarkMode, ClassCss);
    }

    private void NotifyStateChanged()
    {
        OnChange?.Invoke();
    }

    #endregion
}