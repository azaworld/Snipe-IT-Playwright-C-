using Microsoft.Playwright;

namespace PlaywrightTests.Pages;

public class LoginPage
{
    private readonly IPage _page;

    public LoginPage(IPage page)
    {
        _page = page;
    }

    public async Task LoginAsync(string username, string password)
    {
        await _page.GotoAsync("https://demo.snipeitapp.com/login");
        await _page.FillAsync("input[name='username']", username);
        await _page.FillAsync("input[name='password']", password);
        await _page.ClickAsync("button.btn-primary");
    }

    public async Task<bool> IsDashboardDisplayed()
    {
        var dashboardTitle = await _page.InnerTextAsync("h1.pull-left.pagetitle");
        return dashboardTitle == "Dashboard";
    }
}
