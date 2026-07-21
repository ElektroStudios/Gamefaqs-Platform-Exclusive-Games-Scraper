# Gamefaqs Platform Exclusive Games Scraper Change Log 📋

## v2.0 *(current)* 🆕
- Proper integration of FlareSolverr usage, and returned HTML validation.
- Added support to scrap the game lists for Nintendo Switch 2, Oculus Quest/Meta Quest, Windows Mobile, Blackberry, BBC Micro and Commodore PET.
- Minor bugfixes affecting missing duplicated titles in the .url files when building the compressed zip archives.

This release should be fully stable. It was able to scrap the game lists in GameFAQs website for all the current 114 supported platforms entirely without suffering a ban, running for more than an entire week 24/7hrs.

## v1.0.1 🔄
- The application now strictly depends on [FlareSolverr](https://github.com/FlareSolverr/FlareSolverr) because GameFAQs introduced aggressive Cloudflare protection walls, which completely blocked HTTP web requests from downloading the game catalogs.

## v1.0 🔄
Initial Release.