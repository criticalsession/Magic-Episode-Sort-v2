# Magic Episode Sort

**v2.0.0 - The Great SQLite Migration**

*The purpose of this software is not to condone piracy. But if you're gonna do it, you might as well stay organized.*

Magic Episode Sort searches for and finds episode files using common markers in the filename (e.g. S01E05, 1x01, etc). It then sorts the episode files by series title and season number. 
Simply:

1. Set up your source directories
2. Set up your output directory
3. Click Sort

If you'd like to get a bit more out of the software, or need some assistance on setting it up, this guide is for you.

## 1.0 Setup

To download the latest installer head over to the releases section and get the latest release. Magic Episode Sort is currently only supported on Windows.

## 2.0 Using Magic Episode Sort

After installing, run Magic Episode Sort and you'll be automatically directed to the Preferences window for first-time setup.

### 2.1 Preferences

#### 2.1.1 General Preferences

Access the Preferences window by going to "Edit > Preferences" in the main menu. This window will be opened automatically when you run Magic Episode Sort 
for the first time.

- "Search sub-folders": When checked, allows Magic Episode Sort to search sub-folders inside the source directories. Default: ON.

- "Recursively search sub-folders": When checked, Magic Episode Sort will follow all sub-folders to the end. Default: ON.

- "Ask to confirm new series titles": When new series titles are found, Magic Episode Sort will ask you to confirm that what it extracted is the correct series title.
Files often omit brackets and apostrophes in the series title section, so this is where you would update those mistakes. Default: ON.

- "Attempt to get series titles from TVMaze API": After getting the series title from the filename, Magic Episode Sort will query the TVMaze API in an attempt to 
get a more accurate title. You will still be asked to confirm. Default: OFF.

- "Open output directory in explorer": After sorting, Magic Episode Sort will open the assigned output directory in explorer. Default: OFF.

The best way to set up Magic Episode Sort is to simply turn on all available options. Turn the TVMaze API option off if you're having connectivity issues.

#### 2.1.2 Sources

Sources are directories that Magic Episode Sort will search inside for episode files. If "Search sub-folders" is turned on, it will search all sub-folders 
inside the source directories. If "Recursively search sub-folders" is turned on, it will also search all sub-folders inside those sub-folders. 

You can manage your source directories either from the Preferences window by clicking "Edit Sources...", or through the main menu "Edit > Sources...". This will open 
a new window with a list of your currently set directories. Click the "Select Directory" button to find a directory to add, then click "Add Source" to add it to the list. 
If you'd like to remove a source directory, right click on one in the list and select "Delete Source".

Magic Episode Sort will always skip the output directory when searching for new files (even if it's inside one of the source directories.)

#### 2.1.3 Output Directory

The Output Directory is where all the sorted files will go. To set it up, open the Preferences window, and click the "..." next to "Output Directory". After sorting, the episode 
files will be moved to this directory inside their own series and season folders.

For example, if your Output Directory is `C:/Downloads/Sorted`, and Magic Episode Sort found the file `a.christmas.carol.s05e02.mp4`, after sorting this will be the resulting 
folder structure: `C:/Downloads/Sorted/A Christmas Carol/Season 05/a.christmas.carol.s05e02.mp4`.

### 2.2 Custom Series Titles

Custom Series Titles are manually entered series titles that amend Magic Episode Sort's automatically extracted titles.

#### 2.2.1 After a Search

After completing a search, Magic Episode Sort extracts the series titles from the filename. For example, a video file `a.christmas.carol.s05e02.mp4` will result in the series title being 
"A Christmas Carol". If the "Attempt to get series titles from TVMaze API" option is turned on, it will then query the TVMaze API in an attempt to confirm that "A Christmas Carol" is in face 
the correct title or if it's missing some key part - such as an apostrophe or brackets. Because this doesn't always return the correct result, and if the option "Ask to confirm new series titles" 
is turned on, Magic Episode Sort will then prompt you to confirm any **new** series titles (titles found in previous runs are not shown here).

To edit previously-set custom titles, access "Edit > Custom Series Titles..." from the main menu.

#### 2.2.2 Editing Titles

This section applies to the custom titles window, whether it's opened automatically after a search or from the main menu.

... readme in progress ...