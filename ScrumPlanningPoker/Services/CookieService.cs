﻿using Microsoft.JSInterop;

namespace ScrumPlanningPoker.Services;

public class CookieService(IJSRuntime jsRuntime)
{
    #region Statements

    public static string CookieUserGuid { get; set; } = "CookieUserGuid";
    public static string CookieUserName { get; set; } = "CookieUserName";

    #endregion
    
    #region Functions

    public async Task<string?> GetCookie(string cookieName)
    {
        var cookieValue = await jsRuntime.InvokeAsync<string>("getCookie", cookieName);
        return cookieValue;
    }

    public async Task SetCookie(string cookieName, string value, int days = 36500)
    {
        await jsRuntime.InvokeVoidAsync("setCookie", cookieName, value, days);
    }

    public async Task UpdateCookie(string cookieName, string newValue, int days = 36500)
    {
        await jsRuntime.InvokeVoidAsync("setCookie", cookieName, newValue, days);
    }

    #endregion
}