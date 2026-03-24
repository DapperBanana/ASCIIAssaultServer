---
title: BCrypt hashing in place, but watch session/token lifecycle management
date: 2026-03-23
tags: [security, gotcha]
type: gotcha
project: DapperBanana/ASCIIAssaultServer
---

Adding BCrypt for password hashing is solid—salting is handled automatically, no manual work needed. But integrating registration + login into the existing ClientHandler without stateless tokens or session expiry logic creates a hidden risk: if a client authenticates once and then disconnects, reconnecting with the same socket won't re-validate credentials unless you're explicitly tracking session state per connection. 

Right now the flow is likely: validate on login, store auth state in ClientHandler. That works for the current architecture, but as the server scales (multiple servers, reconnections, client crashes), you'll want either:
- Short-lived tokens issued at login that ClientHandler validates per message
- Or explicit session tracking with timeout/refresh logic

BCrypt itself is fine—just audit whether subsequent requests after login are re-checking credentials or trusting the connection state.
