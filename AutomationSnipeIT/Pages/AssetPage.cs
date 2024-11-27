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

    public async Task FillAssetDetailsAsync()
    {
        // wait for the "Create Asset" text to be visible to ensure the page has loaded
        var createAssetText = _page.Locator("h1.pagetitle", new() { HasText = "Create Asset" });
        await createAssetText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        //select model
        var modelDropdown = _page.Locator("#select2-model_select_id-container");
        await modelDropdown.ClickAsync();

        var searchInput = _page.Locator("span.select2-search.select2-search--dropdown input.select2-search__field");
        await searchInput.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 8000 });

        await searchInput.FillAsync("Apple macbook 13");

        var searchResults = _page.Locator("ul.select2-results__options");
        await searchResults.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 6000 });

        var appleMacbookOption = _page.Locator("li.select2-results__option", new() { HasText = "CLaptops - Apple macbook 13" });
        await appleMacbookOption.ClickAsync();


        //Select Ready to deploy
        var statusDropdown = _page.Locator("#select2-status_select_id-container");
        await statusDropdown.ClickAsync();
        var dropdownOptions = _page.Locator("ul.select2-results__options");
        await dropdownOptions.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var readyToDeployOption = _page.Locator("li.select2-results__option", new() { HasText = "Ready to Deploy" });
        await readyToDeployOption.ClickAsync();


        // Assign the asset to a random user
        var userDropdown = _page.Locator("#select2-assigned_user_select-container");
        await userDropdown.ClickAsync();
        var userSearchInput = _page.Locator(".select2-search__field");
        await userSearchInput.FillAsync("Abc");
        var userOption = _page.Locator("li.select2-results__option", new() { HasText = "Abc, Xyz (Abc)" });
        await userOption.ClickAsync();
    }

    public async Task SaveAssetAsync()
    {
        var saveButton = _page.Locator("button[type='submit'].btn.btn-primary.pull-right:has(i.fas.fa-check.icon-white)").Nth(1);
        await saveButton.ClickAsync();
    }

    public async Task<bool> IsAssetCreatedAsync()
    {
        var successAlert = _page.Locator("div.alert.alert-success.fade.in");
        await successAlert.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });
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

    public async Task ClickViewLinkAsync()
    {
        var viewLink = _page.Locator("a:has-text('Click here to view')");
        await viewLink.ClickAsync();
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
