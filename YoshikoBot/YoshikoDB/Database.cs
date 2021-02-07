using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Newtonsoft.Json;

using YoshikoDB.Models;

namespace YoshikoDB {
    internal static class Database {

        private const string CHANNELS = "CHANNELS";
        private const string USERS = "USERS";

        private static string DatabaseRootPath {
            get {
                return Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
        }

        static Database() {
            InitalizePaths();

            #region Local_Function

            void InitalizePaths() {
                Directory.CreateDirectory(Path.Combine(DatabaseRootPath, CHANNELS));
                Directory.CreateDirectory(Path.Combine(DatabaseRootPath, USERS));
            }

            #endregion
        }

        internal static Task UpdateChannelData(Channel channel) {
            string dataPath = Path.Combine(DatabaseRootPath, CHANNELS, channel.ChannelID + ".json");
            File.WriteAllText(dataPath, GenerateJsonFromData<Channel>(channel));

            return Task.CompletedTask;
        }

        internal static Task<Channel> FetchChannelData(string channelID) {
            string dataPath = Path.Combine(DatabaseRootPath, CHANNELS, channelID + ".json");

            if (!File.Exists(dataPath)) {
                return Task.FromResult(new Channel(channelID));
            } else {
                string jsonData = File.ReadAllText(dataPath);
                return Task.FromResult(GenerateDataFromJson<Channel>(jsonData));
            }
        }

        internal static Task<HashSet<Channel>> FetchAllChannelData() {

            // TODO: Optimize
            HashSet<Channel> channelsData = new HashSet<Channel>();

            string channelsDataPath = Path.Combine(DatabaseRootPath, CHANNELS);
            string[] fileNames = Directory.GetFiles(channelsDataPath);

            foreach (string fileName in fileNames) {
                string dataPath = Path.Combine(channelsDataPath, fileName);

                string jsonData = File.ReadAllText(dataPath);
                channelsData.Add(GenerateDataFromJson<Channel>(jsonData));
            }

            return Task.FromResult(channelsData);
        }

        #region Utils


        private static string GenerateJsonFromData<T>(T data) {
            return JsonConvert.SerializeObject(data);
        }

        private static T GenerateDataFromJson<T>(string json) {
            return JsonConvert.DeserializeObject<T>(json);
        }

        #endregion

    }
}
