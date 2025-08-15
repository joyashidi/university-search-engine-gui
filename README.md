# university-search-engine-gui
GUI for university document search engine with upload, search, and viewer functionality


# University Document Search Engine - Integration Guide

## 🎯 What's Already Built

This GUI provides a complete, working search engine interface with:
- ✅ Document search with real-time results display
- ✅ File upload with progress tracking
- ✅ Auto-complete suggestions
- ✅ Advanced filtering (file type) and sorting
- ✅ Document viewer with basic display
- ✅ Professional UI with status indicators

## 🔧 What the Backend Team Needs to Implement

### 1. **Search Engine Core Class**
Create a `SearchEngine` class that the GUI can use:

```csharp
public class SearchEngine
{
    public async Task<List<SearchResult>> SearchAsync(string query, string fileType = "All Files")
    {
        // Your search implementation here
        // Should return list of documents matching the query
    }
    
    public async Task IndexDocumentAsync(string filePath)
    {
        // Your document parsing and indexing logic
        // Should extract text content and add to search index
    }
    
    public List<string> GetAutoCompleteSuggestions(string partialQuery)
    {
        // Return search suggestions based on partial input
        // Could be based on common searches, document titles, etc.
    }
}
```

### 2. **Document Parsing Requirements**

The GUI expects to handle these file types - you'll need parsers for:

| File Type | Extensions | What to Extract |
|-----------|------------|-----------------|
| **PDF** | `.pdf` | Text content, metadata |
| **Word** | `.doc`, `.docx` | Text content, headings |
| **PowerPoint** | `.ppt`, `.pptx` | Slide text, titles |
| **Excel** | `.xls`, `.xlsx` | Cell content, sheet names |
| **Text** | `.txt` | Raw text content |
| **Web** | `.html`, `.xml` | Text content, strip tags |

**Recommended Libraries:**
- **PDF**: iTextSharp, PDFsharp
- **Office Docs**: EPPlus, DocumentFormat.OpenXml
- **Text Extraction**: Any text processing library

### 3. **Search Result Data Structure**

Your search should return objects that match this structure:

```csharp
public class SearchResult
{
    public string Title { get; set; }           // Document title or filename
    public string FileType { get; set; }       // "PDF", "DOC", "TXT", etc.
    public string FileSize { get; set; }       // "2.3 MB", "156 KB"
    public long FileSizeBytes { get; set; }    // For sorting
    public DateTime LastModified { get; set; } // File modification date
    public double RelevanceScore { get; set; } // 0-100 percentage
    public string Preview { get; set; }        // First few lines of content
    public string FilePath { get; set; }       // Full path to file
}
```

## 🔌 Integration Points in GUI Code

### Replace These Mock Methods:

#### **1. Search Implementation** (Line ~539)
```csharp
private async Task<List<SearchResultItem>> SearchDocuments(string query)
{
    // REPLACE THIS with:
    var searchEngine = new SearchEngine();
    var results = await searchEngine.SearchAsync(query, fileTypeFilter?.SelectedItem?.ToString());
    return results.Select(r => new SearchResultItem 
    {
        Title = r.Title,
        FileType = r.FileType,
        // ... map other properties
    }).ToList();
}
```

#### **2. File Upload/Indexing** (Line ~587)
```csharp
private async Task UploadAndIndexFile(string filePath)
{
    // REPLACE THIS with:
    var searchEngine = new SearchEngine();
    await searchEngine.IndexDocumentAsync(filePath);
}
```

#### **3. Auto-complete** (Line ~514)
```csharp
private List<string> GetAutoCompleteSuggestions(string query)
{
    // REPLACE THIS with:
    var searchEngine = new SearchEngine();
    return searchEngine.GetAutoCompleteSuggestions(query);
}
```

### **4. Initialize Search Engine** (Line ~46)
```csharp
public MainForm()
{
    InitializeComponent();
    SetupAutoComplete();
    searchEngine = new SearchEngine(); // Uncomment and initialize
}
```

## 🗂️ Database/Index Design Suggestions

Consider storing:

```sql
Documents Table:
- DocumentId, Title, FilePath, FileType, FileSize, LastModified, Content

SearchIndex Table:
- DocumentId, Word, Frequency, Position

Suggestions Table:
- Query, Frequency, LastUsed
```

## 🚀 Testing Your Integration

1. **Replace the mock methods** with your implementations
2. **Test with real files** - upload PDFs, Word docs, etc.
3. **Verify search results** appear correctly in the grid
4. **Check auto-complete** suggestions work
5. **Test error handling** for unsupported files

## 🎯 Success Criteria

When integrated properly:
- ✅ Upload button indexes real documents
- ✅ Search returns relevant results from your index  
- ✅ Auto-complete suggests real terms from documents
- ✅ Document viewer can display actual content
- ✅ Filtering and sorting work with real data

## 📝 Architecture Overview

```
GUI Layer (✅ COMPLETE)
    ↓
Search Engine Interface (🔧 YOU BUILD THIS)
    ↓
Document Parsers (🔧 YOU BUILD THIS)
    ↓
Search Index/Database (🔧 YOU BUILD THIS)
```

## 🆘 Need Help?

The GUI is fully functional with mock data. You can:
1. Run it now to see exactly how it should work
2. Focus entirely on building the backend components
3. Test your backend by replacing one method at a time

**The hard UI work is done - you just need to build the search engine!** 🎉
