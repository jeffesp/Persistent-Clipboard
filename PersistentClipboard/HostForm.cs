﻿using System;
using System.Linq;
using System.Windows.Forms;
using SimpleLogger;

namespace PersistentClipboard
{
    public partial class HostForm : Form
    {
        private IClicpboardCollector collectionForm;
        private bool searching;
        private readonly ISimpleLogger logger;
        private readonly GlobalHotkey hotkey;
        private readonly ContextMenuStrip trayIconContextMenu;

        public HostForm(ISimpleLogger logger)
        {
            this.logger = logger;

            InitializeComponent();
            Load += HostFormLoad;
            Activated += HostFormActivated;
            Deactivate += HostFormDeactivate;
            FormClosing += HostFormFormClosing;
            trayIcon.MouseClick += TrayIconClick;
            hotkey = new GlobalHotkey(KeyboardHookKeyDown, Keys.Insert, Keys.Control | Keys.Shift);

            trayIconContextMenu = new ContextMenuStrip();
            trayIconContextMenu.ShowImageMargin = false;
            var exitItem = trayIconContextMenu.Items.Add("Exit");
            exitItem.Click += ExitClick;

            trayIcon.ContextMenuStrip = trayIconContextMenu;
        }

        private void ExitClick(object sender, EventArgs e)
        {
            Close();
        }

        void HostFormFormClosing(object sender, FormClosingEventArgs e)
        {
        }

        void HostFormActivated(object sender, EventArgs e)
        {
            logger.Debug("Activated.");
            UpdateItems();
            TopMost = true;
            clippedListBox.Focus();
            Focus();
        }

        void HostFormDeactivate(object sender, EventArgs e)
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

        private void HostFormLoad(Object sender, EventArgs e)
        {
            collectionForm = new CollectionForm(logger);
            collectionForm.EnableCollection();
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

        private void ClippedListBoxClick(object sender, EventArgs e)
        {
            UpdateClipboardWithSelectedItem();
            Hide();
            logger.DebugFormat("Selected: {0}", clippedListBox.SelectedItem);
        }

        private void ClippedListBoxKeyDown(object sender, KeyEventArgs e)
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

        private void UpdateItems()
        {
            UpdateItems(String.Empty);
        }

        private void UpdateItems(string search)
        {
            if (collectionForm != null && collectionForm.HasItems)
            {
                clippedListBox.Items.Clear();
                clippedListBox.Items.AddRange(Enumerable.ToArray(collectionForm.Search(search)));
                if (clippedListBox.Items.Count > 1)
                    clippedListBox.SelectedIndex = String.IsNullOrEmpty(search) ? 1 : 0;
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
            logger.DebugFormat("Caught keyboard hook. Currently: {0}", Visible ? "Visible" : "Not Visible");
            Show();
        }

        private void SearchTextTextChanged(object sender, EventArgs e)
        {
            UpdateItems(searchText.Text);
        }

        private void SearchTextKeyUp(object sender, KeyEventArgs e)
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

        private void TrayIconClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                Show();
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
                if (trayIcon != null)
                    trayIcon.Dispose();
                if (collectionForm != null)
                    collectionForm.Dispose();
                if (hotkey != null)
                    hotkey.Dispose();
            }
            base.Dispose(disposing);
        }

        private void HostForm_Load(object sender, EventArgs e)
        {

        }
    }
}
