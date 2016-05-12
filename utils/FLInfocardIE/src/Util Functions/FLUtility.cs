/*
 * Purpose: Miscellanous utility functions.
 * Author: Cannon
 * Date: Jan 2010
 * 
 * Item hash algorithm from flhash.exe by sherlog@t-online.de (2003-06-11)
 * Faction hash algorithm from flfachash.exe by Haenlomal (October 2006)
 * 
 * This is free software. Permission to copy, store and use granted as long
 * as this note remains intact.
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace FLInfocardIE.Util_Functions
{
    class FLUtility
    {
        public static DateTime INVALID_DATE = new DateTime(0);

        /// <summary>
        /// Decode an ascii hex string into unicode
        /// </summary>
        /// <param name="encodedName">The encoded value</param>
        /// <returns>The deocded value</returns>
        public static string DecodeUnicodeHex(string encodedValue)
        {
            string name = "";
            while (encodedValue.Length > 0)
            {
                name += (char)System.Convert.ToUInt16(encodedValue.Substring(0, 4), 16);
                encodedValue = encodedValue.Remove(0, 4);
            }
            return name;
        }


        /// <summary>
        /// Decode a unicode string into ascii hex
        /// </summary>
        /// <param name="value">The value string to encode</param>
        /// <returns>The encoded value</returns>
        public static string EncodeUnicodeHex(string value)
        {
            return BitConverter.ToString(Encoding.BigEndianUnicode.GetBytes(value)).Replace("-","");
        }

        
        /// <summary>
        /// Look up table for faction id creation.
        /// </summary>
        private static uint[] createFactionIDTable = null;

        /// <summary>
        /// Function for calculating the Freelancer data nickname hash.
        /// Algorithm from flfachash.c by Haenlomal (October 2006).
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public static uint CreateFactionID(string nickName)
        {
            const uint FLFACHASH_POLYNOMIAL = 0x1021;
            const uint NUM_BITS = 8;
            const int HASH_TABLE_SIZE = 256;

            if (createFactionIDTable == null)
            {
                // The hash table used is the standard CRC-16-CCITT Lookup table 
                // using the standard big-endian polynomial of 0x1021.
                createFactionIDTable = new uint[HASH_TABLE_SIZE];
                for (uint i = 0; i < HASH_TABLE_SIZE; i++)
                {
                    uint x = i << (16 - (int)NUM_BITS);
                    for (uint j = 0; j < NUM_BITS; j++)
                    {
                        x = ((x & 0x8000) == 0x8000) ? (x << 1) ^ FLFACHASH_POLYNOMIAL : (x << 1);
                        x &= 0xFFFF;
                    }
                    createFactionIDTable[i] = x;
                }
            }

            byte[] tNickName = Encoding.ASCII.GetBytes(nickName.ToLowerInvariant());

            uint hash = 0xFFFF;
            for (uint i = 0; i < tNickName.Length; i++)
            {
                uint y = (hash & 0xFF00) >> 8;
                hash = y ^ (createFactionIDTable[(hash & 0x00FF) ^ tNickName[i]]);
            }

            return hash;
        }

        /// <summary>
        /// Look up table for id creation.
        /// </summary>
        private static uint[] createIDTable = null;

        /// <summary>
        /// Function for calculating the Freelancer data nickname hash.
        /// Algorithm from flhash.exe by sherlog@t-online.de (2003-06-11)
        /// </summary>
        /// <param name="nickName"></param>
        /// <returns></returns>
        public static uint CreateID(string nickName)
        {
            const uint FLHASH_POLYNOMIAL = 0xA001;
            const int LOGICAL_BITS = 30;
            const int PHYSICAL_BITS = 32;

            // Build the crc lookup table if it hasn't been created
            if (createIDTable == null)
            {
                createIDTable = new uint[256];
                for (uint i = 0; i < 256; i++)
                {
                    uint x = i;
                    for (uint bit = 0; bit < 8; bit++)
                        x = ((x & 1) == 1) ? (x >> 1) ^ (FLHASH_POLYNOMIAL << (LOGICAL_BITS - 16)) : x >> 1;
                    createIDTable[i] = x;
                }
                if (2926433351 != CreateID("st01_to_st03_hole")) throw new Exception("Create ID hash algoritm is broken!");
                if (2460445762 != CreateID("st02_to_st01_hole")) throw new Exception("Create ID hash algoritm is broken!");
                if (2263303234 != CreateID("st03_to_st01_hole")) throw new Exception("Create ID hash algoritm is broken!");
                if (2284213505 != CreateID("li05_to_li01")) throw new Exception("Create ID hash algoritm is broken!");
                if (2293678337 != CreateID("li01_to_li05")) throw new Exception("Create ID hash algoritm is broken!");
            }

            byte[] tNickName = Encoding.ASCII.GetBytes(nickName.ToLowerInvariant());

            // Calculate the hash.
            uint hash = 0;
            for (int i = 0; i < tNickName.Length; i++)
                hash = (hash >> 8) ^ createIDTable[(byte)hash ^ tNickName[i]];
            // b0rken because byte swapping is not the same as bit reversing, but 
            // that's just the way it is; two hash bits are shifted out and lost
            hash = (hash >> 24) | ((hash >> 8) & 0x0000FF00) | ((hash << 8) & 0x00FF0000) | (hash << 24);
            hash = (hash >> (PHYSICAL_BITS - LOGICAL_BITS)) | 0x80000000;

            return hash;
        }

        /// <summary>
        /// Escape a string for an expression.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeLikeExpressionString(string value)
        {
            string escapedText = value;
            escapedText = escapedText.Replace("[", "[[]");
            //filter = filter.Replace("]", "[]]");
            escapedText = escapedText.Replace("%", "[%]");
            escapedText = escapedText.Replace("*", "[*]");
            escapedText = escapedText.Replace("'", "''");
            return escapedText;
        }

        /// <summary>
        /// Escape a string for an expression.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string EscapeEqualsExpressionString(string value)
        {
            string escapedText = value;
            escapedText = escapedText.Replace("'", "''");
            return escapedText;
        }

        /// <summary>
        /// Get the account id from the specified account directory.
        /// Will throw file open exceptions if the 'name' file cannot be opened.
        /// </summary>
        /// <param name="accDirPath">The account directory to search.</param>
        public static string GetAccountID(string accDirPath)
        {
            string accountIdFilePath = accDirPath + Path.DirectorySeparatorChar + "name";

            // Read a 'name' file into memory.
            FileStream fs = System.IO.File.OpenRead(accountIdFilePath);
            byte[] buf = new byte[fs.Length];
            fs.Read(buf, 0, (int)fs.Length);
            fs.Close();

            // Decode the account ID
            string accountID = "";
            for (int i = 0; i < buf.Length; i += 2)
            {
                switch (buf[i])
                {
                    case 0x43:
                        accountID += '-';
                        break;
                    case 0x0f:
                        accountID += 'a';
                        break;
                    case 0x0c:
                        accountID += 'b';
                        break;
                    case 0x0d:
                        accountID += 'c';
                        break;
                    case 0x0a:
                        accountID += 'd';
                        break;
                    case 0x0b:
                        accountID += 'e';
                        break;
                    case 0x08:
                        accountID += 'f';
                        break;
                    case 0x5e:
                        accountID += '0';
                        break;
                    case 0x5f:
                        accountID += '1';
                        break;
                    case 0x5c:
                        accountID += '2';
                        break;
                    case 0x5d:
                        accountID += '3';
                        break;
                    case 0x5a:
                        accountID += '4';
                        break;
                    case 0x5b:
                        accountID += '5';
                        break;
                    case 0x58:
                        accountID += '6';
                        break;
                    case 0x59:
                        accountID += '7';
                        break;
                    case 0x56:
                        accountID += '8';
                        break;
                    case 0x57:
                        accountID += '9';
                        break;
                    default:
                        accountID += '?';
                        break;
                }
            }

            return accountID;
        }

        /// <summary>
        /// Return the location string for the specified player
        /// </summary>
        /// <param name="gameData">The current game data.</param>
        /// <param name="charFile">The player's data file.</param>
        /// <returns>A string containing the location.</returns>
        public static string GetLocation(FLGameData gameData, FLDataFile charFile)
        {
            string location = gameData.GetItemDescByNickNameX(charFile.GetSetting("Player", "system").Str(0));
            if (charFile.SettingExists("Player", "pos"))
            {
                float posX = charFile.GetSetting("Player", "pos").Float(0);
                float posY = charFile.GetSetting("Player", "pos").Float(1);
                float posZ = charFile.GetSetting("Player", "pos").Float(2);
                location += String.Format(" in space {0}, {1}, {2}", posX, posY, posZ);
            }
            else
            {
                location += " docked at " + gameData.GetItemDescByNickNameX(charFile.GetSetting("Player", "base").Str(0));
            }
            return location;
        }


        /// <summary>
        /// Return the ship string for the specified player
        /// </summary>
        /// <param name="gameData">The current game data.</param>
        /// <param name="charFile">The player's data file.</param>
        /// <returns>A string containing the ship name.</returns>
        public static string GetShip(FLGameData gameData, FLDataFile charFile, out Int64 shipArchType)
        {
            string nickNameOrHash = charFile.GetSetting("Player", "ship_archetype").Str(0);
            GameDataSet.HashListRow shipItem = gameData.GetItemByNickName(nickNameOrHash);
            if (shipItem != null)
            {
                shipArchType = shipItem.ItemHash;
            }
            else
            {
                shipArchType = charFile.GetSetting("Player", "ship_archetype").UInt(0);
            }
            return gameData.GetItemDescByHash(shipArchType);
        }

        /// <summary>
        /// Hack FL formatted xml into a RTF format.
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string FLXmlToRtf(string xml)
        {
            int xmlEnd = xml.IndexOf("</RDP>");
            if (xmlEnd >= 0)
                xml = xml.Substring(0, xmlEnd);
            xml = xml.Replace("<JUST loc=\"center\"/>", "\\qc ");
            xml = xml.Replace("<JUST loc=\"left\"/>", "\\pard ");
            xml = xml.Replace("<TRA data=\"1\" mask=\"1\" def=\"-2\"/>", "\\b ");
            xml = xml.Replace("<TRA data=\"1\" mask=\"1\" def=\"0\"/>", "\\b0 ");
            xml = xml.Replace("<TRA data=\"0\" mask=\"1\" def=\"-1\"/>", "\\b0 ");
            xml = xml.Replace("<PARA/>", "\\par ");
            xml = System.Text.RegularExpressions.Regex.Replace(xml, "<[^<>]*>", "");
            xml = xml.Replace("&gt;", ">");
            xml = xml.Replace("&lt;", "<");
            xml = xml.Trim();
            return xml;
        }

        /// <summary>
        /// Rrturn the charfile file access timestamp. This is the time
        /// this character was last accessed.
        /// </summary>
        /// <param name="charFile">The charfile to check.</param>
        /// <returns>The access time</returns>
        public static DateTime GetTimeStamp(FLDataFile charFile)
        {
            if (charFile.SettingExists("Player", "tstamp"))
            {
                long high = charFile.GetSetting("Player", "tstamp").UInt(0);
                long low = charFile.GetSetting("Player", "tstamp").UInt(1);
                return DateTime.FromFileTime(high << 32 | low);
            }
            return DateTime.Now;
        }

        /// <summary>
        /// Return the secs this character has been played on
        /// </summary>
        /// <param name="charFile">The charfile to check.</param>
        /// <returns>Seconds of in game time.</returns>
        public static uint GetOnLineTime(FLDataFile charfile)
        {
            if (charfile.SettingExists("mPlayer", "total_time_played"))
                return (uint)charfile.GetSetting("mPlayer", "total_time_played").Float(0);
            else
                return 0;
        }
    }
}
