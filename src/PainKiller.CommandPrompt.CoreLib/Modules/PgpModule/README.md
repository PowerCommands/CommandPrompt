# PGP Module Version 1.0
PGP module adds support for handling cryptographic operations using PGP (Pretty Good Privacy) standards.

DEPENDENCY:BouncyCastle.Cryptography

## Commands:
## PgpCommand
Create your own PGP keys, do not use the keys provided in this directory other then for testing purposes.

To use PGP encryption and decryption in this application, you'll need a public/private key pair. You can generate these keys using GnuPG (GPG), a widely-used open-source tool for PGP operations.

📦 Install GPG
If you don’t already have GPG installed, you can download it from:

Windows: https://gpg4win.org

macOS: brew install gnupg

Linux (Debian/Ubuntu): sudo apt install gnupg

If you prefer a graphical user interface over the command line, you can use Kleopatra, which is included in the Gpg4win package.