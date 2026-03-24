---
title: Keep SQL_Handler authentication methods focused on queries, not business logic
date: 2026-03-23
tags: [pattern, architecture]
type: pattern
project: DapperBanana/ASCIIAssaultServer
---

Good separation: SQL_Handler handles the DB queries (user lookup, insert, hash comparison), while ClientHandler manages the socket/game flow. But as auth logic grows (password reset, role-based access, MFA), watch that SQL_Handler doesn't become a second business logic layer.

Keep it at the query level: `ValidateUser(username, plaintext)` returns bool or user object. Let ClientHandler/higher layers decide what to do with invalid auth (retry, ban, log). This prevents duplicate validation rules across different entry points (if you later add admin panels, CLI tools, etc.).

Also: ensure SQL_Handler's login/register methods are idempotent or handle constraint violations gracefully (duplicate username, etc.)—currently MySQL likely throws on duplicate key; ClientHandler should catch and respond cleanly to the client.
