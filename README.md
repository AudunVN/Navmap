# Navmap<img height="40" align="left" src="https://github.com/AudunVN/Navmap/blob/gh-pages/favicon.png">
A browser-based map viewer for the Freelancer mod Discovery. This also works for vanilla Freelancer, but there is currently no public release of the map available for that (yet). Extra usage instructions for in-game map control may be found [here](https://github.com/AudunVN/Navmap/blob/gh-pages/user_guide.md).

A complete change and issue log from before this project was moved to a GitHub repo may be found in  [this DiscoveryGC forum thread](http://discoverygc.com/forums/showthread.php?tid=132266&pid=1700007#pid1700007).

## Updating instructions
 - Create new data folder in the root directory of this repo (or somewhere else accessible, these are the v4XXXX folders)
 - Generate infocards.txt using FLInfocardIE, located in the /utils folder.
     - Open the program, point it at your FL directory, click "Load", then "Export" and select the folder from step 1 as your destination directory
     - replace "\r\n\r\n" with "\r\n"
     - replace "\r\nNAME\r\n" with "\r\n"
     - replace "\r\nINFOCARD\r\n" with "\r\n"
     - remove "\r\n" from start of file
 - Copy special_systems.txt into the folder from step 1 from the previous version
     - This file defines the house of each system using the prefix (IW, BW, etc.) in front of their full name. This is the closest Discovery has to a house definition file, unfortunately, and it's not kept up-to-date either - it frequently requires manual modification. 
 - Copy DATA/MISSIONS/mbases.ini into the folder from step 1
 - Copy DATA/INTERFACE/infocardmap.ini into the folder from step 1
 - Copy DATA/SOLAR/solararch.ini into the folder from step 1
 - Copy the contents of the IONCROSS folder from the Freelancer root directory into the folder from step 1
 - Create a folder named "solar" in the folder from step 1
     - Copy the DATA/SOLAR/ASTEROIDS folder (minus MODELS, if you wish - the map doesn't use it) into this folder
 - Create a folder named "universe" in the folder from step 1
     - Copy the DATA/UNIVERSE/SYSTEMS folder (minus BASE_INTERIORS, if you wish - the map doesn't use it) into this folder
	 - Copy DATA/UNIVERSE/universe.ini into this folder
	 - Copy DATA/UNIVERSE/multiverse.ini into this folder
	 - Run FL_Path_Generator.jar on your Freelancer installation, for good measure
	 - Copy DATA/UNIVERSE/shortest_illegal_path.ini into this folder
	 - Copy DATA/UNIVERSE/shortest_legal_path.ini into this folder
	 - Copy DATA/UNIVERSE/systems_shortest_path.ini into this folder
 - Depending on your web server and/or file system, you may need to lowercase absolutely everything; this can be done using the Metamorphose2 tool in the /utils folder on Windows, or your favorite command line tool or whatnot.
 - Check if the OORP systems array in index.html needs updating
     - This thing really needs a per-version config file.
 - Update the data folder path at the top of index.html to point the map script at your new data folder
 
Regarding texture data: These are extracted using [UTF Image Exporter](https://github.com/AudunVN/Navmap/tree/gh-pages/utils/UTFImageExporter), then bulk converted to from txm to jpg using IrFanView, and afterwards recursively and renamed counting up from 01.jpg using Metamorphose2 to ensure that there's at least one texture available from each .txm file (namely, 01.jpg, since they may be stored using any filename inside the .txm file and the navmap has no idea what they might be named in advance, so it simply expects there to be a file named "01.jpg" inside the folder for each txm file).
