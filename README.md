## e8086 - An Intel 8086 emulator written in C# 

This is my on going project of implementing an 8086 emulator.  My goals beginning this project are:

1. A fully functional disassembler
2. A completely implemented CPU with all 8086 instructions
3. Be able to boot some version of DOS.  
4. Design goal is for easy extension into 286/386 and other instruction sets.
5. Use TDD to drive the project.

**** Update for September 26, 2021
Migrate to .NET 5

**** Update for July 19, 2018

Merged into master the first phase of a major refactor of the code.  Moved instructions
into classes in an effort to eliminate some duplicated code.  More refactor to follow.