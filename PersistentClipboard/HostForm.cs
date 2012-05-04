using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace PersistentClipboard
{
    public partial class HostForm : Form
    {
        private IClicpboardCollector collectionForm;
        private GlobalHotkey hotkey;
        private bool searching = false;
        private readonly ILog logger;

        public HostForm(ILog logger)
        {
            this.logger = logger;

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
            logger.Debug("Activated.");
            UpdateItems();
            TopMost = true;
            clippedListBox.Focus();
            Focus();
        }

        void HostForm_Deactivate(object sender, EventArgs e)
        {
            logger.Debug("Deactivated. Hiding.");
            if (searching)
            {
                searchText.Text = String.Empty;
                searchText.Hide();
                searching = false;
            }
            Hide();
        }

        private void HostForm_Load(System.Object sender, System.EventArgs e)
        {
            collectionForm = new CollectionForm(logger);
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
                logger.DebugFormat("{0}, {1}", clippedListBox.SelectedIndex, clippedListBox.SelectedItem);
                collectionForm.EnableCollection();
            }
        }

        private void clippedListBox_Click(object sender, EventArgs e)
        {
            UpdateClipboardWithSelectedItem();
            Hide();
            logger.DebugFormat("Selected: {0}", clippedListBox.SelectedItem);
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

            if (e.KeyCode == Keys.OemQuestion)
            {
                searching = true;
                searchText.Visible = true;
                searchText.Text = String.Empty;
                searchText.Focus();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Delete)
            {
                RemoveItem();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Escape)
            {
                Hide();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                logger.DebugFormat("Handling enter/return. SelectedIndex: {0}", clippedListBox.SelectedIndex);
                SelectItem();
                e.Handled = true;
            }
        }

        private void RemoveItem()
        {
            logger.DebugFormat("Deleting Item. SelectedIndex: {0}", clippedListBox.SelectedIndex);
            int currentIndex = clippedListBox.SelectedIndex;
            collectionForm.RemoveItem((ClippedItem)clippedListBox.SelectedItem);
            clippedListBox.Items.RemoveAt(currentIndex);
            clippedListBox.SelectedIndex = Math.Min(currentIndex, clippedListBox.Items.Count - 1);
        }

        private void SelectItem()
        {
            UpdateClipboardWithSelectedItem();
            logger.DebugFormat("Selected: {0}", clippedListBox.SelectedItem);
            Hide();
        }

        private void KeyboardHookKeyDown()
        {
            logger.DebugFormat("Caught keyboard hook. Currently: {0}", this.Visible ? "Visible" : "Not Visible");
            Show(DesktopWindow.Instance);
        }

        private void searchText_TextChanged(object sender, EventArgs e)
        {
            UpdateItems(searchText.Text);
        }

        private void searchText_KeyUp(object sender, KeyEventArgs e)
        {
            e.Handled = false;
            // Escape means stop searching
            if (e.KeyCode == Keys.Escape)
            {
                searching = false;
                searchText.Text = String.Empty;
                searchText.Visible = false;
                UpdateItems();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                SelectItem();
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up)
            {
                if (clippedListBox.SelectedIndex > 0)
                    clippedListBox.SelectedIndex--;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Down)
            {
                if (clippedListBox.SelectedIndex < clippedListBox.Items.Count)
                    clippedListBox.SelectedIndex++;
                e.Handled = true;
            }
        }
    }
}
