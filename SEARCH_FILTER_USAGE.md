# How to Use Search & Filter Feature

## 🎯 Quick Start Guide

### **Step 1: Navigate to Events Page**

**For Volunteers:**
- Login → Click "Events" or "Browse Events"

**For Superusers:**
- Login → Click "Events"

**For NGOs:**
- Search/filter not available (you only see your own events)

---

### **Step 2: Use the Search & Filter Panel**

You'll see a card at the top with two input fields:

```
┌─────────────────────────────────────────────────────────────────┐
│  Search by NGO Name          Filter by Category      [Search]   │
│  [Enter NGO name...]         [All Categories ▼]      [Reset]    │
└─────────────────────────────────────────────────────────────────┘
```

---

## 🔍 Search Options

### **Option 1: Search by NGO Name**

**What it does:** Finds events created by specific organizations

**How to use:**
1. Type NGO name in the search box (e.g., "Community Helpers")
2. Click "Search" button
3. See only events from matching NGOs

**Examples:**
- Search: `"Red Cross"` → Shows all Red Cross events
- Search: `"Community"` → Shows events from any NGO with "Community" in name
- Search: `"Help"` → Shows events from "Helping Hands", "Community Helpers", etc.

**Tips:**
- ✅ Partial matches work (e.g., "Help" finds "Helping Hands")
- ✅ Case doesn't matter ("red cross" = "Red Cross")
- ✅ Spaces are included in search

---

### **Option 2: Filter by Category**

**What it does:** Shows only events in a specific category

**How to use:**
1. Click the category dropdown
2. Select a category (e.g., "Education", "Healthcare")
3. Click "Search" button
4. See only events in that category

**Available Categories:**
The dropdown shows all categories that exist in the database, such as:
- Education
- Healthcare
- Environment
- Community Service
- Animal Welfare
- Disaster Relief
- Youth Development
- Senior Care
- Food Distribution
- etc.

**Tips:**
- ✅ Select "All Categories" to clear the filter
- ✅ Categories are sorted alphabetically
- ✅ Only categories with active events appear

---

### **Option 3: Combine Both Filters**

**What it does:** Finds events matching BOTH criteria

**How to use:**
1. Enter NGO name in search box
2. Select category from dropdown
3. Click "Search" button
4. See events matching both filters

**Example:**
```
Search: "Red Cross"
Category: "Healthcare"
Result: Only healthcare events by Red Cross organizations
```

---

## 🔄 Reset Filters

**How to clear all filters:**
1. Click the "Reset" button
2. Returns to full event list
3. All filters cleared

**Alternative:**
- Select "All Categories" and leave search box empty
- Click "Search"

---

## 📊 Understanding Results

### **Results Count Display**

When filters are active, you'll see:

```
5 event(s) found matching NGO: Community Helpers in category: Education
```

This tells you:
- **5 events** match your criteria
- Searching for NGO: **"Community Helpers"**
- Filtered by category: **"Education"**

### **No Results**

If no events match, you'll see:

```
No events available
There are currently no events available for volunteering.
```

**What to do:**
- Try different search terms
- Select different category
- Click "Reset" to see all events

---

## 💡 Use Case Examples

### **Example 1: Find All Education Events**
```
1. Leave search box empty
2. Select "Education" from dropdown
3. Click "Search"
→ Result: All education events from any NGO
```

### **Example 2: Find Specific NGO's Events**
```
1. Type "Community Helpers" in search box
2. Leave category as "All Categories"
3. Click "Search"
→ Result: All events by Community Helpers Foundation
```

### **Example 3: Find Healthcare Events by Red Cross**
```
1. Type "Red Cross" in search box
2. Select "Healthcare" from dropdown
3. Click "Search"
→ Result: Only healthcare events by Red Cross
```

### **Example 4: Browse All Events**
```
1. Click "Reset" button
→ Result: All available events shown
```

---

## 🎨 Visual Guide

### **Before Filtering:**
```
Events Page
├── Search & Filter Panel
│   ├── Search: [empty]
│   └── Category: [All Categories]
├── Event 1 (Community Helpers - Education)
├── Event 2 (Red Cross - Healthcare)
├── Event 3 (Animal Shelter - Animal Welfare)
├── Event 4 (Community Helpers - Environment)
└── Event 5 (Food Bank - Food Distribution)

Total: 5 events shown
```

### **After Filtering (Search: "Community", Category: "Education"):**
```
Events Page
├── Search & Filter Panel
│   ├── Search: [Community]
│   └── Category: [Education]
├── Results: "1 event(s) found matching NGO: Community in category: Education"
└── Event 1 (Community Helpers - Education)

Total: 1 event shown
```

---

## ⚡ Quick Tips

### **For Best Results:**
1. ✅ Start broad, then narrow down
2. ✅ Try partial NGO names if you don't know the full name
3. ✅ Use Reset button to start over
4. ✅ Filters persist in URL (you can bookmark filtered results)

### **Common Mistakes:**
1. ❌ Typing full NGO name exactly (partial works better)
2. ❌ Forgetting to click "Search" after changing filters
3. ❌ Not clicking "Reset" when you want to see all events

---

## 🔧 Technical Notes

### **How Search Works:**
- **Partial matching**: "Help" matches "Helping Hands"
- **Case-insensitive**: "red cross" = "Red Cross"
- **Database-level filtering**: Fast and efficient
- **URL parameters**: Filters saved in URL for sharing

### **URL Format:**
```
/Events/Index?searchNGO=Community&category=Education
```

You can:
- Bookmark filtered results
- Share filtered URL with others
- Use browser back/forward buttons

---

## 📱 Mobile Usage

The search and filter panel is **fully responsive**:

**Desktop:**
- Search box, dropdown, and buttons in one row

**Tablet:**
- Search and dropdown side-by-side
- Buttons below

**Mobile:**
- All fields stacked vertically
- Full-width inputs for easy typing

---

## ❓ FAQ

### **Q: Why don't I see the search panel?**
**A:** You're logged in as an NGO. NGOs only see their own events, so search isn't needed.

### **Q: Can I search by event title?**
**A:** Not yet. Currently only NGO name search is supported. Event title search may be added in future.

### **Q: Can I filter by date?**
**A:** Not yet. Date filtering may be added in future updates.

### **Q: Why is my category not in the dropdown?**
**A:** The dropdown only shows categories that have active events. Create an event in that category first.

### **Q: Can I search multiple NGOs at once?**
**A:** Not yet. You can only search one NGO name at a time.

### **Q: Do filters work with the Apply button?**
**A:** Yes! Filtered events still show the "Apply" button if you're a volunteer.

---

## 🎓 Practice Exercises

### **Exercise 1: Basic Search**
1. Go to Events page
2. Search for any NGO name
3. Observe the results

### **Exercise 2: Category Filter**
1. Select a category from dropdown
2. Click Search
3. Verify all events are in that category

### **Exercise 3: Combined Filter**
1. Enter NGO name
2. Select category
3. Click Search
4. Verify results match both criteria

### **Exercise 4: Reset**
1. Apply any filters
2. Click Reset
3. Verify all events are shown again

---

## ✅ Summary

**Search & Filter allows you to:**
- 🔍 Find events by NGO name
- 📂 Filter events by category
- 🎯 Combine both for precise results
- 🔄 Reset to see all events
- 📊 See count of matching events

**Perfect for:**
- Finding events from your favorite NGOs
- Browsing events in specific categories
- Quickly narrowing down hundreds of events
- Discovering new volunteer opportunities

---

**Happy Volunteering! 🎉**
