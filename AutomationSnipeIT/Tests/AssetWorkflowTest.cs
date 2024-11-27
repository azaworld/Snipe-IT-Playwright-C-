using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.Pages;
using System.Text.RegularExpressions;

namespace PlaywrightTests.Tests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class AssetWorkflowTest : PageTest
{
    [Test]
    public async Task AutomateSnipeITAssetWorkflow()
    {
        var loginPage = new LoginPage(Page);
        var assetPage = new AssetPage(Page);

        //Login
        await loginPage.LoginAsync("admin", "password");
        bool isDashboardDisplayed = await loginPage.IsDashboardDisplayed();
        Assert.That(isDashboardDisplayed, Is.True, "Dashboard is not displayed after login.");

        //Navigate to Asset Creation
        await assetPage.NavigateToCreateAssetAsync();

        //Fill Asset Details and Save
        await assetPage.FillAssetDetailsAsync();
        await assetPage.SaveAssetAsync();

        var successAlertLocator = Page.Locator("div.alert-success");
        await Expect(successAlertLocator).ToHaveTextAsync(new System.Text.RegularExpressions.Regex("Success:"));
        await assetPage.ClickViewLinkAsync();

        // Locate the status and user elements
        var statusLocator = Page.Locator("div.col-md-9 >> text='Ready to Deploy'");
        var userLocator = Page.Locator("div.col-md-9 >> a[href*='/users/64']");
        var modelLocator = Page.Locator("div.col-md-9 >> a[href*='/models/26']");
        var historyLocator = Page.Locator("a[href='#history'] >> text='History'");

        await Expect(statusLocator).ToHaveTextAsync(new Regex("Ready to Deploy"));
        await Expect(userLocator).ToHaveTextAsync(new Regex("Abc Xyz"));
        await Expect(modelLocator).ToHaveTextAsync(new Regex("Apple macbook 13"));
        await Expect(historyLocator).ToHaveTextAsync(new Regex("History"));
        await historyLocator.ClickAsync();


        var userInHistoryLocator = Page.Locator("td >> a[href*='/users/64'] >> text='Abc Xyz'");
        await Expect(userInHistoryLocator).ToHaveTextAsync(new Regex("Abc Xyz"));

        var checkoutTextLocator = Page.Locator("td >> text='Checked out on asset creation'");
        await Expect(checkoutTextLocator).ToHaveTextAsync(new Regex("Checked out on asset creation"));
    }
}
