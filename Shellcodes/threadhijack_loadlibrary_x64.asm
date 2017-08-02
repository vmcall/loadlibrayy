PUSHFQ
PUSH rax
PUSH rbx
PUSH rcx
PUSH rdx
PUSH r8
PUSH r9
PUSH r10
PUSH r11

sub RSP, 0x28
movabs RCX, 0x0000000000000000	; Image path
movabs RAX, 0x0000000000000000	; Pointer to LoadLibrary
call RAX
add RSP, 0x28

POP r11
POP r10
POP r9
POP r8
POP rdx
POP rcx
POP rbx
POP rax
POPFQ

ret