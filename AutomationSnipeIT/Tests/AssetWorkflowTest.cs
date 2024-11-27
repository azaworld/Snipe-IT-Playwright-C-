using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using PlaywrightTests.Pages;

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

        // Step 1: Login
        await loginPage.LoginAsync("admin", "password");
        bool isDashboardDisplayed = await loginPage.IsDashboardDisplayed();
        Assert.That(isDashboardDisplayed, Is.True, "Dashboard is not displayed after login.");

        // Step 2: Navigate to Asset Creation
        await assetPage.NavigateToCreateAssetAsync();

        // Step 3: Fill Asset Details and Save
        await assetPage.FillAssetDetailsAsync("Apple macbook 13", "Abc Xyz");
        await assetPage.SaveAssetAsync();

        // Step 4: Verify if the asset was created successfully
        bool isAssetCreated = await assetPage.IsAssetCreatedAsync();
        Assert.That(isAssetCreated, Is.True, "Asset was not created successfully.");

        // Step 5: Extract Asset Tag and View Link
        var (assetTag, viewLink) = await assetPage.ExtractAssetTagAndUrlFromSuccessAlertAsync();
        Assert.That(assetTag, Is.Not.Empty, "Asset tag is empty.");
        await viewLink.ClickAsync();

        // Step 6: Verify Asset Details (Status, User, Model)
        await assetPage.VerifyAssetDetailsAsync("Ready to Deploy", "Abc Xyz", "Apple macbook 13");

        // Step 7: Verify History Details (User and Checkout Text)
        await assetPage.VerifyUserAndCheckoutTextAsync("Abc Xyz", "Checked out on asset creation");
    }
}
