# AuthN & AuthZ
To generate jwt token run:
```
dotnet user-jwts create --audience menu-api
```

To generate admin token:
```
dotnet user-jwts create --audience menu-api --claim country=Ukraine --role admin
```