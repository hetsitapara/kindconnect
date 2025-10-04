# Search & Filter Feature - Events Section

## 🎯 Overview

Added search and filter functionality to the Events section, allowing users to find events by NGO name and filter by category.

---

## ✨ Features Added

### 1. **Search by NGO Name**
- Text input field for searching events by organization name
- Case-insensitive partial matching (e.g., "Community" matches "Community Helpers Foundation")
- Real-time search on form submission

### 2. **Filter by Category**
- Dropdown list populated with all unique event categories from the database
- Dynamic category list (automatically updates as new categories are added)
- "All Categories" option to clear the filter

### 3. **Combined Search & Filter**
- Both filters can be used simultaneously
- Results show events matching BOTH criteria (AND logic)

### 4. **Results Display**
- Shows count of matching events
- Displays active search/filter criteria
- Clear visual feedback

### 5. **Reset Functionality**
- "Reset" button clears all filters and returns to full event list

---

## 🔧 Technical Implementation

### **Controller Changes** (`EventsController.cs`)

#### **Index Action** (Updated)
```csharp
public async Task<IActionResult> Index(string searchNGO, string category)
```

**New Parameters:**
- `searchNGO` - Search term for NGO name
- `category` - Selected category filter

**Logic:**
1. Base query with role-based filtering (existing)
2. Apply NGO name search filter (if provided)
3. Apply category filter (if provided)
4. Get unique categories for dropdown
5. Pass search/filter values to view via ViewBag

**Code:**
```csharp
// Apply search filter by NGO name
if (!string.IsNullOrEmpty(searchNGO))
{
    query = query.Where(e => e.NGO.Name.Contains(searchNGO));
}

// Apply category filter
if (!string.IsNullOrEmpty(category))
{
    query = query.Where(e => e.Category == category);
}

// Get all unique categories for dropdown
var categories = await _context.Events
    .Where(e => e.IsActive)
    .Select(e => e.Category)
    .Distinct()
    .OrderBy(c => c)
    .ToListAsync();

ViewBag.Categories = categories;
ViewBag.SearchNGO = searchNGO;
ViewBag.SelectedCategory = category;
```

#### **Browse Action** (Updated)
```csharp
public async Task<IActionResult> Browse(string searchNGO, string category)
```

Same functionality as Index but for Volunteer role specifically.

---

### **View Changes** (`Views/Events/Index.cshtml`)

#### **Search & Filter UI**
Added a card with search form (hidden for NGO role):

```html
<div class="card mb-4">
    <div class="card-body">
        <form method="get" asp-action="...">
            <div class="row g-3">
                <!-- Search Input -->
                <div class="col-md-5">
                    <label for="searchNGO" class="form-label">Search by NGO Name</label>
                    <input type="text" 
                           class="form-control" 
                           id="searchNGO" 
                           name="searchNGO" 
                           placeholder="Enter NGO name..." 
                           value="@ViewBag.SearchNGO">
                </div>
                
                <!-- Category Dropdown -->
                <div class="col-md-4">
                    <label for="category" class="form-label">Filter by Category</label>
                    <select class="form-select" id="category" name="category">
                        <option value="">All Categories</option>
                        @foreach (var cat in ViewBag.Categories as List<string>)
                        {
                            <option value="@cat" selected="@(cat == ViewBag.SelectedCategory)">
                                @cat
                            </option>
                        }
                    </select>
                </div>
                
                <!-- Buttons -->
                <div class="col-md-3 d-flex align-items-end">
                    <button type="submit" class="btn btn-primary me-2">
                        <i class="fas fa-search"></i> Search
                    </button>
                    <a asp-action="..." class="btn btn-outline-secondary">
                        <i class="fas fa-redo"></i> Reset
                    </a>
                </div>
            </div>
        </form>
    </div>
</div>
```

#### **Results Count Display**
Shows filtered results information:

```html
@if (!string.IsNullOrEmpty(ViewBag.SearchNGO) || !string.IsNullOrEmpty(ViewBag.SelectedCategory))
{
    <div class="mb-3">
        <p class="text-muted">
            <strong>@Model.Count()</strong> event(s) found
            @if (!string.IsNullOrEmpty(ViewBag.SearchNGO))
            {
                <span> matching NGO: <strong>@ViewBag.SearchNGO</strong></span>
            }
            @if (!string.IsNullOrEmpty(ViewBag.SelectedCategory))
            {
                <span> in category: <strong>@ViewBag.SelectedCategory</strong></span>
            }
        </p>
    </div>
}
```

---

## 👥 User Experience

### **For Volunteers**
1. Navigate to Events → Browse
2. See search bar and category dropdown at top
3. Enter NGO name or select category (or both)
4. Click "Search" to filter results
5. See count of matching events
6. Click "Reset" to clear filters

### **For Superusers**
1. Navigate to Events → Index
2. Same search/filter functionality as volunteers
3. Can see all events (including private/inactive)

### **For NGOs**
- Search/filter section is **hidden**
- NGOs only see their own events (no need for search)

---

## 📊 Example Use Cases

### **Use Case 1: Find Events by Specific NGO**
```
User Action: Enter "Community Helpers" in search box
Result: Shows only events created by "Community Helpers Foundation"
```

### **Use Case 2: Filter by Category**
```
User Action: Select "Education" from category dropdown
Result: Shows only events in "Education" category
```

### **Use Case 3: Combined Search**
```
User Action: 
  - Search: "Red Cross"
  - Category: "Healthcare"
Result: Shows only healthcare events by Red Cross organizations
```

### **Use Case 4: No Results**
```
User Action: Search for non-existent NGO
Result: Shows "No events available" message
```

---

## 🎨 UI/UX Features

### **Visual Design**
- Clean card-based layout
- Bootstrap 5 styling for consistency
- Font Awesome icons for buttons
- Responsive design (mobile-friendly)

### **Form Behavior**
- GET method (preserves search in URL)
- Maintains search values after submission
- Dropdown shows selected category
- Graceful handling of empty results

### **Accessibility**
- Proper form labels
- Semantic HTML
- Keyboard navigation support

---

## 🔍 Technical Details

### **Query Performance**
- Uses LINQ `Contains()` for partial string matching
- Filters applied at database level (not in-memory)
- Eager loading with `.Include(e => e.NGO)` to avoid N+1 queries
- Distinct categories fetched efficiently

### **Database Queries**
```sql
-- Search by NGO name
SELECT * FROM Events 
WHERE NGO.Name LIKE '%searchTerm%'

-- Filter by category
SELECT * FROM Events 
WHERE Category = 'selectedCategory'

-- Get unique categories
SELECT DISTINCT Category FROM Events 
WHERE IsActive = 1 
ORDER BY Category
```

### **Security**
- No SQL injection risk (Entity Framework parameterizes queries)
- Role-based authorization maintained
- Input sanitization handled by ASP.NET Core

---

## 🧪 Testing Scenarios

### **Test 1: Search Functionality**
1. Create events with different NGO names
2. Search for partial NGO name
3. Verify only matching events appear

### **Test 2: Category Filter**
1. Create events in different categories
2. Select category from dropdown
3. Verify only events in that category appear

### **Test 3: Combined Filters**
1. Apply both search and category filter
2. Verify results match both criteria

### **Test 4: Empty Results**
1. Search for non-existent NGO
2. Verify "No events available" message displays

### **Test 5: Reset Button**
1. Apply filters
2. Click Reset button
3. Verify all filters cleared and full list shown

### **Test 6: URL Persistence**
1. Apply filters
2. Copy URL
3. Paste in new tab
4. Verify filters persist

---

## 📝 Future Enhancements (Optional)

### **Possible Additions:**
1. **Date Range Filter** - Filter events by date range
2. **Location Filter** - Filter by city/state
3. **Capacity Filter** - Show only events with available spots
4. **Sort Options** - Sort by date, capacity, NGO name
5. **Advanced Search** - Multiple NGO names, keyword search in description
6. **Save Filters** - Remember user's preferred filters
7. **Export Results** - Export filtered events to PDF/CSV
8. **Auto-complete** - NGO name suggestions as user types

---

## 🐛 Known Limitations

1. **Case Sensitivity**: Search is case-insensitive but accent-sensitive
2. **Partial Match Only**: No fuzzy matching or typo tolerance
3. **No Pagination**: All results shown on one page (may be slow with many events)
4. **Static Categories**: Categories not predefined (depends on existing data)

---

## 📚 Files Modified

1. **`Controllers/EventsController.cs`**
   - Updated `Index()` action
   - Updated `Browse()` action

2. **`Views/Events/Index.cshtml`**
   - Added search/filter form
   - Added results count display

---

## ✅ Summary

The search and filter feature enhances the Events section by allowing users to:
- **Search** events by NGO name
- **Filter** events by category
- **Combine** both filters for precise results
- **Reset** filters easily
- **See** clear feedback on filtered results

This improves user experience significantly, especially as the number of events grows.
