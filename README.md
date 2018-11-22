# Npgsql.EntityFrameworkCore.PostgreSQL.CaseInsensitive
Case insensitive extension for Npgsql.EntityFrameworkCore.PostgreSQL

## Definition

CaseInsensitive extension allow searching in Npgsql.EntityFrameworkCore.PostgreSQL like at standard MS SQL Server.

## How to use

In you main context, change you method OnConfiguring(DbContextOptionsBuilder optionsBuilder) from invoking 
DbContextOptionsBuilder.UseNpgsql(connectionString) to UseNpgsqlCaseInsensitive(connectionString). 

From now you data filtering will be case insensitive, Enjoy :-) 

Example:
```
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    optionsBuilder.UseNpgsqlCaseInsensitive(_settings.ConnectionString);
}

```
