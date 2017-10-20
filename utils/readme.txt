Updating for a new version:
1. generate infocards.txt
   - replace "\r\n\r\n" with "\r\n"
   - replace "\r\nNAME\r\n" with "\r\n"
   - replace "\r\nINFOCARD\r\n" with "\r\n"
   - remove \r\n from start of file
2.1 run path gen
2. copy /UNIVERSE
3. copy /MISSIONS/mbases.ini
4. copy /IONCROSS contents
5. copy /SOLAR/ASTEROIDS
7. copy /DATA/INTERFACE/infocardmap.ini
7. copy (and update) systems_special.txt from previous version
8. copy solararch.ini
6. lowercase absolutely everything (Métamorphose 2 recommended)
7. verify whether oorpArray in index.html requires updating (check for new dead-end systems)
9. get planet textures using UTF Image Exporter
10. batch convert from txm to jpg using IrFanView recursively 
11. rename files to ##.jpg where the number ## is a file counter for each sub-directory using metamph. 2
12. lowercase everything