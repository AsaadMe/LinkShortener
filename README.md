# Link Shortener

Using .NET8 Minimal API / MySQL / MySql.EntityFrameworkCore

### API Endpoints

- `/create/{Full URL}` -> return short link url.

  Example: /create/https://google.com -> \<domain\>/a/htyb

- `/a/{Short Key}` -> redirect to Full URL.

### _Links_ Table Schema

| Name         | Type         | Options                     |
| ------------ | ------------ | --------------------------- |
| ID           | int          | AUTO_INCREAMENT PRIMARY KEY |
| LongUrl      | varchar(255) | NOT NULL UNIQUE             |
| ShortKey     | varchar(255) | NOT NULL                    |
| CreationTime | datetime     |
