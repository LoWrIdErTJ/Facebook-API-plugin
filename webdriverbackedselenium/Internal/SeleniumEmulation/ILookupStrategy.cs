using FB_API_Plugin.Webdriver;

namespace FB_API_Plugin.webdriverbackedselenium.Internal.SeleniumEmulation
{
    /// <summary>
    /// Provides methods for finding elements.
    /// </summary>
    internal interface ILookupStrategy
    {
        /// <summary>
        /// Finds an element.
        /// </summary>
        /// <param name="driver">The <see cref="IWebDriver"/> to use in finding the element.</param>
        /// <param name="use">The locator string to use.</param>
        /// <returns>An <see cref="IWebElement"/> that matches the locator string.</returns>
        IWebElement Find(IWebDriver driver, string use);
    }
}
