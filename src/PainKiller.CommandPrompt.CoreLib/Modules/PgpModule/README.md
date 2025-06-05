# PGP Module Version 1.0
PGP module adds support for handling cryptographic operations using PGP (Pretty Good Privacy) standards.

DEPENDENCY:BouncyCastle.Cryptography

## Commands:
## PgpCommand
To use PGP encryption and decryption in this application, you'll need a public/private key pair. You can generate these keys using GnuPG (GPG), a widely-used open-source tool for PGP operations.
If you want to test the PGP module, you can use the provided test keys that are placed in the test/PgpModule directory of this repository, but remember to create your own keys for production use.

📦 Install GPG
If you don’t already have GPG installed, you can download it from:

Windows: https://gpg4win.org

macOS: brew install gnupg

Linux (Debian/Ubuntu): sudo apt install gnupg

If you prefer a graphical user interface over the command line, you can use Kleopatra, which is included in the Gpg4win package.