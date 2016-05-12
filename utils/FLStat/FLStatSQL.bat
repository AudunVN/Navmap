:: Batch file to start sqlite3 with FLStat.sql, with headers in column mode
:: (column widths may need to be adjusted for your language and/or mod).
:: DOSKEY macros are provided to emulate FLStat itself (with no blank items
:: and some headers are different to avoid excessive spacing).
:: Note: some prices have different values compared to FL/FLStat.
::	 E.g. best buy H-Fuel = 300 * 0.08 = 24 here, but 23 in FL & FLStat.
:: Similarly, some per second values are smaller, due to truncated refire.
@echo off
SetLocal

:: EXIST does not work with a linked directory (as "My Documents" is from
:: Vista onwards), but FOR does.
set SQLFile=
for %%f in ("%USERPROFILE%\My Documents\FLStat.sql" FLStat.sql %1) do (
  if not [%%~af] == [] set SQLFile=%%f
)
if [%SQLFile%] == [] (
  echo Please specify the location of the FLStat SQL database.
  goto :eof
)
if not [%1] == [] if not [%SQLFile%] == [%1] (
  echo "%~1" does not exist.
  goto :eof
)

:: Multiple commands generate multiple prompts, so each command turns off the
:: prompt, then uses this to turn it back on.
set SQLPROMPT=$T.prompt 'FLStat$G '


:: Bases [sql] - list bases.
:: E.g. Bases AND System = 'Texas' ORDER BY Base
doskey /exename=sqlite3.exe Bases=.prompt ''$T^
.width 30 43 29 16 10 16 -7 -7$T^
.print^
 "                               "^
 "                                            "^
 "                              "^
 "                 "^
 "  System   "^
 "                 "^
 "Base    "^
 "Base"$T^
SELECT^
 base.name COLLATE NOCASE Base,^
 CASE WHEN group_id = faction_id OR faction_id = 0 THEN faction.name^
      ELSE printf('%%s%%s%%s*',^
		  faction.name,^
		  CASE WHEN group_id = 0 THEN '' ELSE ' - ' END,^
		  (SELECT name FROM faction WHERE id = faction_id)) END^
   COLLATE NOCASE 'Owner - Faction*',^
 system.name COLLATE NOCASE System,^
 territory Territory,^
 system.nickname ' Nickname',^
 base.nickname 'Base Nickname',^
 base.ids_name 'Name ID',^
 base.ids_info 'Info ID'^
FROM base, faction, system ^
WHERE group_id = faction.id^
  AND system_id = system.id^
  AND substr(base.name, 1, 1) != '['^
  AND (base.ids_info != 0 OR (base.ids_name $G$G 16) $G= 7) ^
$*;%SQLPROMPT%


:: Goods 'Base' [sql] - list goods on BASE.
:: E.g. Goods 'Planet Manhattan' AND Type = 'gun' ORDER BY Good
doskey /exename=sqlite3.exe Goods=.prompt ''$T^
.width 33 12 -9 5 -5 -5 -9 33$T^
.print^
 "                                  "^
 "             "^
 "          "^
 "Base  "^
 "Level "^
 "Rep.  "^
 "  Price"$T^
SELECT^
 CASE WHEN equip_cat.id != 10 THEN equipment.name^
      ELSE (SELECT name FROM equipment equip, ship_package ^
	    WHERE equip_id = equipment.id AND ship_id = equip.id) END^
   COLLATE NOCASE Good,^
 CASE WHEN equip_cat.id = 10 THEN (SELECT name FROM equip_cat WHERE id = 8)^
      WHEN equip_cat.id != 2 THEN equip_cat.name^
      ELSE (SELECT gun_cat.name FROM gun_cat, gun^
	    WHERE equip_id = equipment.id AND gun_cat.id = gun.category) END^
   Type,^
 CAST(price * price_modifier AS INTEGER) Price,^
 yesno.name Sells,^
 level_reqd 'Req''d',^
 printf('%%5.2f', rep_reqd) 'Req''d',^
 printf('%%8.4f', price_modifier) Modifier,^
 equipment.nickname Nickname ^
FROM base_equip, equipment, equip_cat, yesno, base ^
WHERE equip_id = equipment.id^
  AND equip_cat.id = category^
  AND yesno.id = (min_inv != 0 AND stock != 0)^
  AND base_id = base.id^
  AND base.name = $*;%SQLPROMPT%


:: Factions [sql] - list factions.
:: E.g. Factions AND Nickname LIKE 'gd%' ORDER BY Faction
doskey /exename=sqlite3.exe Factions=.prompt ''$T^
.width 28 16 -11 -7 -7 -7 0 -7 -7$T^
.print^
 "                             "^
 "               "^
 "   Object   "^
 "Mission "^
 "Mission "^
 "Mission "$T^
SELECT^
 name Faction,^
 shortname 'Short Name',^
 printf('%%11.4f', repchg_obj_destruction) Destruction,^
 printf('%%7.4f', repchg_mission_success) Success,^
 printf('%%7.4f', repchg_mission_failure) Failure,^
 printf('%%7.4f', repchg_mission_abort) 'Abort ',^
 nickname Nickname,^
 ids_name 'Name ID',^
 ids_info 'Info ID' ^
FROM faction ^
WHERE ids_info $G 1 ^
$*;%SQLPROMPT%


:: Faction 'Faction' - find nickname of FACTION (long or short name).
:: E.g. Faction 'Liberty Navy'
doskey /exename=sqlite3.exe Faction=.prompt ''$T^
.header off$T^
SELECT nickname FROM faction ^
WHERE name = $* OR shortname = $*;$T^
.header on%SQLPROMPT%


:: Empathy 'nickname' [sql] - list empathies of a faction.
:: E.g. Empathy 'li_n_grp' ORDER BY Faction
doskey /exename=sqlite3.exe Empathy=.prompt ''$T^
.width 29 -10 -7 0$T^
SELECT^
 CASE WHEN faction_id = faction_id2 THEN^
   printf('%%s*', faction2.name) ELSE faction2.name END COLLATE NOCASE Faction,^
 printf('%%10.2f', rep) Reputation,^
 printf('%%7.2f', empathy) Empathy,^
 faction2.nickname Nickname ^
FROM faction_rep, faction, faction faction2 ^
WHERE faction_id = faction.id^
  AND faction2.id = faction_id2^
  AND faction2.ids_info != 1^
  AND faction.nickname = $*;%SQLPROMPT%


:: Bribe 'faction' - list bribes for FACTION (long, short or nickname).
:: E.g. Bribe 'Corsairs'
doskey /exename=sqlite3.exe Bribe=.prompt ''$T^
.width 30 43 29 -13$T^
SELECT^
 base.name COLLATE NOCASE 'Base Selling Rep Hack',^
 CASE WHEN group_id = base.faction_id OR base.faction_id = 0 THEN faction.name^
      ELSE printf('%%s%%s%%s*',^
		  faction.name,^
		  CASE WHEN group_id = 0 THEN '' ELSE ' - ' END,^
		  (SELECT name FROM faction WHERE id = base.faction_id)) END^
   COLLATE NOCASE 'Owner - Faction*',^
 system.name COLLATE NOCASE System,^
 printf('%%.2f', occurrence) 'Probability %%' ^
FROM rep_hack, base, system, faction ^
WHERE rep_hack.faction_id = (SELECT id FROM faction^
			     WHERE name = $* OR^
			      shortname = $* OR^
			       nickname = $*)^
  AND base_id = base.id^
  AND group_id = faction.id^
  AND system_id = system.id^
  AND (base.ids_info != 0 OR (base.ids_name $G$G 16) $G= 7) ^
ORDER BY occurrence DESC, 1;%SQLPROMPT%


:: Good 'good' [sql] - show bases containing GOOD.
:: E.g. Good 'Cannonball Missile' AND category = 2 ORDER BY Base
:: (use category to get the launcher, not the ammo; use 6 for mine droppers)
doskey /exename=sqlite3.exe Good=.prompt ''$T^
.width 33 43 29 -9 5 -5 -5 16 33$T^
.print^
 "                                  "^
 "                                            "^
 "                              "^
 "          "^
 "Base  "^
 "Level "^
 "Rep."$T^
SELECT^
 base.name COLLATE NOCASE Base,^
 CASE WHEN group_id = faction_id OR faction_id = 0 THEN faction.name^
      ELSE printf('%%s%%s%%s*',^
		  faction.name,^
		  CASE WHEN group_id = 0 THEN '' ELSE ' - ' END,^
		  (SELECT name FROM faction WHERE id = faction_id)) END^
   COLLATE NOCASE 'Owner - Faction*',^
 system.name COLLATE NOCASE System,^
 CAST(price * price_modifier AS INTEGER) Price,^
 yesno.name Sells,^
 level_reqd 'Req''d',^
 printf('%%5.2f', rep_reqd) 'Req''d',^
 base.nickname 'Base Nickname',^
 equipment.nickname 'Equipment Nickname' ^
FROM base_equip, base, faction, system, equipment, yesno ^
WHERE base_id = base.id^
  AND equip_id = equipment.id^
  AND group_id = faction.id^
  AND system_id = system.id^
  AND yesno.id = (min_inv != 0 AND stock != 0)^
  AND (base.ids_info != 0 OR (base.ids_name $G$G 16) $G= 7)^
  AND equipment.name = $*;%SQLPROMPT%


:: Ship 'ship' [sql] - show bases containing SHIP.
:: E.g. Ship 'Anubis' ORDER BY Base
:: Easier to have a separate query rather than making this work with Good
:: (especially when working with DOSKEY).
doskey /exename=sqlite3.exe Ship=.prompt ''$T^
.width 33 43 29 -9 5 -5 -5 16 33$T^
.print^
 "                                  "^
 "                                            "^
 "                              "^
 "          "^
 "Base  "^
 "Level "^
 "Rep."$T^
SELECT^
 base.name COLLATE NOCASE Base,^
 CASE WHEN group_id = faction_id OR faction_id = 0 THEN faction.name^
      ELSE printf('%%s%%s%%s*',^
		  faction.name,^
		  CASE WHEN group_id = 0 THEN '' ELSE ' - ' END,^
		  (SELECT name FROM faction WHERE id = faction_id)) END^
   COLLATE NOCASE 'Owner - Faction*',^
 system.name COLLATE NOCASE System,^
 CAST(equipment.price * price_modifier AS INTEGER) Price,^
 yesno.name Sells,^
 level_reqd 'Req''d',^
 printf('%%5.2f', rep_reqd) 'Req''d',^
 base.nickname 'Base Nickname',^
 equipment.nickname 'Equipment Nickname' ^
FROM base_equip, base, faction, system, equipment,^
     ship_package, equipment shipname, yesno ^
WHERE base_id = base.id^
  AND base_equip.equip_id = equipment.id^
  AND group_id = faction.id^
  AND system_id = system.id^
  AND yesno.id = (min_inv != 0 AND stock != 0)^
  AND equipment.id = ship_package.equip_id^
  AND ship_id = shipname.id^
  AND (base.ids_info != 0 OR (base.ids_name $G$G 16) $G= 7)^
  AND shipname.name = $*;%SQLPROMPT%


:: Commodities [sql] - list commodities.
:: E.g. Commodities ORDER BY Commodity
doskey /exename=sqlite3.exe Commodities=.prompt ''$T^
.width 40 -6 -9 -10 -6 -5 32 -7 -7$T^
.print^
 "                                         "^
 "       "^
 "  Best    "^
 "   Best    "^
 "Profit"$T^
SELECT^
 name COLLATE NOCASE Commodity,^
 price Price,^
 CAST(best_buy * price AS INTEGER) 'Buy Price',^
 CAST(best_sell * price AS INTEGER) 'Sell Price',^
 CAST(best_sell * price AS INTEGER) - CAST(best_buy * price AS INTEGER) Margin,^
 jump_dist Jumps,^
 nickname Nickname,^
 ids_name 'Name ID',^
 ids_info 'Info ID' ^
FROM commodity, equipment ^
WHERE equip_id = id AND (best_sell != 1 OR best_buy != 0) ^
$*;%SQLPROMPT%


:: Guns [sql] - list guns & turrets.
:: E.g. Guns ORDER BY Gun
doskey /exename=sqlite3.exe Guns=.prompt ''$T^
.width 27 -5 7 12 -7 -8 -10 -11 -6 -10 -12 -7 -10 -8 -10 -7 -7 -9 -7 8 27 -7 -7$T^
SELECT^
 equipment.name COLLATE NOCASE Gun,^
 CASE WHEN class = 0 THEN '' ELSE class END Class,^
 gun_cat.name Type,^
 gun_type.name Technology,^
 price Price,^
 printf('%%8.2f', hull_dmg) 'Hull Dmg',^
 printf('%%10.2f', shield_dmg) 'Shield Dmg',^
 printf('%%11.2f', power_usage) 'Power Usage',^
 printf('%%6.2f', refire_rate) Refire,^
 printf('%%10.2f', hull_dmg * refire_rate) 'Hull Dmg/s',^
 printf('%%12.2f', shield_dmg * refire_rate) 'Shield Dmg/s',^
 printf('%%7.2f', power_usage * refire_rate) 'Power/s',^
 printf('%%10.4f', CASE WHEN hull_dmg $G= shield_dmg^
			THEN hull_dmg ELSE shield_dmg END /^
		   CASE WHEN power_usage = 0 THEN 1 ELSE power_usage END)^
   Efficiency,^
 printf('%%8.2f', CASE WHEN hull_dmg $G= shield_dmg THEN hull_dmg^
		       ELSE shield_dmg END * refire_rate * 1000 /^
		       CASE WHEN price = 0 THEN 1 ELSE price END)^
   Value,^
 printf('%%10.2f', CASE WHEN hull_dmg $G= shield_dmg^
			THEN hull_dmg ELSE shield_dmg END /^
		   CASE WHEN power_usage = 0 THEN 1 ELSE power_usage END *^
		   CASE WHEN hull_dmg $G= shield_dmg THEN hull_dmg^
			ELSE shield_dmg END * refire_rate * 1000 /^
			CASE WHEN price = 0 THEN 1 ELSE price END)^
   Rating,^
 printf('%%7.2f', velocity) Speed,^
 printf('%%7.2f', velocity * lifetime) Range,^
 printf('%%9.2f', toughness) Toughness,^
 hit_pts 'Hit Pts',^
 yesno.name Lootable,^
 nickname Nickname,^
 ids_name 'Name ID',^
 ids_info 'Info ID' ^
FROM gun, gun_cat, gun_type, equipment, munition, yesno ^
WHERE motor_id = 0^
  AND gun.equip_id = equipment.id^
  AND gun_cat.id = gun.category^
  AND gun_type.id = gt_id^
  AND munition_id = munition.equip_id^
  AND lootable = yesno.id^
  AND price != 0 AND (lootable = 1 OR buyable = 1) ^
$*;%SQLPROMPT%


:: GunMod 'technology' [sql] - list gun modifiers.
:: E.g. GunMod 'w_laser01' AND shield_type.name LIKE '%1' ORDER BY modifier
doskey /exename=sqlite3.exe GunMod=.prompt ''$T^
.width 0 0 -22$T^
SELECT^
 gun_type.name 'Gun Technology',^
 shield_type.name 'Shield Technology',^
 modifier 'Shield Damage Modifier' ^
FROM gun_type, shield_type, shield_mod ^
WHERE gun_type.id = gt_id^
  AND shield_type.id = st_id^
  AND gun_type.name = $*;%SQLPROMPT%


:: ShieldMod 'technology' [sql] - list shield modifiers.
:: E.g. ShieldMod 's_graviton01' AND gun_type.name LIKE '%1' ORDER BY modifier
doskey /exename=sqlite3.exe ShieldMod=.prompt ''$T^
.width 0 0 -22$T^
SELECT^
 gun_type.name 'Gun Technology',^
 shield_type.name 'Shield Technology',^
 modifier 'Shield Damage Modifier' ^
FROM gun_type, shield_type, shield_mod ^
WHERE gun_type.id = gt_id^
  AND shield_type.id = st_id^
  AND shield_type.name = $*;%SQLPROMPT%


:: Missiles [sql] - list missiles, torpedos & cruise disruptors.
:: E.g. Missiles ORDER BY Gun
doskey /exename=sqlite3.exe Missiles=.prompt ''$T^
.width 27 -5 7 -7 -6 -8 -7 -7 -10 -7 -6 -5 -6 6 -6 -6 -7 -7 -8 -5 -9^
       -8 -5 -5 -5 -4 -4 -4 -9 -9 -7 8 33 -7 -7$T^
.print^
 "                            "^
 "      "^
 "        "^
 "        "^
 "  Ammo "^
 "         "^
 " Shield "^
 " Power  "^
 "           "^
 "        "^
 "       "^
 "Det.  "^
 "       "^
 "       "^
 "Seeker "^
 "Seeker "^
 "  Time  "^
 " Muzzle "^
 "         "^
 "Motor "^
 "  Motor   "^
 "  Motor  "^
 "---- Max Range ---- "^
 "-- Max  Speed -- "^
 "   Max"$T^
SELECT^
 launcher.name COLLATE NOCASE Gun,^
 CASE WHEN gun_cat.id != 3 OR class = 0 THEN '' ELSE class END Class,^
 gun_cat.name Type,^
 launcher.price Price,^
 ammo.price Price,^
 printf('%%8.2f', explosion.hull_dmg) 'Hull Dmg',^
 printf('%%7.2f', explosion.shield_dmg) Damage,^
 printf('%%7.2f', power_usage) 'Usage ',^
 printf('%%10.2f', CASE WHEN explosion.hull_dmg $G= explosion.shield_dmg^
			THEN explosion.hull_dmg ELSE explosion.shield_dmg END /^
		   CASE WHEN power_usage = 0 THEN 1 ELSE power_usage END)^
   Efficiency,^
 printf('%%7.2f', CASE WHEN explosion.hull_dmg $G= explosion.shield_dmg^
		       THEN explosion.hull_dmg ELSE explosion.shield_dmg END /^
		  CASE WHEN ammo.price = 0 THEN 1 ELSE ammo.price END)^
   Value,^
 printf('%%6.2f', refire_rate) Refire,^
 detonation_dist 'Dist.',^
 radius Radius,^
 seeker_type.name Seeker,^
 seeker_range Range,^
 seeker_fov_deg FOV,^
 time_to_lock 'to Lock',^
 printf('%%7.2f', velocity) Speed,^
 printf('%%8.4f', munition.lifetime) Lifetime,^
 printf('%%5.2f', delay) Delay,^
 printf('%%9.4f', accel) 'Accel. ',^
 printf('%%8.4f', motor.lifetime) Lifetime,^
 printf('%%5.0f', max_range) Rest,^
 printf('%%5.0f', max_range + 80 * munition.lifetime) '80 ',^
 printf('%%5.0f', max_range + 200 * munition.lifetime) '200',^
 printf('%%4.0f', max_speed) Rest,^
 printf('%%4.0f', max_speed + 80) '80 ',^
 printf('%%4.0f', max_speed + 200) '200',^
 printf('%%9.2f', max_angular_velocity) 'Ang. Vel.',^
 printf('%%9.2f', toughness) Toughness,^
 launcher.hit_pts 'Hit Pts',^
 yesno.name Lootable,^
 launcher.nickname Nickname,^
 launcher.ids_name 'Name ID',^
 launcher.ids_info 'Info ID' ^
FROM gun, gun_cat, equipment launcher, equipment ammo, munition,^
     explosion, motor, seeker_type, yesno ^
WHERE motor_id != 0^
  AND gun.equip_id = launcher.id^
  AND munition_id = ammo.id^
  AND gun_cat.id = gun.category^
  AND munition_id = munition.equip_id^
  AND explosion_id = explosion.id^
  AND motor_id = motor.id^
  AND seeker = seeker_type.id^
  AND launcher.lootable = yesno.id^
  AND launcher.price != 0 AND (launcher.lootable = 1 OR launcher.buyable = 1) ^
$*;%SQLPROMPT%


:: Mines [sql] - list mines.
:: E.g. Mines ORDER BY Mine
doskey /exename=sqlite3.exe Mines=.prompt ''$T^
.width 17 -6 -5 -8 -7 -5 -6 -5 -6 -5 -5 -6 -6 -8 -5 -9 -7 8 13 -7 -7$T^
.print^
 "                  "^
 "       "^
 " Ammo "^
 "         "^
 " Shield "^
 "      "^
 "       "^
 "Det.  "^
 "       "^
 "Seek  "^
 " Top  "^
 "       "^
 "Linear "^
 "         "^
 "Owner"$T^
SELECT^
 dropper.name COLLATE NOCASE Mine,^
 dropper.price Price,^
 ammo.price Price,^
 printf('%%8.2f', explosion.hull_dmg) 'Hull Dmg',^
 printf('%%7.2f', explosion.shield_dmg) Damage,^
 printf('%%5.2f', CASE WHEN explosion.hull_dmg $G= explosion.shield_dmg^
		      THEN explosion.hull_dmg ELSE explosion.shield_dmg END /^
		  CASE WHEN ammo.price = 0 THEN 1 ELSE ammo.price END)^
   Value,^
 printf('%%6.2f', refire_rate) Refire,^
 detonation_dist 'Dist.',^
 radius Radius,^
 seek_dist 'Dist.',^
 top_speed Speed,^
 acceleration 'Accel.',^
 printf('%%6.2f', linear_drag) 'Drag ',^
 lifetime Lifetime,^
 owner_safe_time Safe,^
 printf('%%9.2f', toughness) Toughness,^
 dropper.hit_pts 'Hit Pts',^
 yesno.name Lootable,^
 dropper.nickname Nickname,^
 dropper.ids_name 'Name ID',^
 dropper.ids_info 'Info ID' ^
FROM minedropper, mine, equipment dropper, equipment ammo, explosion, yesno ^
WHERE mine_id = mine.equip_id^
  AND minedropper.equip_id = dropper.id^
  AND mine_id = ammo.id^
  AND explosion_id = explosion.id^
  AND dropper.lootable = yesno.id ^
$*;%SQLPROMPT%


:: Shields [sql] - list shields.
:: E.g. Shields ORDER BY Shield
doskey /exename=sqlite3.exe Shields=.prompt ''$T^
.width 33 -5 9 13 -7 -8 -6 -10 -6 -10 -7 -9 -7 8 28 -7 -7$T^
.print^
 "                                  "^
 "      "^
 "          "^
 "              "^
 "        "^
 "         "^
 "Regen. "^
 " Constant  "^
 "       "^
 " Rebuild   "^
 "Rebuild"$T^
SELECT^
 equipment.name COLLATE NOCASE Shield,^
 CASE WHEN class = 0 THEN '' ELSE class END Class,^
 shield_cat.name Type,^
 shield_type.name Technology,^
 price Price,^
 max_capacity Capacity,^
 regen_rate 'Rate ',^
 constant_power_draw 'Power Draw',^
 printf('%%6.2f', CASE WHEN max_capacity != 0 THEN max_capacity^
		       WHEN regen_rate != 0 THEN regen_rate^
		       ELSE constant_power_draw END * 1000.0 /^
		  CASE WHEN price = 0 THEN 1 ELSE price END) Value,^
 rebuild_power_draw 'Power Draw',^
 off_rebuild_time Time,^
 printf('%%9.2f', toughness) Toughness,^
 hit_pts 'Hit Pts',^
 yesno.name Lootable,^
 nickname Nickname,^
 ids_name 'Name ID',^
 ids_info 'Info ID' ^
FROM shield, shield_cat, shield_type, equipment, yesno ^
WHERE equip_id = equipment.id^
  AND shield.category = shield_cat.id^
  AND st_id = shield_type.id^
  AND lootable = yesno.id^
  AND price != 0 AND (lootable = 1 OR buyable = 1) ^
$*;%SQLPROMPT%


:: Thrusters [sql] - list thrusters.
:: E.g. Thrusters ORDER BY Thruster
doskey /exename=sqlite3.exe Thrusters=.prompt ''$T^
.width 28 -5 -9 -11 -10 -6 -7 -7 8 20 -7 -7$T^
SELECT^
 equipment.name COLLATE NOCASE Thruster,^
 price Price,^
 max_force 'Max Force',^
 printf('%%11.2f', power_usage) 'Power Usage',^
 printf('%%10.2f', CASE WHEN max_force = 0 THEN power_usage^
		   ELSE max_force /^
		   CASE WHEN power_usage = 0 THEN 1 ELSE power_usage END END)^
   Efficiency,^
 printf('%%6.2f', CASE WHEN max_force = 0^
		  THEN power_usage * 1000 ELSE CAST(max_force AS REAL) END /^
		  CASE WHEN price = 0 THEN 1 ELSE price END) Value,^
 printf('%%7.2f', max_force / CASE WHEN power_usage $L= 100 THEN 1^
				   ELSE power_usage - 100 END *^
		  max_force / 1000 /^
		  CASE WHEN price = 0 THEN 1 ELSE price END) Rating,^
 hit_pts 'Hit Pts',^
 yesno.name Lootable,^
 nickname Nickname,^
 ids_name 'Name ID',^
 ids_info 'Info ID' ^
FROM thruster, equipment, yesno ^
WHERE equip_id = equipment.id^
  AND lootable = yesno.id^
  AND price != 0 AND (lootable = 1 OR buyable = 1) ^
$*;%SQLPROMPT%


:: Engines [sql] - list engines.
:: E.g. Engines ORDER BY Engine
doskey /exename=sqlite3.exe Engines=.prompt ''$T^
.width 39 -8 -10 -9 -8 -6 -7 -6 -6 -11 20 -7 -7$T^
.print^
 "                                        "^
 "         "^
 "   Max     "^
 " Linear   "^
 "Reverse  "^
 "  Top  "^
 "Reverse "^
 "      "^
 "      "^
 "    Cruise"$T^
SELECT^
 name COLLATE NOCASE Engine,^
 price Price,^
 printf('%%10.2f', max_force) 'Force   ',^
 printf('%%9.2f', linear_drag) 'Drag   ',^
 printf('%%8.2f', reverse_fraction) Fraction,^
 printf('%%6.2f', max_force / linear_drag) Speed,^
 printf('%%7.2f', max_force / linear_drag * reverse_fraction) 'Speed ',^
 printf('%%6.2f', max_force / linear_drag * 1000 / price) Value,^
 printf('%%6.2f', max_force / linear_drag * 1000 / price * reverse_fraction) Rating,^
 printf('%%11.2f', cruise_charge_time) 'Charge Time',^
 nickname Nickname,^
 ids_name 'Name ID',^
 ids_info 'Info ID' ^
FROM engine, equipment ^
WHERE equip_id = id^
  AND price != 0 AND (lootable = 1 OR buyable = 1) ^
$*;%SQLPROMPT%


:: Powerplants [sql] - list powerplants.
:: E.g. Powerplants ORDER BY Powerplant
doskey /exename=sqlite3.exe Powerplants=.prompt ''$T^
.width 43 -7 -8 -13 -6 -6 -8 -13 21 -7 -7$T^
.print^
 "                                            "^
 "        "^
 " Power   "^
 "    Power     "^
 "       "^
 "       "^
 " Thrust  "^
 "   Thrust     "$T^
SELECT^
 name COLLATE NOCASE Powerplant,^
 price Price,^
 capacity Capacity,^
 charge_rate 'Recharge Rate',^
 printf('%%6.2f', capacity * 1000.0 / price) Value,^
 printf('%%6.2f', CAST(capacity AS REAL) / price * charge_rate) Rating,^
 thrust_capacity Capacity,^
 thrust_charge_rate 'Recharge Rate',^
 nickname Nickname,^
 ids_name 'Name ID',^
 ids_info 'Info ID' ^
FROM _power, equipment ^
WHERE equip_id = id^
  AND price != 0 AND (lootable = 1 OR buyable = 1) ^
$*;%SQLPROMPT%


:: Ships [sql] - list ships.
:: E.g. Ships ORDER BY Ship
doskey /exename=sqlite3.exe Ships=.prompt ''$T^
.width 21 18 9 -9 -6 -9 -8 -9 -13 -16 -17 -8 -13 -8 -13 -7 -8 -11 -12 25 -7 -7$T^
.print^
 "                      "^
 "                   "^
 "          "^
 "          "^
 "       "^
 "          "^
 "         "^
 "          "^
 " Max Angular  "^
 "Angular Distance "^
 " Time to 90%% Max  "^
 " Power   "^
 "    Power     "^
 " Thrust  "^
 "   Thrust     "^
 "Impulse  "^
 "Reverse"$T^
SELECT^
 equipment.name COLLATE NOCASE Ship,^
 CASE WHEN class != 0 OR type = 'fighter' THEN ship_class.name ELSE '' END^
   Class,^
 type Type,^
 package.price Price,^
 equipment.hit_pts Armor,^
 hold_size 'Hold Size',^
 nanobots Nanobots,^
 batteries Batteries,^
 printf('%%13.2f', max_angular_speed * 180 / 3.1415926536) 'Speed (deg/s)',^
 printf('%%16.2f', dist_05 * 180 / 3.1415926536) 'in 0.5 sec (deg)',^
 printf('%%17.4f', time_to_reach_90) 'Angular Speed (s)',^
 capacity Capacity,^
 charge_rate 'Recharge Rate',^
 thrust_capacity Capacity,^
 thrust_charge_rate 'Recharge Rate',^
 printf('%%7.2f', max_force / (engine.linear_drag + ship.linear_drag)) 'Speed ',^
 printf('%%8.2f', reverse_fraction) Fraction,^
 printf('%%11.2f', nudge_force) 'Nudge Force',^
 printf('%%12.2f', strafe_force) 'Strafe Force',^
 equipment.nickname Nickname,^
 equipment.ids_name 'Name ID',^
 equipment.ids_info 'Info ID' ^
FROM ship,ship_class, equipment, _power,engine,ship_package,equipment package ^
WHERE ship.equip_id = equipment.id^
  AND ship_package.ship_id = ship.equip_id^
  AND ship_package.equip_id = package.id^
  AND ship_package.power_id = _power.equip_id^
  AND ship_package.engine_id = engine.equip_id^
  AND ship_class.id = class^
  AND package.price != 0 AND package.buyable = 1 ^
$*;%SQLPROMPT%


:: HP 'ship' - list hardpoints for SHIP.
:: E.g. HP 'Eagle'
:: Note: assumes classes are consecutive.
doskey /exename=sqlite3.exe HP=.prompt ''$T^
.width 18 -5 16$T^
SELECT^
 hard_point.nickname Hardpoint,^
 CASE WHEN type BETWEEN 4 AND 8 THEN ''^
      ELSE printf('%%d-%%d', min(class), max(class)) END Class,^
 CASE WHEN type IN (4,5) AND count(type) = 2 THEN 'CD/T'^
      ELSE hp_cat.name END Type ^
FROM ship_hp, ship_package, hard_point, hp_type, hp_cat, equipment ^
WHERE ship_hp.ship_id = equipment.id^
  AND ship_hp.ship_id = ship_package.ship_id^
  AND hp_id = hard_point.id^
  AND hptype_id = hp_type.id^
  AND hp_cat.id = type^
  AND equipment.name = $*^
GROUP BY Hardpoint ^
ORDER BY hp_cat.id, Hardpoint;%SQLPROMPT%


:: HPclass 'ship' - list hardpoints for SHIP.
:: E.g. HPclass 'Eagle'
:: Note: lists all classes, not just the min-max range.
doskey /exename=sqlite3.exe HPclass=.prompt ''$T^
.width 18 16 -20$T^
SELECT^
 hard_point.nickname Hardpoint,^
 CASE WHEN type IN (4,5) AND count(type) = 2 THEN 'CD/T'^
      ELSE hp_cat.name END Type,^
 CASE WHEN type BETWEEN 4 AND 8 THEN ''^
      ELSE group_concat(class, ' ') END Class ^
FROM ship_hp, ship_package, hard_point, hp_type, hp_cat, equipment ^
WHERE ship_hp.ship_id = equipment.id^
  AND ship_hp.ship_id = ship_package.ship_id^
  AND hp_id = hard_point.id^
  AND hptype_id = hp_type.id^
  AND hp_cat.id = type^
  AND equipment.name = $*^
GROUP BY Hardpoint ^
ORDER BY hp_cat.id, Hardpoint;%SQLPROMPT%


:: Help - show commands.
doskey /exename=sqlite3.exe Help=.print^
 "\nBases [sql] - list bases."^
 "\nGoods 'base' [sql] - list goods on BASE."^
 "\nFactions [sql] - list factions."^
 "\nFaction 'faction' - find the nickname of FACTION (long or short name)."^
 "\nEmpathy 'nickname' [sql] - list empathies of a faction."^
 "\nBribe 'faction' - list bribes for FACTION (long, short or nickname)."^
 "\nGood 'good' [sql] - show bases containing GOOD."^
 "\nShip 'ship' [sql] - show bases containing SHIP."^
 "\nCommodities [sql] - list commodities."^
 "\nGuns [sql] - list guns & turrets."^
 "\nGunMod 'technology' [sql] - list modifiers for gun TECHNOLOGY."^
 "\nShieldMod 'technology' [sql] - list modifiers for shield TECHNOLOGY."^
 "\nMissiles [sql] - list missiles, torpedos & cruise disruptors."^
 "\nMines [sql] - list mines."^
 "\nShields [sql] - list shields."^
 "\nThrusters [sql] - list thrusters."^
 "\nEngines [sql] - list engines."^
 "\nPowerplants [sql] - list powerplants."^
 "\nShips [sql] - list ships."^
 "\nHP 'ship' - list hardpoints for SHIP (class is MIN-MAX range)."^
 "\nHPclass 'ship' - list hardpoints for SHIP (shows all classes)."^
 "\n\nE.g. Good 'Cannonball Missile' AND category = 2 ORDER BY Base"^
 "\nCategory restricts it to the launcher, not the ammo; use 6 for mine droppers.\n"


sqlite3 -header -column -init %SQLFile%^
 -cmd ".print Enter \\\"Help\\\" for FLStat queries.\n"^
 -cmd ".prompt 'FLStat> '"
