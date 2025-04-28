namespace WebApi.Authentication.Entity;

public record User(string Username, string Password, string Role, string[] Scopes);
public record LoginModel(string Username, string Password);
