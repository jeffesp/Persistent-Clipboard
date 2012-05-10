# Persistent Clipboard

### Basic Description

Saves clipboard entries for later use. Keeps the information around across restarts. 

### Current Features

- Will not save blank or duplicate (same text twice in a row) entries.
- `Ctrl+Shift+Ins` will show the form.
- Enter or Click on an item puts that item on the clipboard and hides the form.
- `Esc` will hide the form without making a selection
- `/` or `?` then type in textbox to search history for specific item
	- `Esc` while searching will exit the search
- Note: If you run an app as an Administrator, you will need to run this as an admin as well to get the data from that app.

### Current TODO:

1. Add context menu when left clicking on item with delete and clear commands.
2. Configurable number of items in history.
2. Handle html, rtf, csv text types from clipboard.
3. Allow user defined static entries.
4. Support images from clipboard
	1. Processing of images so we are not storing bmps in the file system (?).
	2. Display thumbnail in the list.
	3. How do you search for an image?
5. Support files from clipboard
	1. What data comes in from the clipboard? Would we be able to get file content? Would we want to?
	2. How do you display that information in the list?
	3. Searching needed for filenames. Search on file attributes?
6. Support drag and drop 
	1. drop items to it instead of saving on cut/copy.
	2. drag from to put back on clipboard and paste into destination application.
	3. Will need to revisit how we show/hide in order to do this.
6. Allow portable mode which stores data in a subdirectory instead of user profile.

LICENSE

The MIT License

Copyright (c) 2012 Jeff Espenschied

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
