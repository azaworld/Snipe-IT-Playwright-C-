using Microsoft.Playwright;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Microsoft.Playwright.NUnit;

namespace PlaywrightTests.Pages;

public class AssetPage
{
    private readonly IPage _page;

    public AssetPage(IPage page)
    {
        _page = page;
    }

    public async Task NavigateToCreateAssetAsync()
    {
        var createNewLink = _page.Locator("a.dropdown-toggle", new() { HasText = "Create New" });
        await createNewLink.ClickAsync();

        var createNewDropdown = _page.Locator("li.dropdown.open > ul.dropdown-menu");
        await createNewDropdown.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        var assetLink = createNewDropdown.Locator("a", new() { HasText = "Asset" });
        await assetLink.ClickAsync();

        await _page.WaitForURLAsync("https://demo.snipeitapp.com/hardware/create");
    }

    public async Task FillAssetDetailsAsync(string modelName, string userName)
    {
        // Select model
        var modelDropdown = _page.Locator("#select2-model_select_id-container");
        await modelDropdown.ClickAsync();
        var searchInput = _page.Locator("span.select2-search.select2-search--dropdown input.select2-search__field");
        await searchInput.FillAsync(modelName);
        await _page.Locator("li.select2-results__option", new() { HasText = modelName }).ClickAsync();

        // Select status
        var statusDropdown = _page.Locator("#select2-status_select_id-container");
        await statusDropdown.ClickAsync();
        await _page.Locator("li.select2-results__option", new() { HasText = "Ready to Deploy" }).ClickAsync();

        // Assign user
        var userDropdown = _page.Locator("#select2-assigned_user_select-container");
        await userDropdown.ClickAsync();
        await _page.Locator(".select2-search__field").FillAsync(userName);
        await _page.Locator("li.select2-results__option", new() { HasText = userName }).ClickAsync();
    }

    public async Task SaveAssetAsync()
    {
        var saveButton = _page.Locator("button[type='submit'].btn.btn-primary.pull-right:has(i.fas.fa-check.icon-white)").Nth(1);
        await saveButton.ClickAsync();
    }

    public async Task<bool> IsAssetCreatedAsync()
    {
        var successAlert = _page.Locator("div.alert.alert-success.fade.in");
        await successAlert.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        return await successAlert.IsVisibleAsync();
    }

    // New method to extract asset tag and URL from success alert
    public async Task<(string assetTag, ILocator viewLink)> ExtractAssetTagAndUrlFromSuccessAlertAsync()
    {
        var successAlert = _page.Locator("div.alert.alert-success.fade.in");
        var successMessage = await successAlert.InnerTextAsync();
        var assetTagMatch = Regex.Match(successMessage, @"Asset with tag (\d+) was created successfully");
        var assetTag = assetTagMatch.Groups[1].Value;

        var viewLink = _page.Locator("a:has-text('Click here to view')");

        return (assetTag, viewLink); // Returning assetTag and the locator for the view link
    }

    // New method to check status, user, model, and history
    public async Task VerifyAssetDetailsAsync(string expectedStatus, string expectedUser, string expectedModel)
    {
        // Locate elements
        var statusLocator = _page.Locator("div.col-md-9 >> text='" + expectedStatus + "'");
        var userLocator = _page.Locator("div.col-md-9 >> a[href*='/users/64']");
        var modelLocator = _page.Locator("div.col-md-9 >> a[href*='/models/26']");
        var historyLocator = _page.Locator("a[href='#history'] >> text='History'");

        // Assert the status text matches the expected value
        var statusText = await statusLocator.TextContentAsync();
        Assert.That(statusText, Is.EqualTo(expectedStatus), $"Expected status '{expectedStatus}' but found '{statusText}'.");

        // Assert the user text matches the expected value
        var userText = await userLocator.TextContentAsync();
        Assert.That(userText, Is.EqualTo(expectedUser), $"Expected user '{expectedUser}' but found '{userText}'.");

        // Assert the model text matches the expected value
        var modelText = await modelLocator.TextContentAsync();
        Assert.That(modelText, Is.EqualTo(expectedModel), $"Expected model '{expectedModel}' but found '{modelText}'.");

        // Assert the history text is present
        var historyText = await historyLocator.TextContentAsync();
        Assert.That(historyText, Is.EqualTo("History"), $"Expected 'History' but found '{historyText}'.");

        // Click on the history link
        await historyLocator.ClickAsync();
    }


    // New method to assert user text and checkout text in history
    public async Task VerifyUserAndCheckoutTextAsync(string expectedUser, string expectedCheckoutText)
    {
        // Assert the user text in the <td> element
        var userInHistoryLocator = _page.Locator($"td >> a[href*='/users/64'] >> text='{expectedUser}'");
        var userText = await userInHistoryLocator.TextContentAsync();
        Assert.That(userText, Is.EqualTo(expectedUser), $"Expected '{expectedUser}' but found '{userText}'.");

        // Assert the "Checked out on asset creation" text in the <td> element
        var checkoutTextLocator = _page.Locator($"td >> text='{expectedCheckoutText}'");
        var checkoutText = await checkoutTextLocator.TextContentAsync();
        Assert.That(checkoutText, Is.EqualTo(expectedCheckoutText), $"Expected '{expectedCheckoutText}' but found '{checkoutText}'.");
    }

}
