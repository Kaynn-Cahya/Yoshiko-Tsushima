using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using YoshikoDB.Models;

namespace YoshikoDB {
    public class Query {

        public Task UpdateChannelData(Channel channelData) {
            return Database.UpdateChannelData(channelData);
        }

        public Task<Channel> FetchChannelData(string channelID) {
            return Database.FetchChannelData(channelID);
        }

    }
}
