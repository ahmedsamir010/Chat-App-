public class PresenceTracker
{
    public static readonly Dictionary<string, List<string>> OnlineUsers = new Dictionary<string, List<string>>();

    public static bool ConnectedUser(string username, string connectionId)
    {
        bool isOnline = false;
        lock (OnlineUsers)
        {
            if (OnlineUsers.TryGetValue(username, out var connections))
            {
                connections.Add(connectionId);
            }
            else
            {
                OnlineUsers[username] = new List<string> { connectionId };
                isOnline = true;
            }
        }
        return isOnline;
    }

    public async Task<bool> UserDisConnected(string username, string connectionId)
    {
        bool isOffline = false;
        lock (OnlineUsers)
        {
            if (!OnlineUsers.ContainsKey(username))
            {
                return isOffline;
            }

            var connections = OnlineUsers[username];
            connections?.Remove(connectionId);

            if (connections?.Count == 0)
            {
                OnlineUsers.Remove(username);
                isOffline = true;
            }
        }
        return isOffline;
    }

    public static Task<string[]> GetOnlineUsers()
    {
        string[] onlineUsersArray;
        lock (OnlineUsers)
        {
            onlineUsersArray = OnlineUsers.Keys.OrderBy(userName => userName).ToArray();
        }
        return Task.FromResult(onlineUsersArray);
    }

    public static async Task<List<string>> GetConnectionForUsers(string username)
    {
        List<string> connectionIds;
        lock (OnlineUsers)
        {
            connectionIds = OnlineUsers.GetValueOrDefault(username);
        }
        return connectionIds;
    }
}
