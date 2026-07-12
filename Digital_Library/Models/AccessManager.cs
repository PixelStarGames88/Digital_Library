namespace Digital_Library.Models;

internal class AccessManager
{
    private static readonly Dictionary<string, (string Password, Dictionary<string, string> Permissions)> _users = new()
{
    { "Преподаватель", ("prof", new() { ["author"] = "CRUD", ["publication"] = "CRUD", ["publisher"] = "CRUD" }) },
    { "Студент", ("stud", new() { ["author"] = "R", ["publication"] = "R", ["publisher"] = "R" }) }
};

    public static bool CheckPassword(string role, string password) =>
        _users.ContainsKey(role) && _users[role].Password == password;

    public static Dictionary<string, string> GetPermissions(string role) =>
        _users.ContainsKey(role) ? _users[role].Permissions : new();
}