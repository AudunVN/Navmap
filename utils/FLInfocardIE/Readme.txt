FL Infocard Importer/Exporter by cannon
26 July 2011

This program imports and exports FLDev style infocard files into Freelancer string
table and infocard resource dlls. It checks for dll and infocard file correctness
and will complain if faults are found. Where possible it will attempt to automatically
fix errors.

Usage:
* Load: Set your path to Freelancer and press 'Load'. This loads all dll strings into memory.
* Import: Import your infocard file. This file should be in UTF-8 format.
* You may need to fix errors. Repeat the load and import steps until all errors are corrected.
* Save: Press the save button to save the changes. All infocard DLLs are rewritten.

You can export all infocard information using the 'export' button.

Format for imported/exported infocards:

* A NAME infocard should have the format
<ids number>
NAME
<text>

* A infocard should have the format
<ids number>
INFOCARD
<text>

* Lines starting with ; are ignored
* If <text> includes a "\n" then this will be replaced with a newline character 0x0A in the dll. This allows newlines to be for news and NPC names.

Notes:
* Empty infocards without appropriate xml tags are treated as illegal.
* Infocards that do not start and end with < > characters are treated as illegal. 
* News items should be in NAME sections.
* Conflicting NAME and INFOCARD IDS numbers are not permitted. These are not in
vanilla and shouldn't be used by mods either.