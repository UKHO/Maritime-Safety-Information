using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Playwright;
using NUnit.Framework.Legacy;

namespace UKHO.MaritimeSafetyInformation.PlaywrightTests.PageObjects
{
    internal class RadioNavigationalWarningsObject
    {
        private readonly IPage _page;
        public ILocator Reference { get; }
        public ILocator Datetime { get; }
        public ILocator Description { get; }
        public ILocator CreateNewRecord { get; }
        public ILocator Content { get; }
        public ILocator Create { get; }
        public ILocator Warning { get; }
        public ILocator WarningError { get; }
        public ILocator ReferenceError { get; }
        public ILocator DatetimeError { get; }
        public ILocator SummaryError { get; }
        public ILocator ContentError { get; }
        public ILocator EditNewRecord { get; }
        public ILocator WarningType { get; }
        public ILocator Year { get; }
        public ILocator Filter { get; }
        public ILocator Delete { get; }
        public ILocator Edit { get; }
        public ILocator ExpiryDate { get; }
        public ILocator AlertMessage { get; }
        public ILocator Overlay { get; }
        public ILocator Backdrop { get; }
        public ILocator ConfirmAlertClose { get; }
        public ILocator ConfirmAlertNo { get; }
        public ILocator ConfirmAlertYes { get; }
        public string Message => "A warning record with this reference number already exists. Would you like to add another record with the same reference?";

        public RadioNavigationalWarningsObject(IPage page)
        {
            _page = page;
            Reference = _page.Locator("#Reference");
            Datetime = _page.Locator("#DateTimeGroup");
            Description = _page.Locator("#Summary");
            CreateNewRecord = _page.Locator("text=Create New");
            Content = _page.Locator("#Content");
            Create = _page.Locator("#btnCreate");
            Warning = _page.Locator("#WarningType");
            WarningError = _page.Locator("#WarningType-error");
            ReferenceError = _page.Locator("#Reference-error");
            DatetimeError = _page.Locator("#DateTimeGroup-error");
            SummaryError = _page.Locator("#Summary-error");
            ContentError = _page.Locator("#Content-error");
            EditNewRecord = _page.Locator("#btnEdit");
            WarningType = _page.Locator("#WarningType");
            Year = _page.Locator("#Year");
            Filter = _page.Locator("#BtnFilter");
            Delete = _page.Locator("#IsDeleted");
            Edit = _page.Locator("[id^='Edit'] > a");
            ExpiryDate = _page.Locator("#ExpiryDate");
            AlertMessage = _page.Locator(".modal-body");
            Overlay = _page.Locator("#overlayId");
            Backdrop = _page.Locator("#backdropId");
            ConfirmAlertClose = _page.Locator("#modalConfirmCloseId");
            ConfirmAlertNo = _page.Locator("#modalConfirmNoId");
            ConfirmAlertYes = _page.Locator("#modalConfirmYesId");
        }

        public async Task SelectRadioNavigationalWarningAsync()
        {
            await CreateNewRecord.ClickAsync();
            Assert.That(await Warning.IsVisibleAsync());
        }

        public async Task CreateRNWAsync()
        {
            await Create.ClickAsync();
        }

        public async Task IsDeleteAsync()
        {
            await Delete.UncheckAsync();
            Assert.That(await Warning.IsVisibleAsync());
            
        }

        public async Task GetDialogTextAsync(string text)
        {
            _page.Dialog += async (_, dialog) =>
            {
                Assert.That(dialog.Message, Is.EqualTo(text));
                await dialog.AcceptAsync();
            };
            await Task.CompletedTask;
        }

        public async Task CheckErrorMessageAsync(ILocator locator, string text)
        {
            await _page.WaitForLoadStateAsync();
            var content = (await locator.TextContentAsync()) ?? string.Empty;
            Assert.That(content, Is.EqualTo(text));
        }

        public async Task FillFormWithValidDetailsAsync(string warningType, string content)
        {
            await Warning.SelectOptionAsync(new SelectOptionValue { Value = warningType });
            await Reference.FillAsync("reference");
            await _page.WaitForTimeoutAsync(3000);
            await Datetime.FillAsync("05072022 05533");
            await _page.Keyboard.PressAsync("ArrowDown");
            await Description.FillAsync("testdata");
            await Content.FillAsync(content);
            await ExpiryDate.FillAsync("05072022 06000");
        }

        public async Task ClearInputAsync(ILocator locator)
        {
            await locator.ClickAsync(new LocatorClickOptions { ClickCount = 3 });
            await _page.Keyboard.PressAsync("Control+A");
            await _page.Keyboard.PressAsync("Backspace");
        }

        public async Task EditRNWAsync()
        {
            await EditNewRecord.ClickAsync();
        }

        public async Task GetEditUrlAsync()
        {
            var newEdit = Edit.First;
            await newEdit.ClickAsync();
        }

        public async Task PageLoadAsync()
        {
            await Task.WhenAll(
                _page.WaitForNavigationAsync(),  //Rhz To Do
                CreateNewRecord.ClickAsync()
            );
        }

        public async Task SearchListWithFilterAsync(string selectWarnings)
        {
            await WarningType.SelectOptionAsync(new SelectOptionValue { Label = selectWarnings });
            await Filter.ClickAsync();
        }

        public async Task FillEditFormWithValidDetailsAsync(string content)
        {
            await Reference.FillAsync("reference");
            await _page.WaitForTimeoutAsync(3000);
            await Datetime.FillAsync("05072022 05533");
            await _page.Keyboard.PressAsync("ArrowDown");
            await Description.FillAsync("testdata");
            await Content.FillAsync(content);
        }

        public async Task ConfirmationBoxAsync(ILocator locator, string text, string alert)
        {
            await _page.WaitForLoadStateAsync();
            var content = (await locator.TextContentAsync())?.Trim() ?? string.Empty;
            Assert.That(content, Is.EqualTo(text));

            if (alert == "yes")
            {
                await ConfirmAlertYes.ClickAsync();
            }
            else if (alert == "no")
            {
                await ConfirmAlertNo.ClickAsync();
            }
            else
            {
                await ConfirmAlertClose.ClickAsync();
            }
        }

        public async Task CheckConfirmationBoxVisibleAsync(bool visible)
        {
            await _page.WaitForLoadStateAsync();

            if (visible)
            {
                StringAssert.Contains("d-block", await Overlay.GetAttributeAsync("class"));
                StringAssert.Contains("d-block", await Backdrop.GetAttributeAsync("class"));
            }
            else
            {
                StringAssert.Contains("d-none", await Overlay.GetAttributeAsync("class"));
                StringAssert.Contains("d-none", await Backdrop.GetAttributeAsync("class"));
            }
        }


    }
}
