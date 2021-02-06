using System;
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
                return Path.Combine(Assembly.GetEntryAssembly().Location);
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
            File.WriteAllText(dataPath, GenerateJsonFromData());

            return Task.CompletedTask;

            #region Local_Function

            string GenerateJsonFromData() {
                return JsonConvert.SerializeObject(channel);
            }

            #endregion
        }

        internal static Task<Channel> FetchChannelData(string channelID) {
            string dataPath = Path.Combine(DatabaseRootPath, CHANNELS, channelID + ".json");

            if (!File.Exists(dataPath)) {
                return Task.FromResult(new Channel(channelID));
            } else {
                string jsonData = File.ReadAllText(dataPath);
                return Task.FromResult(GenerateDataFromJson(jsonData));
            }

            #region Local_Function



            Channel GenerateDataFromJson(string json) {
                return JsonConvert.DeserializeObject<Channel>(json);
            }

            #endregion
        }

    }
}
