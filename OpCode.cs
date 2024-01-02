// Decompiled with JetBrains decompiler
// Type: Pololu.Usc.Bytecode.Opcode
// Assembly: Bytecode, Version=1.1.4860.23405, Culture=neutral, PublicKeyToken=null
// MVID: A422FA04-51E0-4056-BA68-889B23015D8B
// Assembly location: C:\Program Files (x86)\Pololu\Maestro\bin\mohan\Bytecode.dll

#nullable disable
namespace Pololu.Usc.Bytecode
{
    public enum Opcode
    {
        QUIT,
        LITERAL,
        LITERAL8,
        LITERAL_N,
        LITERAL8_N,
        RETURN,
        JUMP,
        JUMP_Z,
        DELAY,
        GET_MS,
        DEPTH,
        DROP,
        DUP,
        OVER,
        PICK,
        SWAP,
        ROT,
        ROLL,
        BITWISE_NOT,
        BITWISE_AND,
        BITWISE_OR,
        BITWISE_XOR,
        SHIFT_RIGHT,
        SHIFT_LEFT,
        LOGICAL_NOT,
        LOGICAL_AND,
        LOGICAL_OR,
        NEGATE,
        PLUS,
        MINUS,
        TIMES,
        DIVIDE,
        MOD,
        POSITIVE,
        NEGATIVE,
        NONZERO,
        EQUALS,
        NOT_EQUALS,
        MIN,
        MAX,
        LESS_THAN,
        GREATER_THAN,
        SERVO,
        SERVO_8BIT,
        SPEED,
        ACCELERATION,
        GET_POSITION,
        GET_MOVING_STATE,
        LED_ON,
        LED_OFF,
        PWM,
        PEEK,
        POKE,
        SERIAL_SEND_BYTE,
        CALL,
    }
}
