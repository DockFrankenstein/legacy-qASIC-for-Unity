using System;
using System.Collections.Generic;
using qASIC.InputManagement.Devices;
using qASIC.InputManagement.Map;
using UnityEngine;

namespace qASIC.InputManagement.Players
{
    public static class InputPlayerManager
    {
        private static string FIRST_PLAYER_DEFAULT_NAME = "InputPlayer.0";

        public static List<InputPlayer> Players { get; private set; } = new List<InputPlayer>() { new InputPlayer(FIRST_PLAYER_DEFAULT_NAME), };
        public static List<string> PlayerNames { get; private set; } = new List<string>( new string[] { FIRST_PLAYER_DEFAULT_NAME });

        public static event Action<InputPlayer> OnPlayerCreated;
        public static event Action<InputPlayer> OnPlayerRemoved;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Initialize()
        {
            DeviceManager.OnDeviceConnected += HandleDeviceConnected;
            DeviceManager.OnDeviceDisconnected += (_, device) =>
            {
                foreach (InputPlayer player in Players)
                    if (player.CurrentDevices.Contains(device))
                        player.CurrentDevices.Remove(device);
            };

            InputUpdateManager.OnUpdate += UpdatePlayers;
        }

        static void HandleDeviceConnected(int index, IInputDevice device)
        {
            if (Players.Count == 0)
            {
                CreatePlayerFromDevice(device);
                return;
            }

            Players[0].CurrentDevices.Add(device);
        }

        public static void CreatePlayerFromDevice(IInputDevice device)
        {
            InputPlayer newPlayer = null;
            int playerIndex = 0;

            //Register on an empty player, if there is one
            for (int i = 0; i < Players.Count; i++)
            {
                if (Players[i].CurrentDevice != null) continue;
                Players[i].CurrentDevices.Add(device);
                playerIndex = i;
                newPlayer = Players[i];
                break;
            }

            //If it's a new player
            if (newPlayer == null)
            {
                Players.Add(null);
                PlayerNames.Add(string.Empty);
                playerIndex = Players.Count - 1;
                newPlayer = new InputPlayer(CreateNewPlayerName(), device);

                if (InputManager.Map != null)
                    newPlayer.MapData = InputManager.Map.GetData();
            }

            if (InputManager.MapLoaded)
                newPlayer.MapData = InputManager.Map.GetData();

            Players[playerIndex] = newPlayer;
            PlayerNames[playerIndex] = newPlayer.ID;

            //Call method if it's new
            if (playerIndex == Players.Count - 1)
                OnPlayerCreated?.Invoke(newPlayer);
        }

        /// <summary>Resets every player's map data and bindings for this session</summary>
        public static void RebuildPlayerMapData(InputMap map)
        {
            foreach (var player in Players)
            {
                if (player == null) continue;
                player.Map = map;
                player.MapData = map.GetData();
            }
        }

        #region Logic
        private static void UpdatePlayers()
        {
            foreach (var player in Players)
                if (player != null)
                    foreach (var device in player.CurrentDevices)
                        device.Update();
        }
        #endregion

        #region Removing
        public static void RemovePlayerByID(string id) =>
            RemovePlayer(GetPlayerByID(id));

        public static void RemovePlayer(InputPlayer player)
        {
            if (!Players.Contains(player))
            {
                Logger.DeleteErrorPlayerNotFound(player);
                return;
            }

            int index = Players.IndexOf(player);

            Players[index] = null;
            PlayerNames[index] = string.Empty;

            OnPlayerRemoved?.Invoke(player);
        }
        #endregion

        #region Get
        public static InputPlayer GetPlayerByID(string id) =>
            TryGetPlayerByID(id, out InputPlayer player) ? player : null;

        public static bool TryGetPlayerByID(string id, out InputPlayer player)
        {
            foreach (InputPlayer p in Players)
            {
                if (p.ID == id)
                {
                    player = p;
                    return true;
                }
            }

            player = null;
            return false;
        }
        #endregion

        #region Utility
        /// <summary>Generates an unused name for the new player</summary>
        public static string CreateNewPlayerName()
        {
            string template = "InputPlayer.{0}";
            int i = 0;
            while (true)
            {
                string name = string.Format(template, i);
                if (!PlayerNames.Contains(name))
                    return name;

                i++;
            }
        }
        #endregion

        static class Logger
        {
            public static void DeleteErrorPlayerNotFound(InputPlayer player) =>
                qDebug.LogError($"[Cablebox] Couldn't delete player '{player.ID}', player not found.");
        }
    }
}
