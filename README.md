# ASCII Assault Server

![Server-Client Diagram](https://github.com/DapperBanana/ASCIIAssaultServer/blob/master/Server_Client_Communication.png)

 - Got the server-client configuration complete

### Next up list for server-side
- [x] Research and choose secure hashing (and salting) algorithm
  - Went with PBKDF2 + SHA256, 100k iterations, 16-byte random salt
- [x] Set up user registration system for people logging in
- [x] confirm password works with salting system for mySql
- [x] set up logging in system and character choice
  - [ ] build out character table in MySql
- [ ] work on sending images, sound, etc. over socket for users
  - [ ] maybe just sync? send

### Database Setup
Run `db/create_users_table.sql` against your MySQL instance before starting the server.
