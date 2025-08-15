using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SearchEngineGUI
{
    // Data model for search results - moved to top level namespace
    public class SearchResultItem
    {
        public string Title { get; set; } = "";
        public string FileType { get; set; } = "";
        public string FileSize { get; set; } = "";
        public long FileSizeBytes { get; set; }
        public DateTime LastModified { get; set; }
        public double RelevanceScore { get; set; }
        public string Preview { get; set; } = "";
        public string FilePath { get; set; } = "";
    }

    public partial class MainForm : Form
    {
        private TextBox? searchTextBox;
        private Button? searchButton;
        private Button? uploadButton;
        private Button? clearButton;
        private ListBox? autoCompleteListBox;
        private DataGridView? resultsGrid;
        private Label? statusLabel;
        private Label? resultsCountLabel;
        private ProgressBar? searchProgressBar;
        private Label? uploadStatusLabel;
        private ComboBox? fileTypeFilter;
        private ComboBox? sortComboBox;
        private System.Windows.Forms.Timer? searchTimer;
        private bool isSearching = false;

        // TODO: Replace with actual SearchEngine class from your team
        // private SearchEngine searchEngine;

        public MainForm()
        {
            InitializeComponent();
            SetupAutoComplete();
            // searchEngine = new SearchEngine(); // Initialize your team's search engine
        }

        private void InitializeComponent()
        {
            this.SuspendLayout();

            // Form setup
            this.Text = "University Document Search Engine";
            this.Size = new Size(1200, 800);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MinimumSize = new Size(1000, 600);
            this.BackColor = Color.WhiteSmoke;

            // Title Label
            Label titleLabel = new Label()
            {
                Text = "University Search Engine",
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                ForeColor = Color.DarkBlue,
                Location = new Point(20, 10),
                Size = new Size(400, 35),
                TextAlign = ContentAlignment.MiddleLeft
            };

            // Search Panel
            Panel searchPanel = new Panel()
            {
                Location = new Point(20, 55),
                Size = new Size(1140, 100),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Search TextBox
            searchTextBox = new TextBox()
            {
                Location = new Point(15, 15),
                Size = new Size(500, 30),
                Font = new Font("Segoe UI", 12),
                PlaceholderText = "Search for documents, keywords, or content..."
            };
            searchTextBox.KeyDown += SearchTextBox_KeyDown;
            searchTextBox.TextChanged += SearchTextBox_TextChanged;

            // Search Button
            searchButton = new Button()
            {
                Location = new Point(525, 15),
                Size = new Size(100, 30),
                Text = "Search",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.DodgerBlue,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            searchButton.Click += SearchButton_Click;

            // Upload Button
            uploadButton = new Button()
            {
                Location = new Point(635, 15),
                Size = new Size(120, 30),
                Text = "Upload Files",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                BackColor = Color.ForestGreen,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            uploadButton.Click += UploadButton_Click;

            // Clear Button
            clearButton = new Button()
            {
                Location = new Point(765, 15),
                Size = new Size(80, 30),
                Text = "Clear",
                Font = new Font("Segoe UI", 10),
                BackColor = Color.Gray,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            clearButton.Click += ClearButton_Click;

            // File Type Filter
            Label filterLabel = new Label()
            {
                Text = "File Type:",
                Location = new Point(15, 55),
                Size = new Size(70, 20),
                Font = new Font("Segoe UI", 9)
            };

            fileTypeFilter = new ComboBox()
            {
                Location = new Point(85, 53),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            fileTypeFilter.Items.AddRange(new string[] { "All Files", "PDF", "Word", "PowerPoint", "Excel", "Text", "HTML", "XML" });
            fileTypeFilter.SelectedIndex = 0;
            fileTypeFilter.SelectedIndexChanged += FilterChanged;

            // Sort ComboBox
            Label sortLabel = new Label()
            {
                Text = "Sort by:",
                Location = new Point(220, 55),
                Size = new Size(60, 20),
                Font = new Font("Segoe UI", 9)
            };

            sortComboBox = new ComboBox()
            {
                Location = new Point(280, 53),
                Size = new Size(120, 25),
                DropDownStyle = ComboBoxStyle.DropDownList,
                Font = new Font("Segoe UI", 9)
            };
            sortComboBox.Items.AddRange(new string[] { "Relevance", "Date Modified", "File Name", "File Size" });
            sortComboBox.SelectedIndex = 0;
            sortComboBox.SelectedIndexChanged += SortChanged;

            // Search Progress Bar
            searchProgressBar = new ProgressBar()
            {
                Location = new Point(855, 15),
                Size = new Size(200, 30),
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Visible = false
            };

            // AutoComplete ListBox
            autoCompleteListBox = new ListBox()
            {
                Location = new Point(15, 45),
                Size = new Size(500, 100),
                Font = new Font("Segoe UI", 10),
                Visible = false,
                BackColor = Color.LightYellow,
                BorderStyle = BorderStyle.FixedSingle
            };
            autoCompleteListBox.Click += AutoCompleteListBox_Click;

            // Add controls to search panel
            searchPanel.Controls.AddRange(new Control[] {
                searchTextBox, searchButton, uploadButton, clearButton,
                filterLabel, fileTypeFilter, sortLabel, sortComboBox,
                searchProgressBar, autoCompleteListBox
            });

            // Results Panel
            Panel resultsPanel = new Panel()
            {
                Location = new Point(20, 165),
                Size = new Size(1140, 450),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Results Count Label
            resultsCountLabel = new Label()
            {
                Location = new Point(15, 10),
                Size = new Size(400, 20),
                Font = new Font("Segoe UI", 9),
                Text = "Ready to search..."
            };

            // Results DataGrid
            resultsGrid = new DataGridView()
            {
                Location = new Point(15, 35),
                Size = new Size(1110, 400),
                Font = new Font("Segoe UI", 10),
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            // Setup DataGrid columns
            resultsGrid.Columns.Add("Title", "Document Title");
            resultsGrid.Columns.Add("Type", "Type");
            resultsGrid.Columns.Add("Size", "Size");
            resultsGrid.Columns.Add("Modified", "Date Modified");
            resultsGrid.Columns.Add("Relevance", "Relevance %");
            resultsGrid.Columns.Add("Preview", "Preview");

            // Set column widths
            resultsGrid.Columns[0].Width = 250; // Title
            resultsGrid.Columns[1].Width = 80;  // Type
            resultsGrid.Columns[2].Width = 80;  // Size
            resultsGrid.Columns[3].Width = 120; // Modified
            resultsGrid.Columns[4].Width = 100; // Relevance
            resultsGrid.Columns[5].Width = 300; // Preview

            resultsGrid.CellDoubleClick += ResultsGrid_CellDoubleClick;
            resultsGrid.CellFormatting += ResultsGrid_CellFormatting;

            resultsPanel.Controls.AddRange(new Control[] { resultsCountLabel, resultsGrid });

            // Status Panel
            Panel statusPanel = new Panel()
            {
                Location = new Point(20, 625),
                Size = new Size(1140, 40),
                BackColor = Color.LightGray,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Status Label
            statusLabel = new Label()
            {
                Location = new Point(15, 10),
                Size = new Size(800, 20),
                Font = new Font("Segoe UI", 9),
                Text = "Ready. Upload documents or start searching.",
                BackColor = Color.Transparent
            };

            // Upload Status Label
            uploadStatusLabel = new Label()
            {
                Location = new Point(820, 10),
                Size = new Size(300, 20),
                Font = new Font("Segoe UI", 9),
                Text = "",
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent
            };

            statusPanel.Controls.AddRange(new Control[] { statusLabel, uploadStatusLabel });

            // Add all main controls to form
            this.Controls.AddRange(new Control[] {
                titleLabel, searchPanel, resultsPanel, statusPanel
            });

            this.ResumeLayout();
        }

        private void SetupAutoComplete()
        {
            searchTimer = new System.Windows.Forms.Timer()
            {
                Interval = 300 // 300ms delay for autocomplete
            };
            searchTimer.Tick += SearchTimer_Tick;
        }

        private void SearchTextBox_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                _ = PerformSearch();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideAutoComplete();
            }
            else if (e.KeyCode == Keys.Down && autoCompleteListBox?.Visible == true)
            {
                if (autoCompleteListBox.SelectedIndex < autoCompleteListBox.Items.Count - 1)
                    autoCompleteListBox.SelectedIndex++;
                e.Handled = true;
            }
            else if (e.KeyCode == Keys.Up && autoCompleteListBox?.Visible == true)
            {
                if (autoCompleteListBox.SelectedIndex > 0)
                    autoCompleteListBox.SelectedIndex--;
                e.Handled = true;
            }
        }

        private void SearchTextBox_TextChanged(object? sender, EventArgs e)
        {
            searchTimer?.Stop();
            searchTimer?.Start();
        }

        private void SearchTimer_Tick(object? sender, EventArgs e)
        {
            searchTimer?.Stop();

            string query = searchTextBox?.Text?.Trim() ?? "";
            if (query.Length >= 2)
            {
                ShowAutoComplete(query);
            }
            else
            {
                HideAutoComplete();
            }
        }

        private void ShowAutoComplete(string query)
        {
            try
            {
                // TODO: Replace with actual autocomplete from your team's API
                var suggestions = GetAutoCompleteSuggestions(query);

                if (suggestions.Any() && autoCompleteListBox != null)
                {
                    autoCompleteListBox.Items.Clear();
                    foreach (var suggestion in suggestions)
                    {
                        autoCompleteListBox.Items.Add(suggestion);
                    }
                    autoCompleteListBox.Visible = true;
                    autoCompleteListBox.BringToFront();
                }
                else
                {
                    HideAutoComplete();
                }
            }
            catch (Exception ex)
            {
                if (statusLabel != null)
                    statusLabel.Text = "Autocomplete error: " + ex.Message;
            }
        }

        private void HideAutoComplete()
        {
            if (autoCompleteListBox != null)
                autoCompleteListBox.Visible = false;
        }

        private void AutoCompleteListBox_Click(object? sender, EventArgs e)
        {
            if (autoCompleteListBox?.SelectedItem != null && searchTextBox != null)
            {
                searchTextBox.Text = autoCompleteListBox.SelectedItem.ToString() ?? "";
                HideAutoComplete();
                _ = PerformSearch();
            }
        }

        private async void SearchButton_Click(object? sender, EventArgs e)
        {
            await PerformSearch();
        }

        private async Task PerformSearch()
        {
            if (isSearching) return;

            string query = searchTextBox?.Text?.Trim() ?? "";
            if (string.IsNullOrEmpty(query))
            {
                MessageBox.Show("Please enter a search query.", "Search", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            try
            {
                isSearching = true;
                if (searchButton != null) searchButton.Enabled = false;
                if (searchProgressBar != null) searchProgressBar.Visible = true;
                if (statusLabel != null) statusLabel.Text = "Searching...";
                HideAutoComplete();

                var startTime = DateTime.Now;

                // TODO: Replace with actual search call to your team's SearchEngine
                var results = await SearchDocuments(query);

                var endTime = DateTime.Now;
                var searchTime = (endTime - startTime).TotalSeconds;

                DisplayResults(results);
                if (resultsCountLabel != null)
                    resultsCountLabel.Text = $"Found {results.Count} results in {searchTime:F3} seconds";
                if (statusLabel != null)
                    statusLabel.Text = $"Search completed. Found {results.Count} results.";

            }
            catch (Exception ex)
            {
                MessageBox.Show($"Search failed: {ex.Message}", "Search Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (statusLabel != null)
                    statusLabel.Text = "Search failed: " + ex.Message;
            }
            finally
            {
                isSearching = false;
                if (searchButton != null) searchButton.Enabled = true;
                if (searchProgressBar != null) searchProgressBar.Visible = false;
            }
        }

        private void DisplayResults(List<SearchResultItem> results)
        {
            resultsGrid?.Rows.Clear();

            if (resultsGrid == null) return;

            foreach (var result in results)
            {
                var row = new DataGridViewRow();
                row.Cells.Add(new DataGridViewTextBoxCell { Value = result.Title });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = result.FileType });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = result.FileSize });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = result.LastModified.ToString("MMM dd, yyyy") });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = $"{result.RelevanceScore:F1}%" });
                row.Cells.Add(new DataGridViewTextBoxCell { Value = result.Preview });

                row.Tag = result; // Store full result object
                resultsGrid.Rows.Add(row);
            }
        }

        private void ResultsGrid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 4 && e.Value != null) // Relevance column
            {
                var relevanceText = e.Value.ToString() ?? "";
                if (double.TryParse(relevanceText.Replace("%", ""), out double relevance))
                {
                    if (relevance >= 80)
                        e.CellStyle.BackColor = Color.LightGreen;
                    else if (relevance >= 60)
                        e.CellStyle.BackColor = Color.LightYellow;
                    else
                        e.CellStyle.BackColor = Color.LightPink;
                }
            }
        }

        private void ResultsGrid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && resultsGrid != null)
            {
                var result = resultsGrid.Rows[e.RowIndex].Tag as SearchResultItem;
                if (result != null)
                    OpenDocument(result);
            }
        }

        private void OpenDocument(SearchResultItem result)
        {
            try
            {
                // TODO: Implement document viewer or external opening
                var viewerForm = new DocumentViewerForm(result);
                viewerForm.ShowDialog();

                if (statusLabel != null)
                    statusLabel.Text = $"Opened: {result.Title}";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Cannot open document: {ex.Message}", "Open Document", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UploadButton_Click(object? sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.Filter = "All Supported|*.pdf;*.doc;*.docx;*.ppt;*.pptx;*.xls;*.xlsx;*.txt;*.html;*.xml|" +
                               "PDF Files|*.pdf|" +
                               "Word Documents|*.doc;*.docx|" +
                               "PowerPoint|*.ppt;*.pptx|" +
                               "Excel Files|*.xls;*.xlsx|" +
                               "Text Files|*.txt|" +
                               "Web Files|*.html;*.xml";
                dialog.Multiselect = true;
                dialog.Title = "Select documents to upload";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    UploadFiles(dialog.FileNames);
                }
            }
        }

        private async void UploadFiles(string[] filePaths)
        {
            try
            {
                if (uploadButton != null) uploadButton.Enabled = false;
                if (uploadStatusLabel != null) uploadStatusLabel.Text = "Uploading files...";

                int successCount = 0;
                int totalCount = filePaths.Length;

                foreach (string filePath in filePaths)
                {
                    try
                    {
                        // TODO: Replace with actual upload call to your team's indexer
                        await UploadAndIndexFile(filePath);
                        successCount++;

                        if (uploadStatusLabel != null)
                            uploadStatusLabel.Text = $"Uploading... {successCount}/{totalCount}";
                        Application.DoEvents(); // Update UI
                    }
                    catch (Exception ex)
                    {
                        var fileName = Path.GetFileName(filePath);
                        if (statusLabel != null)
                            statusLabel.Text = $"Failed to upload {fileName}: {ex.Message}";
                    }
                }

                if (uploadStatusLabel != null)
                    uploadStatusLabel.Text = $"Uploaded {successCount}/{totalCount} files";
                if (statusLabel != null)
                    statusLabel.Text = $"Upload completed. {successCount} files indexed successfully.";

                if (successCount < totalCount)
                {
                    MessageBox.Show($"Upload completed with issues.\n{successCount} files uploaded successfully.\n{totalCount - successCount} files failed.",
                                  "Upload Status", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"All {successCount} files uploaded and indexed successfully!",
                                  "Upload Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Upload failed: {ex.Message}", "Upload Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (statusLabel != null) statusLabel.Text = "Upload failed";
            }
            finally
            {
                if (uploadButton != null) uploadButton.Enabled = true;
                if (uploadStatusLabel != null) uploadStatusLabel.Text = "";
            }
        }

        private void ClearButton_Click(object? sender, EventArgs e)
        {
            searchTextBox?.Clear();
            resultsGrid?.Rows.Clear();
            if (resultsCountLabel != null) resultsCountLabel.Text = "Ready to search...";
            if (statusLabel != null) statusLabel.Text = "Search cleared.";
            HideAutoComplete();
        }

        private void FilterChanged(object? sender, EventArgs e)
        {
            // Re-run search with new filter if there are results
            if (resultsGrid?.Rows.Count > 0 && !string.IsNullOrEmpty(searchTextBox?.Text))
            {
                _ = PerformSearch();
            }
        }

        private void SortChanged(object? sender, EventArgs e)
        {
            // Re-sort current results
            if (resultsGrid?.Rows.Count > 0)
            {
                var results = resultsGrid.Rows.Cast<DataGridViewRow>()
                                          .Where(r => r.Tag is SearchResultItem)
                                          .Select(r => (SearchResultItem)r.Tag!)
                                          .ToList();

                switch (sortComboBox?.SelectedItem?.ToString())
                {
                    case "Relevance":
                        results = results.OrderByDescending(r => r.RelevanceScore).ToList();
                        break;
                    case "Date Modified":
                        results = results.OrderByDescending(r => r.LastModified).ToList();
                        break;
                    case "File Name":
                        results = results.OrderBy(r => r.Title).ToList();
                        break;
                    case "File Size":
                        results = results.OrderByDescending(r => r.FileSizeBytes).ToList();
                        break;
                }

                DisplayResults(results);
            }
        }

        // TODO: Replace these methods with actual calls to your team's SearchEngine

        private List<string> GetAutoCompleteSuggestions(string query)
        {
            // Mock autocomplete - replace with actual implementation
            var suggestions = new List<string>
            {
                query + " algorithm",
                query + " data structure",
                query + " programming",
                query + " computer science",
                query + " tutorial"
            };

            return suggestions.Take(5).ToList();
        }

        private async Task<List<SearchResultItem>> SearchDocuments(string query)
        {
            // Mock search results - replace with actual search engine call
            await Task.Delay(100); // Simulate search delay

            var mockResults = new List<SearchResultItem>
            {
                new SearchResultItem
                {
                    Title = "Introduction to Machine Learning",
                    FileType = "PDF",
                    FileSize = "2.3 MB",
                    FileSizeBytes = 2411724,
                    LastModified = DateTime.Now.AddDays(-2),
                    RelevanceScore = 95.5,
                    Preview = "This document covers fundamental concepts of machine learning including supervised learning...",
                    FilePath = @"C:\docs\ml_intro.pdf"
                },
                new SearchResultItem
                {
                    Title = "Data Structures and Algorithms",
                    FileType = "DOC",
                    FileSize = "1.8 MB",
                    FileSizeBytes = 1887436,
                    LastModified = DateTime.Now.AddDays(-5),
                    RelevanceScore = 87.2,
                    Preview = "Comprehensive guide to data structures including arrays, linked lists, trees...",
                    FilePath = @"C:\docs\data_structures.doc"
                }
            };

            return mockResults;
        }

        private async Task UploadAndIndexFile(string filePath)
        {
            // Mock upload - replace with actual indexer call
            await Task.Delay(200); // Simulate upload delay

            // Validate file exists and is supported type
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found: {filePath}");

            var extension = Path.GetExtension(filePath).ToLower();
            var supportedTypes = new[] { ".pdf", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".txt", ".html", ".xml" };

            if (!supportedTypes.Contains(extension))
                throw new NotSupportedException($"File type not supported: {extension}");

            // TODO: Call your team's document indexer here
            // searchEngine.IndexDocument(filePath);
        }
    }

    // Simple Document Viewer Form
    public partial class DocumentViewerForm : Form
    {
        private readonly SearchResultItem document;
        private RichTextBox? contentTextBox;

        public DocumentViewerForm(SearchResultItem doc)
        {
            this.document = doc;
            InitializeComponent();
            LoadDocument();
        }

        private void InitializeComponent()
        {
            this.Text = $"Document Viewer - {document.Title}";
            this.Size = new Size(800, 600);
            this.StartPosition = FormStartPosition.CenterParent;

            // Content TextBox
            contentTextBox = new RichTextBox()
            {
                Dock = DockStyle.Fill,
                Font = new Font("Consolas", 10),
                ReadOnly = true,
                BackColor = Color.White
            };

            // Toolbar
            ToolStrip toolbar = new ToolStrip();
            toolbar.Items.Add("Close", null, (s, e) => this.Close());
            toolbar.Items.Add("Print", null, (s, e) => PrintDocument());
            toolbar.Items.Add("Save As...", null, (s, e) => SaveDocument());

            this.Controls.Add(contentTextBox);
            this.Controls.Add(toolbar);
        }

        private void LoadDocument()
        {
            try
            {
                // TODO: Load actual document content based on file type
                // For now, show basic info
                if (contentTextBox != null)
                {
                    contentTextBox.Text = $"Document: {document.Title}\n" +
                                         $"File Type: {document.FileType}\n" +
                                         $"File Size: {document.FileSize}\n" +
                                         $"Last Modified: {document.LastModified}\n" +
                                         $"File Path: {document.FilePath}\n\n" +
                                         $"Preview: {document.Preview}\n\n" +
                                         $"[Full document content would be loaded here based on file type]";
                }
            }
            catch (Exception ex)
            {
                if (contentTextBox != null)
                    contentTextBox.Text = $"Error loading document: {ex.Message}";
            }
        }

        private void PrintDocument()
        {
            MessageBox.Show("Print functionality would be implemented here.");
        }

        private void SaveDocument()
        {
            MessageBox.Show("Save As functionality would be implemented here.");
        }
    }
}