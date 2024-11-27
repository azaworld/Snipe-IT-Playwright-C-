using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;
using System.Text.RegularExpressions;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class SnipeITTest1 : PageTest
{
    private string assetName = $"Macbook Pro 13 - {Guid.NewGuid()}";

    [Test]
    public async Task AutomateSnipeITAssetWorkflow1()
    {
        // Login to the application
        await Page.GotoAsync("https://demo.snipeitapp.com/login");
        await Page.FillAsync("input[name='username']", "admin");
        await Page.FillAsync("input[name='password']", "password");
        await Page.ClickAsync("button.btn-primary");
        var dashboardTitle = await Page.InnerTextAsync("h1.pull-left.pagetitle");
        Assert.That(dashboardTitle, Is.EqualTo("Dashboard"), "The page is not the Dashboard after login.");

        // Navigate to Assets and create a new asset
        var createNewLink = Page.Locator("a.dropdown-toggle", new() { HasText = "Create New" });
        await createNewLink.ClickAsync();

        // Wait for the specific dropdown menu related to "Create New" to be visible
        var createNewDropdown = Page.Locator("li.dropdown.open > ul.dropdown-menu");
        await createNewDropdown.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        // Now click on the "Asset" link within the dropdown menu
        var assetLink = createNewDropdown.Locator("a", new() { HasText = "Asset" });
        await assetLink.ClickAsync();

        // Wait for the URL to change to the asset creation page
        await Page.WaitForURLAsync("https://demo.snipeitapp.com/hardware/create");

        // wait for the "Create Asset" text to be visible to ensure the page has loaded
        var createAssetText = Page.Locator("h1.pagetitle", new() { HasText = "Create Asset" });
        await createAssetText.WaitForAsync(new() { State = WaitForSelectorState.Visible });

        //select model
        var modelDropdown = Page.Locator("#select2-model_select_id-container");
        await modelDropdown.ClickAsync();

        var searchInput = Page.Locator("span.select2-search.select2-search--dropdown input.select2-search__field");
        await searchInput.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 8000 });

        await searchInput.FillAsync("Apple macbook 13");

        var searchResults = Page.Locator("ul.select2-results__options");
        await searchResults.WaitForAsync(new() { State = WaitForSelectorState.Visible, Timeout = 6000 });

        var appleMacbookOption = Page.Locator("li.select2-results__option", new() { HasText = "CLaptops - Apple macbook 13" });
        await appleMacbookOption.ClickAsync();


        //Select Ready to deploy
        var statusDropdown = Page.Locator("#select2-status_select_id-container");
        await statusDropdown.ClickAsync();
        var dropdownOptions = Page.Locator("ul.select2-results__options");
        await dropdownOptions.WaitForAsync(new() { State = WaitForSelectorState.Visible });
        var readyToDeployOption = Page.Locator("li.select2-results__option", new() { HasText = "Ready to Deploy" });
        await readyToDeployOption.ClickAsync();


        // Assign the asset to a random user
        // Step 1: Click on the "Select a User" dropdown
        var userDropdown = Page.Locator("#select2-assigned_user_select-container");
        await userDropdown.ClickAsync();

        // Step 2: Search for "Abc" in the search box
        var userSearchInput = Page.Locator(".select2-search__field");
        await userSearchInput.FillAsync("Abc");

        // Step 3: Wait for the search results to appear and select the user
        var userOption = Page.Locator("li.select2-results__option", new() { HasText = "Abc, Xyz (Abc)" });
        await userOption.ClickAsync();


        //Save the asset
        // Click the second "Save" button using nth() for precise selection
        var saveButton = Page.Locator("button[type='submit'].btn.btn-primary.pull-right:has(i.fas.fa-check.icon-white)").Nth(1);
        await saveButton.ClickAsync();


        // Step 2: Wait for the success alert to appear
        var successAlert = Page.Locator("div.alert.alert-success.fade.in");
        await successAlert.WaitForAsync(new LocatorWaitForOptions { State = WaitForSelectorState.Visible });

        // Step 3: Extract the asset tag and URL from the success alert
        var successMessage = await successAlert.InnerTextAsync();
        var assetTagMatch = System.Text.RegularExpressions.Regex.Match(successMessage, @"Asset with tag (\d+) was created successfully");
        var assetTag = assetTagMatch.Groups[1].Value;
        var viewLink = Page.Locator("a:has-text('Click here to view')");
        await viewLink.ClickAsync();

        // Locate the status and user elements
        var statusLocator = Page.Locator("div.col-md-9 >> text='Ready to Deploy'");
        var userLocator = Page.Locator("div.col-md-9 >> a[href*='/users/64']");
        var modelLocator = Page.Locator("div.col-md-9 >> a[href*='/models/26']");
        var historyLocator = Page.Locator("a[href='#history'] >> text='History'");

        // Use ToHaveTextAsync to assert the expected text values
        await Expect(statusLocator).ToHaveTextAsync(new Regex("Ready to Deploy"));
        await Expect(userLocator).ToHaveTextAsync(new Regex("Abc Xyz"));
        await Expect(modelLocator).ToHaveTextAsync(new Regex("Apple macbook 13"));
        await Expect(historyLocator).ToHaveTextAsync(new Regex("History"));
        await historyLocator.ClickAsync();

        // Assert the user text "Abc Xyz" in the <td> element
        var userInHistoryLocator = Page.Locator("td >> a[href*='/users/64'] >> text='Abc Xyz'");
        await Expect(userInHistoryLocator).ToHaveTextAsync(new Regex("Abc Xyz"));

        // Assert the "Checked out on asset creation" text in the <td> element
        var checkoutTextLocator = Page.Locator("td >> text='Checked out on asset creation'");
        await Expect(checkoutTextLocator).ToHaveTextAsync(new Regex("Checked out on asset creation"));
    }
}
