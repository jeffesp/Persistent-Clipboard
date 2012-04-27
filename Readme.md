Persistent Clipboard

Saves clipboard entries for later use. Keeps the information around across 
restarts. In fact, it currently keeps them around forever.

- Will not save blank or duplicate (same text twice in a row) entries.
- Ctrl+Shift+F12 will show the form.
- Enter or Click on an item puts that item on the clipboard and hides the form.
- Esc will hide the form without making a selection

If you use it and run Visual Studio as an Administrator, you will need to run 
this as an admin as well.

TODO:
0. Remove old entries from database.
1. Allow portable mode which stores data in a subdirectory instead of user profile.
2. Hide on startup? Show on startup? What buttons to display (min, max, close)?
3. Context menu on list
  a. Delete
  b. Clear
4. NotifyIcon and splash screen
5. Searching the history and display.
6. Auto paste on select - how do you even do that?
7. Allow user defined static entries.

LICENSE

The MIT License

Copyright (c) 2010 Jeff Espenschied

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