## Changelog

#### v2.1.1 - 14/01/2019

- Split the page-change event in a page-change event and a range-change event: 
If you dont have a second page by default and you're adding items, then the external end variable won't change
because no page-change event was triggered. This is now solved via the range-change event.

#### v2.0.5 - 27/11/2018

- Not all pages were shown properly. Fixed it.

#### v2.0.4 - 24/11/2018

- Prefix the tailwind classes with `vue-ads-`.

#### v2.0.3 - 08/11/2018

- Prefix the tailwind classes with `ads-`.

#### v2.0.2 - 08/11/2018

- Add a disable-styling property on the VueAdsPageButton to remove all stylings on the button.

#### v2.0.0 - 09/10/2018

- Small rewrite of the paginator:
    - Make it possible to use your own buttons or extend the default VueAdsPageButton, by adding a buttons template.
    - So now we could remove the detailClasses and buttonClasses for custom styling.
    - Remove the range object and add their attributes as clear properties. Only use start and end anymore.
    - Purge the css.
    - Move configs from package.json to separate files.

#### v1.1.0 - 22/08/2018

- Add a loading property to inidicate if the current page is loading
- Add the detailClasses and buttonClasses property to custom style the detail box and the buttons

#### v1.0.1 - 14/08/2018

- Make the emitted events case insensitive.

#### v1.0.0 - 14/08/2018

- Initial release.
