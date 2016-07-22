## e8086 - An Intel 8086 emulator written in C# 

This is my on going project of implementing an 8086 emulator.  My goals beginning this project are:

1. A fully functional disassembler
2. A completely implemented CPU with all 8086 instructions
3. Be able to boot some version of DOS.  
4. Design goal is for easy extension into 286/386 and other instruction sets.
5. Use TDD to drive the project.

**22-Jul-2016**
All instructions except interrupts are implemented. Using console app to debug and successfully run a test program.
	
**15-Jan-2016**
The disassembler is fully working.  Beginning on the CPU.

**08-Jan-2016**
All but two instructions (the TEST instructions) are now implemented for disassembly with unit tests passing.

**07-Jan-2016**
The first 5 op codes pass unit tests.	

**01-Jan-2016**
After research and design work coding has begun on the disassembler.
