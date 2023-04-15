# Magic Episode Sort

**v2.1.0 - The Great SQLite Migration**

*The purpose of this software is not to condone piracy. But if you're gonna do it, you might as well stay organized.*

Magic Episode Sort searches for and finds episode files using common markers in the filename (e.g. S01E05, 1x01, etc). It then sorts the episode files by series title and season number. 
Simply:

1. Set up your source directories
2. Set up your output directory
3. Click Sort

## 1.0 Setup

To download the latest installer head over to the releases section and get the latest release. Magic Episode Sort is currently only supported on Windows.

## 2.0 Preferences

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