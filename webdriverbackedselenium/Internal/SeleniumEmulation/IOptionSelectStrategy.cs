using System.Collections.ObjectModel;
using FB_API_Plugin.Webdriver;

namespace FB_API_Plugin.webdriverbackedselenium.Internal.SeleniumEmulation
{
    /// <summary>
    /// Provides a method by which to select options.
    /// </summary>
    internal interface IOptionSelectStrategy
    {
        /// <summary>
        /// Selects an option.
        /// </summary>
        /// <param name="fromOptions">A list of options to select from.</param>
        /// <param name="selectThis">The option to select.</param>
        /// <param name="setSelected"><see langword="true"/> to select the option; <see langword="false"/> to unselect.</param>
        /// <param name="allowMultipleSelect"><see langword="true"/> to allow multiple selections; otherwise, <see langword="false"/>.</param>
        /// <returns><see langword="true"/> if the option is selected; otherwise, <see langword="false"/>.</returns>
        bool SelectOption(ReadOnlyCollection<IWebElement> fromOptions, string selectThis, bool setSelected, bool allowMultipleSelect);
    }
}
