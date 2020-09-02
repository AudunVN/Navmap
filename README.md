# Navmap<img height="40" align="left" src="https://github.com/AudunVN/Navmap/blob/gh-pages/favicon.png">
A browser-based map viewer for the Freelancer mod Discovery. This also works for vanilla Freelancer, but there is currently no public release of the map available for that (yet). Extra usage instructions for in-game map control may be found [here](https://github.com/AudunVN/Navmap/blob/gh-pages/user_guide.md).

A complete change and issue log from before this project was moved to a GitHub repo may be found in  [this DiscoveryGC forum thread](http://discoverygc.com/forums/showthread.php?tid=132266&pid=1700007#pid1700007).

## Updating instructions
 - Run update.py in the root directory of this repo, and follow the instructions
 - Check if the OORP systems array in index.html needs updating
     - This thing really needs a per-version config file.
 
Regarding texture data: These are extracted using [UTF Image Exporter](https://github.com/AudunVN/Navmap/tree/gh-pages/utils/UTFImageExporter), then bulk converted to from txm to jpg using IrFanView, and afterwards recursively and renamed counting up from 01.jpg using Metamorphose2 to ensure that there's at least one texture available from each .txm file (namely, 01.jpg, since they may be stored using any filename inside the .txm file and the navmap has no idea what they might be named in advance, so it simply expects there to be a file named "01.jpg" inside the folder for each txm file).

Icons and such are converted from TGA after being exported using ImageMagick or similar: ```mogrify -flip -path png -format png *.tga```