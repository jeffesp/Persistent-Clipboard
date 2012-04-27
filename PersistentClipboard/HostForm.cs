/* Copyright (c) 2010 Jeff Espenschied

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
*/

using System;
using System.Linq;
using System.Windows.Forms;

namespace PersistentClipboard
{
    public partial class HostForm : Form
    {
        private IClicpboardCollector collectionForm;
        private GlobalHotkey hotkey;
        private bool searching = false;

        public HostForm()
        {
            InitializeComponent();
            Load += new EventHandler(HostForm_Load);
            Activated += new EventHandler(HostForm_Activated);
            Deactivate += new EventHandler(HostForm_Deactivate);
            FormClosing += new FormClosingEventHandler(HostForm_FormClosing);
        }

        void HostForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            collectionForm.Dispose();
        }

        void HostForm_Activated(object sender, EventArgs e)
        {
            Program.Logger.Debug("Activated.");
            UpdateItems();
            TopMost = true;
            clippedListBox.Focus();
            Focus();
        }

        void HostForm_Deactivate(object sender, EventArgs e)
        {
            Program.Logger.Debug("Deactivated. Hiding.");
            if (searching)
            {
                searchLabel.Text = String.Empty;
                searchLabel.Hide();
                searching = false;
            }
            Hide();
        }

        private void HostForm_Load(System.Object sender, System.EventArgs e)
        {
            collectionForm = new CollectionForm();
            collectionForm.EnableCollection();
            hotkey = new GlobalHotkey(KeyboardHookKeyDown, Keys.Insert, Keys.Control | Keys.Shift);
            UpdateItems();
        }

        private void UpdateClipboardWithSelectedItem()
        {
            if (clippedListBox.SelectedItem != null)
            {
                collectionForm.DisableCollection();
                Clipboard.SetText(clippedListBox.SelectedItem.ToString());
                collectionForm.EnableCollection();
            }
        }

        private void clippedListBox_Click(object sender, EventArgs e)
        {
            UpdateClipboardWithSelectedItem();
            Hide();
            Program.Logger.DebugFormat("Selected: {0}", clippedListBox.SelectedItem);
        }

        private void UpdateItems()
        {
            UpdateItems(String.Empty);
        }

        private void UpdateItems(string searchText)
        {
            if (collectionForm != null && collectionForm.HasItems)
            {
                clippedListBox.Items.Clear();
                clippedListBox.Items.AddRange(collectionForm.Search(searchText).Take(30).ToArray());
                if (clippedListBox.Items.Count > 1)
                    clippedListBox.SelectedIndex = String.IsNullOrEmpty(searchText) ? 1 : 0;
            }
        }

        private void clippedListBox_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = false;

            if (searching)
            {
                // Escape means stop searching, Delete or Backspace removes last character
                if (e.KeyCode == Keys.Escape)
                {
                    searching = false;
                    searchLabel.Text = String.Empty;
                    searchLabel.Visible = false;
                    UpdateItems();
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Back && searchLabel.Text.Length > 0)
                {
                    searchLabel.Text = searchLabel.Text.Substring(0, searchLabel.Text.Length - 1);
                    UpdateItems(searchLabel.Text);
                    e.Handled = true;
                }
                else
                {
                    var kc = new KeysConverter();
                    string keyCodeString = kc.ConvertToString(e.KeyCode);
                    if (keyCodeString.Length == 1)
                    {
                        Program.Logger.DebugFormat("got char: {0}", keyCodeString);
                        searchLabel.Text += e.Shift ? keyCodeString : keyCodeString.ToLower();
                        UpdateItems(searchLabel.Text);
                        e.Handled = true;
                    }
                }
            }
            else
            {
                if (e.KeyCode == Keys.OemQuestion)
                {
                    searching = true;
                    searchLabel.Visible = true;
                    searchLabel.Text = String.Empty;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Delete)
                {
                    // Remove current item
                    int currentIndex = clippedListBox.SelectedIndex;
                    collectionForm.RemoveItem((ClippedItem)clippedListBox.SelectedItem);
                    clippedListBox.Items.RemoveAt(currentIndex);
                    clippedListBox.SelectedIndex = currentIndex > clippedListBox.Items.Count ? currentIndex + 1 : currentIndex - 1;
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    Hide();
                    e.Handled = true;
                }
            }

            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                UpdateClipboardWithSelectedItem();
                Hide();
                e.Handled = true;
                Program.Logger.DebugFormat("Selected: {0}", clippedListBox.SelectedItem);
            }
        }

        private void KeyboardHookKeyDown()
        {
            Program.Logger.DebugFormat("Caught keyboard hook. Currently: {0}", this.Visible ? "Visible" : "Not Visible");
            Show(DesktopWindow.Instance);
        }

        private void searchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                Hide();
                e.Handled = true;
                return;
            }
            e.Handled = false;
        }
    }
}
