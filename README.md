# Shelf Life API

A dotnet api to keep track of food expiry.

# Instruction

To run the api locally, first run the migration to create local SQLite database:

```
cd Shelf.Life.Database
dotnet ef database update
```

This should create a `shelf_life.db` database in
```
C:\Users\[username]\AppData\Local\Shelf.Life.Api\shelf_life.db
```

If not found, then run 
```
dotnet ef dbcontext info
```
and look at the `Data source: ...`.

