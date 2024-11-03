## Changelog

#### v2.3.0 - 07/03/2019

- Reset the sort. The next click after sorting the rows ascending and descending will reset the sorting to
the initial sort.
- Group rows by a specific column value or use a closure that processes the column value first.

#### v2.1.0 - 15/02/2019

- Combined the basic table component and the async table component back to one component
- Added an excel export

#### v2.0.0 - 04/02/2019

- Split the table tree in 3 components:
    - the basic table component
    - the async table component
    - the full component (with a filter box and a pagination component)

#### v1.5.0 - 27/01/2019

- Make it possible to show/hide columns.

#### v1.4.0 - 27/01/2019

- Add underscores as a prefix to the meta data of row objects.
- Removed the possibility to don't store async items in the cache.
- Don't create new objects from rows and columns but use them as they were created.


#### v1.3.0 - 04/01/2019

- make the properties of the rows reactive.

#### v1.2.5 - 18/12/2018

- Use system ui as the table tree font
- Remove the table title by default because you will not use a title with the value 'title'.

#### v1.2.4 - 07/12/2018

- Bugfix on formatting the cell
- Add the collapse/expand icon in a preferred column.

#### v1.2.3 - 07/12/2018

- Bugfix if an item was loaded on the zero index.
- Only remove the pagination on filtering and loading.

#### v1.2.2 - 27/11/2018

- Small improvements
- Update pagination component

#### v1.2.1 - 24/11/2018

- Removed the width property for columns because it's now possible with the styling object.
- Removed the start and end properties used by the pagination template. Now you have to call the pagination pageChange method.
- Seperate config files from the package.json
- Update eslint

#### v1.2.0 - 23/11/2018

- Make a template of the filter input field and label.
- Make a template of the pagination component.
- Review the way of styling the table.
- Don't call a method to execute the render of the table, but use computed properties.

#### v1.1.0 - 23/08/2018

- A table cell can be overwritten with a custom template. So it's possible to use components in the cell.
- Added a timeout when typing in the filter field, if an async call has to be made.

#### v1.0.0 - 15/08/2018

- Initial release.
