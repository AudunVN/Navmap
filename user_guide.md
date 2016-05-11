# User guide
## Basics
>work in progress

## In-game commands
Allowing the map script to read your DSAce.log file (usually located in `My Documents/My Games/Freelancer`) locally enables the use of various in-game commands, such as `/map` and `/pos` hopefully followed by TLAGSNET integration at some point down the line.

No data is uploaded or shared anywhere when you allow the map script to access your DSAce.log file; everything is processed locally in your web browser, which sidesteps any security issues.
### /map
The `/map <input>` (e.g. `/map fort bush`) command allows you to use the online map in another window - such as on another screen while you have Discovery open in fullscreen - when used along with the `<input>` options listed below.
- `/map <base>` opens the system map for that base and highlights it on the map.
- `/map <system>` opens the map for that system.
- `/map up/down` scrolls the map up/down.
- `/map removehighlights` removes any currently active highlight animations.
- `/map clearships` removes any ship positions added using the `/pos` command as described below.
- `/map sirius` opens the universe map.

### /pos
<img src="https://cloud.githubusercontent.com/assets/2485340/15176059/9fb9ca32-1769-11e6-96d8-4695a61581fc.jpg" height="250">

The `/pos in <system>` (e.g. `/pos in new york`) command allows you to see your position in the system you're currently in. Getting the orientation shown on the map to be correct will usually require using the command twice without changing your direction of travel in-game, as this is used to calculate where your ship is headed - the orientation output with the /pos command is unfortunately not suited for this application.
