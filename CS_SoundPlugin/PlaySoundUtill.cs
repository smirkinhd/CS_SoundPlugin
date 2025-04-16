using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;

namespace CS_SoundPlugin
{
    public static class PlaySoundUtill
    {
        public static void PlaySound(CCSPlayerController player, JoinSoundConfig config)
        {
            try
            {
                if (!player.IsValid || player.IsBot)
                    return;

                string soundPath = GetSoundPath(player, config);
                if (!string.IsNullOrEmpty(soundPath))
                {
                    player.ExecuteClientCommand($"play {soundPath}");
                    Server.PrintToConsole($"Played sound {soundPath} for {player.PlayerName}");
                }
                else
                {
                    Server.PrintToConsole("No sound available for playback.");
                }
            }
            catch (Exception ex)
            {
                Server.PrintToConsole($"Error playing sound for {player.PlayerName}: {ex.Message}");
            }
        }

        public static string GetSoundPath(CCSPlayerController player, JoinSoundConfig Config)
        {
            Dictionary<ulong, int> playerSoundIndex = new Dictionary<ulong, int>();

            if (Config.MusicList.Count == 0)
                return string.Empty;

            switch (Config.SoundMode.ToLower())
            {
                case "random":
                    return Config.MusicList[new Random().Next(Config.MusicList.Count)].Path;

                case "order":
                default:
                    var steamId = player.SteamID;
                    if (!playerSoundIndex.ContainsKey(steamId))
                    {
                        playerSoundIndex[steamId] = 0;
                    }

                    int index = playerSoundIndex[steamId];
                    string path = Config.MusicList[index].Path;
                    playerSoundIndex[steamId] = (index + 1) % Config.MusicList.Count;
                    return path;
            }
        }
        public static void PlayKnifeKillSound(JoinSoundConfig Config)
        {
            try
            {
                string knifeSoundPath = Config.MusicList.FirstOrDefault()?.Knife ?? string.Empty;
                if (string.IsNullOrEmpty(knifeSoundPath))
                {
                    Server.PrintToConsole("No knife kill sound configured.");
                    return;
                }

                var players = Utilities.GetPlayers()
                    .Where(p => p.IsValid && !p.IsBot)
                    .ToList();

                foreach (var player in players)
                {
                    player.ExecuteClientCommand($"play {knifeSoundPath}");
                }

                Server.PrintToConsole($"Played knife kill sound {knifeSoundPath} for all players.");
            }
            catch (Exception ex)
            {
                Server.PrintToConsole($"Error playing knife kill sound: {ex.Message}");
            }
        }
    }
}
