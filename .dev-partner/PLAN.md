# ASCII Assault Server — Development Plan

## Completed
- [x] Server/client TCP communication
- [x] MySQL database connection via config
- [x] Password hashing with PBKDF2-SHA256 (salted, 100k iterations)
- [x] User registration flow (username uniqueness check, hashed storage)
- [x] User login/authentication flow
- [x] Client handler login/register prompt before entering game
- [x] Server broadcast and client lifecycle management
- [x] MySQL users table schema

## In Progress
- [ ] Character table in MySQL and character selection after login
- [ ] Track last_login timestamp on successful auth

## Up Next
- [ ] Build character table schema and selection flow
- [ ] Asset transfer over socket (images, sound, or sync approach)
- [ ] Graceful error handling for DB connection failures
- [ ] Input validation / length limits on username and password
- [ ] Rate limiting on failed login attempts
