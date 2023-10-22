# Development JWt

See the [Microsoft guide](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/jwt-authn?view=aspnetcore-7.0&tabs=windows) for more details.

```powershell
# grab curl from git-for-windows
$curl = "C:\Program Files\Git\mingw64\bin\curl.exe"

# anonymous requests
& $curl -i http://localhost:5132/random-number
& $curl -i http://localhost:5132/bigger-random-number
& $curl -i http://localhost:5132/secret-number

# requests with user jwt
$token = dotnet user-jwts create --name TestUser --role "user" --output token
& $curl -i -H "Authorization: Bearer $token" http://localhost:5132/random-number
& $curl -i -H "Authorization: Bearer $token" http://localhost:5132/bigger-random-number
& $curl -i -H "Authorization: Bearer $token" http://localhost:5132/secret-number

# requests with admin jwt
$token = dotnet user-jwts create --name TestUser --role "admin" --output token
& $curl -i -H "Authorization: Bearer $token" http://localhost:5132/random-number
& $curl -i -H "Authorization: Bearer $token" http://localhost:5132/bigger-random-number
& $curl -i -H "Authorization: Bearer $token" http://localhost:5132/secret-number
```

`dotnet user-jwts create` adds a config block to appsettings.Development.json, creates a user-jwt json file with the token, and creates user-secret json file with the signing key.
